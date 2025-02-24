using AutoMapper;
using Candle_API.Data.DTOs.Cart;
using Candle_API.Data.Entities;
using Candle_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Candle_API.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly CandleDbContext _context;
        private readonly ILogger<CartService> _logger;
        private readonly IMapper _mapper;
        private const decimal TAX_RATE = 0.07m; // 7% de impuesto

        public CartService(CandleDbContext context, ILogger<CartService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<CartDTO> GetCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            // Cargar el carrito con todas sus relaciones
            await _context.Entry(cart)
                .Collection(c => c.CartItems)
                .Query()
                .Include(ci => ci.Product)
                    .ThenInclude(p => p.ProductImages)
                .Include(ci => ci.Color)
                .Include(ci => ci.Size)
                .Include(ci => ci.Aroma)
                .LoadAsync();

            // Mapear el carrito completo
            var cartDTO = _mapper.Map<CartDTO>(cart);
            CalculateCartTotals(cartDTO);

            return cartDTO;
        }

        public async Task<CartOperationResponseDTO> AddToCartAsync(int userId, AddToCartDTO request)
        {
            try
            {
                _logger.LogInformation($"Iniciando AddToCartAsync para usuario {userId} con producto {request.ProductId}");

                var validation = await ValidateCartItemAsync(
                    request.ProductId,
                    request.ColorId,
                    request.SizeId,
                    request.AromaId,
                    request.Quantity
                );

                if (!validation.IsValid)
                {
                    _logger.LogWarning($"Validación fallida: {validation.Message}");
                    return new CartOperationResponseDTO
                    {
                        Success = false,
                        Message = validation.Message
                    };
                }

                var cart = await GetOrCreateCartAsync(userId);
                var existingItem = await FindExistingCartItemAsync(cart.Id, request);

                if (existingItem != null)
                {
                    _logger.LogInformation($"Actualizando item existente {existingItem.Id}");
                    existingItem.Quantity += request.Quantity;
                    existingItem.UnitPrice = validation.CurrentPrice;
                }
                else
                {
                    _logger.LogInformation("Creando nuevo item en el carrito");
                    var newItem = new CartItem
                    {
                       
                        CartId = cart.Id,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        UnitPrice = validation.CurrentPrice,
                        ColorId = request.ColorId,
                        SizeId = request.SizeId,
                        AromaId = request.AromaId
                    };
                    _context.CartItems.Add(newItem);
                }

                cart.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Guardando cambios en la base de datos");
                await _context.SaveChangesAsync();

                var summary = await GetCartSummaryAsync(userId);
                return new CartOperationResponseDTO
                {
                    Success = true,
                    Message = "Producto agregado al carrito exitosamente",
                    CartSummary = summary
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar item al carrito para el usuario {UserId}", userId);
                throw;
            }
        }

        public async Task<CartOperationResponseDTO> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDTO request)
        {
            var cartItem = await GetCartItemAsync(userId, cartItemId);
            if (cartItem == null)
            {
                return new CartOperationResponseDTO
                {
                    Success = false,
                    Message = "Item no encontrado en el carrito"
                };
            }

            if (request.Quantity == 0)
            {
                return await RemoveFromCartAsync(userId, cartItemId);
            }

            var validation = await ValidateCartItemAsync(
                cartItem.ProductId,
                request.ColorId,
                request.SizeId,
                request.AromaId,
                request.Quantity
            );

            if (!validation.IsValid)
            {
                return new CartOperationResponseDTO
                {
                    Success = false,
                    Message = validation.Message
                };
            }

            cartItem.Quantity = request.Quantity;
            cartItem.UnitPrice = validation.CurrentPrice;
            cartItem.ColorId = request.ColorId;
            cartItem.SizeId = request.SizeId;
            cartItem.AromaId = request.AromaId;

            cartItem.Cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var summary = await GetCartSummaryAsync(userId);
            return new CartOperationResponseDTO
            {
                Success = true,
                Message = "Carrito actualizado exitosamente",
                CartSummary = summary
            };
        }

        public async Task<CartOperationResponseDTO> RemoveFromCartAsync(int userId, int cartItemId)
        {
            var cartItem = await GetCartItemAsync(userId, cartItemId);
            if (cartItem == null)
            {
                return new CartOperationResponseDTO
                {
                    Success = false,
                    Message = "Item no encontrado en el carrito"
                };
            }

            _context.CartItems.Remove(cartItem);
            cartItem.Cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var summary = await GetCartSummaryAsync(userId);
            return new CartOperationResponseDTO
            {
                Success = true,
                Message = "Item eliminado del carrito",
                CartSummary = summary
            };
        }

        public async Task<CartOperationResponseDTO> ClearCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var items = await _context.CartItems
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new CartOperationResponseDTO
            {
                Success = true,
                Message = "Carrito vaciado exitosamente",
                CartSummary = new CartSummaryDTO
                {
                    ItemCount = 0,
                    SubTotal = 0,
                    Tax = 0,
                    Total = 0
                }
            };
        }

        // Métodos privados de ayuda
        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }


        private void CalculateCartTotals(CartDTO cart)
        {
            cart.SubTotal = cart.Items.Sum(item => item.Subtotal);
            cart.Tax = cart.SubTotal * TAX_RATE;
            cart.Total = cart.SubTotal + cart.Tax;
            cart.ItemCount = cart.Items.Sum(item => item.Quantity);
        }


        public async Task<CartItemValidationDTO> ValidateCartItemAsync(
   int productId,
   int? colorId,
   int? sizeId,
   int? aromaId,
   int quantity)
        {
            try
            {
                // Primero verificamos que el producto exista
                var product = await _context.Products
                    .Include(p => p.ProductColors.Where(pc => pc.ColorId == colorId))
                    .Include(p => p.ProductSizes.Where(ps => ps.SizeId == sizeId))
                    .Include(p => p.ProductAromas.Where(pa => pa.AromaId == aromaId))
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    _logger.LogWarning($"Producto no encontrado: {productId}");
                    return new CartItemValidationDTO
                    {
                        IsValid = false,
                        Message = "Producto no encontrado",
                        CurrentPrice = 0
                    };
                }

                // Validar stock
                if (product.Stock < quantity)
                {
                    _logger.LogWarning($"Stock insuficiente para producto {productId}. Solicitado: {quantity}, Disponible: {product.Stock}");
                    return new CartItemValidationDTO
                    {
                        IsValid = false,
                        Message = $"Stock insuficiente. Stock disponible: {product.Stock}",
                        IsInStock = false,
                        AvailableStock = product.Stock,
                        CurrentPrice = product.Price
                    };
                }

                // Validar las opciones seleccionadas
                var validationErrors = new List<string>();

                if (colorId.HasValue)
                {
                    var validColor = product.ProductColors.Any();
                    if (!validColor)
                        validationErrors.Add($"Color {colorId} no disponible para este producto");
                }

                if (sizeId.HasValue)
                {
                    var validSize = product.ProductSizes.Any();
                    if (!validSize)
                        validationErrors.Add($"Tamaño {sizeId} no disponible para este producto");
                }

                if (aromaId.HasValue)
                {
                    var validAroma = product.ProductAromas.Any();
                    if (!validAroma)
                        validationErrors.Add($"Aroma {aromaId} no disponible para este producto");
                }

                if (validationErrors.Any())
                {
                    _logger.LogWarning($"Validación fallida para producto {productId}: {string.Join(", ", validationErrors)}");
                    return new CartItemValidationDTO
                    {
                        IsValid = false,
                        Message = string.Join(". ", validationErrors),
                        HasValidCombination = false,
                        CurrentPrice = product.Price
                    };
                }

                // Calcular precio final
                decimal finalPrice = product.Price;
                if (colorId.HasValue && product.ProductColors.Any())
                    finalPrice += product.ProductColors.First().AddicionalPrice;
                if (sizeId.HasValue && product.ProductSizes.Any())
                    finalPrice += product.ProductSizes.First().AddicionalPrice;
                if (aromaId.HasValue && product.ProductAromas.Any())
                    finalPrice += product.ProductAromas.First().AdditionalPrice;

                return new CartItemValidationDTO
                {
                    IsValid = true,
                    IsInStock = true,
                    AvailableStock = product.Stock,
                    CurrentPrice = finalPrice,
                    HasValidCombination = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validando item del carrito para producto {productId}");
                throw;
            }
        }

        public async Task<CartSummaryDTO> GetCartSummaryAsync(int userId)
            {
                var cart = await GetCartAsync(userId);
                return new CartSummaryDTO
                {
                    ItemCount = cart.ItemCount,
                    SubTotal = cart.SubTotal,
                    Tax = cart.Tax,
                    Total = cart.Total
                };
            }

           

          

            private async Task<CartItem> GetCartItemAsync(int userId, int cartItemId)
            {
                return await _context.CartItems
                    .Include(ci => ci.Cart)
                    .FirstOrDefaultAsync(ci =>
                        ci.Id == cartItemId &&
                        ci.Cart.UserId == userId &&
                        ci.Cart.IsActive);
            }

            private async Task<CartItem> FindExistingCartItemAsync(int cartId, AddToCartDTO request)
            {
                return await _context.CartItems
                    .FirstOrDefaultAsync(ci =>
                        ci.CartId == cartId &&
                        ci.ProductId == request.ProductId &&
                        ci.ColorId == request.ColorId &&
                        ci.SizeId == request.SizeId &&
                        ci.AromaId == request.AromaId);
            }
        }





    }

