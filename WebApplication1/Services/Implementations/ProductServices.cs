using AutoMapper;
using Candle_API.Data.DTOs.Colors;
using Candle_API.Data.DTOs.Product;
using Candle_API.Data.DTOs.Size;
using Candle_API.Data.Entities;
using Candle_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Candle_API.Tools;

namespace Candle_API.Services.Implementations
{
    public class ProductServices: IProduct
    {
        private readonly CandleDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductServices> _logger;

        public ProductServices(
        CandleDbContext context,
        IMapper mapper,
        ILogger<ProductServices> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }


        // Get all products
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Set<Product>()
                .Include(p => p.SubCategory)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                    .AsNoTracking()
                .ToListAsync();


            if (!products.Any())
            {
                _logger.LogWarning("No se encontraron productos en la base de datos");
            }

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        // Get product by ID
        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Set<Product>()
                .AsNoTracking()
               .Include(p => p.SubCategory)
               .Include(p => p.ProductColors)
                   .ThenInclude(pc => pc.Color)
               .Include(p => p.ProductSizes)
                   .ThenInclude(ps => ps.Size)
                   .Include(p => p.ProductImages)
                   .Include(p => p.ProductAromas)
                   .ThenInclude(p => p.Aroma)
               .FirstOrDefaultAsync(p => p.Id == id);

                if(product == null) throw new NullReferenceException();


                return _mapper.Map<ProductDto>(product);

            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, $"Error al obtener el producto con ID: {id}");
                throw new KeyNotFoundException($"No se encontró el producto con ID: {id}");

            }

        }


        // Create a product
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            try
            {
                var subcategory = await _context.Set<SubCategory>()
               .FirstOrDefaultAsync(s => s.Id == createProductDto.SubcategoryId);

                if (subcategory == null)
                    throw new NullReferenceException();

                var product = _mapper.Map<Product>(createProductDto);
                product.CreatedAt = DateTime.UtcNow;

              
                _context.Set<Product>().Add(product);
                await _context.SaveChangesAsync();

                return await GetProductByIdAsync(product.Id);
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Error al crear un producto");
                throw new KeyNotFoundException($"No se encontró la subcategoría con ID: {createProductDto.SubcategoryId}");
            }
           
        }


        // Update a product
        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new KeyNotFoundException($"No se encontró el producto con ID: {id}");

            _mapper.Map(updateProductDto, product);
            await _context.SaveChangesAsync();

            return await GetProductByIdAsync(id);
        }

        // Delete a product

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

        // Get products by subcategory
        public async Task<IEnumerable<ProductDto>> GetProductsBySubcategoryAsync(int subcategoryId)
        {
            var products = await _context.Products
                .Include(p => p.SubCategory)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .Where(p => p.SubcategoryId == subcategoryId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }


        //product exists
        public async Task<bool> ProductExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }


        // Add size to product

        // Services/ProductService.cs
        public async Task<ProductDto> AddSizeToProductAsync(int productId, CreateSizeDto sizeDto)
        {
            var product = await _context.Products
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException($"No se encontró el producto con ID: {productId}");

            // Verificar si ya existe un tamaño con el mismo nombre
            var existingSize = await _context.Sizes
                .FirstOrDefaultAsync(s => s.Name.ToLower() == sizeDto.Name.ToLower());

            Size size;
            if (existingSize != null)
            {
                // Verificar si el producto ya tiene este tamaño
                if (product.ProductSizes.Any(ps => ps.SizeId == existingSize.Id))
                {
                    throw new InvalidOperationException(
                        $"El tamaño '{sizeDto.Name}' ya está asociado a este producto");
                }
                size = existingSize;
            }
            else
            {
                // Crear nuevo tamaño
                size = new Size
                {
                    Name = sizeDto.Name
                };
                _context.Sizes.Add(size);
                await _context.SaveChangesAsync(); // Guardar para obtener el ID
            }

            // Asociar el tamaño al producto
            product.ProductSizes.Add(new ProductSize
            {
                ProductId = productId,
                SizeId = size.Id
            });

            await _context.SaveChangesAsync();

            // Recargar el producto con todas sus relaciones
            var updatedProduct = await _context.Products
                .Include(p => p.SubCategory)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return _mapper.Map<ProductDto>(updatedProduct);
        }



        // Update product size
        public async Task<ProductDto> UpdateProductSizeAsync(int productId, int sizeId, UpdateSizeDto updateSizeDto)
        {
            // Verificar que el producto existe y cargar sus relaciones
            var product = await _context.Products
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException($"No se encontró el producto con ID: {productId}");

            // Verificar que el size está asociado al producto
            var productSize = product.ProductSizes
                .FirstOrDefault(ps => ps.SizeId == sizeId);

            if (productSize == null)
                throw new KeyNotFoundException($"No se encontró el tamaño con ID: {sizeId} en el producto {productId}");

            // Verificar si ya existe otro tamaño con el mismo nombre
            var existingSize = await _context.Sizes
                .FirstOrDefaultAsync(s => s.Name.ToLower() == updateSizeDto.Name.ToLower()
                                      && s.Id != sizeId);

            if (existingSize != null)
            {
                throw new InvalidOperationException(
                    $"Ya existe otro tamaño con el nombre '{updateSizeDto.Name}'");
            }

            // Actualizar el nombre del tamaño
            productSize.Size.Name = updateSizeDto.Name;

            await _context.SaveChangesAsync();

            // Recargar el producto con todas sus relaciones
            var updatedProduct = await _context.Products
                .Include(p => p.SubCategory)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return _mapper.Map<ProductDto>(updatedProduct);
        }




        // Remove product size
        // Services/ProductService.cs
        public async Task<ProductDto> RemoveProductSizeAsync(int productId, int sizeId)
        {
            // Verificar que el producto existe y cargar sus relaciones
            var product = await _context.Products
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException($"No se encontró el producto con ID: {productId}");

            // Verificar que el size está asociado al producto
            var productSize = product.ProductSizes
                .FirstOrDefault(ps => ps.SizeId == sizeId);

            if (productSize == null)
                throw new KeyNotFoundException($"No se encontró el tamaño con ID: {sizeId} en el producto {productId}");

            // Eliminar la relación
            product.ProductSizes.Remove(productSize);
            _context.ProductSizes.Remove(productSize);

            await _context.SaveChangesAsync();

            // Recargar el producto con todas sus relaciones
            var updatedProduct = await _context.Products
                .Include(p => p.SubCategory)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return _mapper.Map<ProductDto>(updatedProduct);
        }



        //anadir colores al producto
        public async Task<ProductDto> AddColorToProductAsync(int productId, CreateColorDto colorDto)
        {
            var product = await _context.Products
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException($"No se encontró el producto con ID: {productId}");

            // Verificar si ya existe un color con el mismo nombre o código hex
            var existingColor = await _context.Colors
                .FirstOrDefaultAsync(c => c.Name.ToLower() == colorDto.Name.ToLower()
                                      || c.HexCode.ToUpper() == colorDto.HexCode.ToUpper());

            Color color;
            if (existingColor != null)
            {
                // Verificar si el producto ya tiene este color
                if (product.ProductColors.Any(pc => pc.ColorId == existingColor.Id))
                {
                    throw new InvalidOperationException(
                        $"El color '{colorDto.Name}' ya está asociado a este producto");
                }
                color = existingColor;
            }
            else
            {
                // Crear nuevo color
                color = new Color
                {
                    Name = colorDto.Name,
                    HexCode = colorDto.HexCode.ToUpper() // Normalizar a mayúsculas
                };
                _context.Colors.Add(color);
                await _context.SaveChangesAsync(); // Guardar para obtener el ID
            }

            // Asociar el color al producto
            product.ProductColors.Add(new ProductColor
            {
                ProductId = productId,
                ColorId = color.Id
            });

            await _context.SaveChangesAsync();

            // Recargar el producto con todas sus relaciones
            var updatedProduct = await _context.Products
                .Include(p => p.SubCategory)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return _mapper.Map<ProductDto>(updatedProduct);
        }



        // Update product color

        // Services/ProductService.cs
        public async Task<ProductDto> UpdateProductColorAsync(int productId, int colorId, UpdateColorDto updateColorDto)
        {
            // Verificar que el producto existe y cargar sus relaciones
            var product = await _context.Products
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException($"No se encontró el producto con ID: {productId}");

            // Verificar que el color está asociado al producto
            var productColor = product.ProductColors
                .FirstOrDefault(pc => pc.ColorId == colorId);

            if (productColor == null)
                throw new KeyNotFoundException($"No se encontró el color con ID: {colorId} en el producto {productId}");

            // Verificar si ya existe otro color con el mismo nombre o código hex
            var existingColor = await _context.Colors
                .FirstOrDefaultAsync(c => c.Id != colorId &&
                    (c.Name.ToLower() == updateColorDto.Name.ToLower() ||
                     c.HexCode.ToUpper() == updateColorDto.HexCode.ToUpper()));

            if (existingColor != null)
            {
                throw new InvalidOperationException(
                    $"Ya existe otro color con el nombre '{updateColorDto.Name}' o el código hexadecimal '{updateColorDto.HexCode}'");
            }

            // Actualizar el color
            var color = productColor.Color;
            color.Name = updateColorDto.Name;
            color.HexCode = updateColorDto.HexCode.ToUpper(); // Normalizar a mayúsculas

            await _context.SaveChangesAsync();

            // Recargar el producto con todas sus relaciones
            var updatedProduct = await _context.Products
                .Include(p => p.SubCategory)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return _mapper.Map<ProductDto>(updatedProduct);
        }




        // Remove product color
        // Services/ProductService.cs
        public async Task<ProductDto> RemoveProductColorAsync(int productId, int colorId)
        {
            // Verificar que el producto existe y cargar sus relaciones
            var product = await _context.Products
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException($"No se encontró el producto con ID: {productId}");

            // Verificar que el color está asociado al producto
            var productColor = product.ProductColors
                .FirstOrDefault(pc => pc.ColorId == colorId);

            if (productColor == null)
                throw new KeyNotFoundException($"No se encontró el color con ID: {colorId} en el producto {productId}");

            // Verificar que no sea el último color del producto (opcional)
            if (product.ProductColors.Count <= 1)
            {
                throw new InvalidOperationException("No se puede eliminar el último color del producto");
            }

            // Eliminar la relación
            product.ProductColors.Remove(productColor);
            _context.ProductColors.Remove(productColor);

            await _context.SaveChangesAsync();

            // Recargar el producto con todas sus relaciones
            var updatedProduct = await _context.Products
                .Include(p => p.SubCategory)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return _mapper.Map<ProductDto>(updatedProduct);
        }


        // Change subcategory
        public async Task<ProductSubcategoryChangeDto> ChangeProductSubcategoryAsync(int productId, int newSubcategoryId)
        {
            // Verificar que el producto existe
            var product = await _context.Products
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException($"No se encontró el producto con ID: {productId}");

            // Verificar que la nueva subcategoría existe
            var newSubcategory = await _context.Subcategories
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == newSubcategoryId);

            if (newSubcategory == null)
                throw new KeyNotFoundException($"No se encontró la subcategoría con ID: {newSubcategoryId}");

            // Verificar que no es la misma subcategoría
            if (product.SubcategoryId == newSubcategoryId)
                throw new InvalidOperationException("El producto ya pertenece a esta subcategoría");

            // Guardar la información anterior para el mensaje
            var oldSubcategoryName = product.SubCategory.Name;
            var oldCategoryName = product.SubCategory.Category?.Name;

            // Cambiar la subcategoría
            product.SubcategoryId = newSubcategoryId;

            await _context.SaveChangesAsync();

            // Crear un objeto con información detallada del cambio
            var changeInfo = new SubcategoryChangeInfo
            {
                OldCategory = oldCategoryName,
                OldSubcategory = oldSubcategoryName,
                NewCategory = newSubcategory.Category?.Name,
                NewSubcategory = newSubcategory.Name
            };


            // Cambiar la subcategoría
            product.SubcategoryId = newSubcategoryId;
            await _context.SaveChangesAsync();

            // Recargar el producto con todas sus relaciones
            var updatedProduct = await _context.Products
                .Include(p => p.SubCategory)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return new ProductSubcategoryChangeDto
            {
                Product = _mapper.Map<ProductDto>(updatedProduct),
                ChangeInfo = changeInfo
            };



        }



        public async Task<ProductColorAssociationDto> AssociateColorAsync(int productId, AssociateColorDto associateColorDto)
        {
            var product = await _context.Products
                .Include(p => p.ProductColors)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException($"No se encontró el producto con ID: {productId}");

            var color = await _context.Colors
                .FirstOrDefaultAsync(c => c.Id == associateColorDto.ColorId);

            if (color == null)
                throw new KeyNotFoundException($"No se encontró el color con ID: {associateColorDto.ColorId}");

            // Verificar si ya existe la asociación
            if (product.ProductColors.Any(pc => pc.ColorId == associateColorDto.ColorId))
                throw new InvalidOperationException($"El color ya está asociado a este producto");

            var productColor = new ProductColor
            {
                ProductId = productId,
                ColorId = associateColorDto.ColorId,
                Stock = associateColorDto.Stock
            };

            product.ProductColors.Add(productColor);
            await _context.SaveChangesAsync();

            return new ProductColorAssociationDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Color = _mapper.Map<ColorDto>(color),
                Stock = associateColorDto.Stock
            };
        }

        //asociar tamaño al producto

        public async Task<ProductSizeAssociationDto> AssociateSizeAsync(int productId, AssociateSizeDto associateSizeDto)
        {
            var product = await _context.Products
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException($"No se encontró el producto con ID: {productId}");

            var size = await _context.Sizes
                .FirstOrDefaultAsync(s => s.Id == associateSizeDto.SizeId);

            if (size == null)
                throw new KeyNotFoundException($"No se encontró el tamaño con ID: {associateSizeDto.SizeId}");

            // Verificar si ya existe la asociación
            if (product.ProductSizes.Any(ps => ps.SizeId == associateSizeDto.SizeId))
                throw new InvalidOperationException($"El tamaño ya está asociado a este producto");

            var productSize = new ProductSize
            {
                ProductId = productId,
                SizeId = associateSizeDto.SizeId,
                Stock = associateSizeDto.Stock
            };

            product.ProductSizes.Add(productSize);
            await _context.SaveChangesAsync();

            return new ProductSizeAssociationDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Size = _mapper.Map<SizeDto>(size),
                Stock = associateSizeDto.Stock
            };
        }


        //add image to product
        public async Task<ProductResponseDTO> AddProductImagesAsync(AddProductImagesDTO imagesDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == imagesDto.ProductId);

                if (product == null)
                {
                    throw new NotFoundException($"No se encontró el producto con ID {imagesDto.ProductId}");
                }

                // Verificar el límite de imágenes
                var currentImagesCount = product.ProductImages?.Count ?? 0;
                if (currentImagesCount + imagesDto.ImageUrls.Count > 5)
                {
                    throw new BadRequestException($"El producto no puede tener más de 5 imágenes. Actualmente tiene {currentImagesCount}");
                }

                // Agregar las nuevas imágenes
                foreach (var imageUrl in imagesDto.ImageUrls)
                {
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = imageUrl,
                        IsMain = !product.ProductImages.Any(x => x.IsMain), // Primera imagen será principal si no hay otra
                        ProductId = product.Id
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<ProductResponseDTO>(product);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        //update main image
        public async Task<ProductResponseDTO> SetMainImageAsync(UpdateMainImageDTO updateDto)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == updateDto.ProductId);

            if (product == null)
            {
                throw new NotFoundException($"No se encontró el producto con ID {updateDto.ProductId}");
            }

            var newMainImage = product.ProductImages?
                .FirstOrDefault(i => i.Id == updateDto.ImageId);

            if (newMainImage == null)
            {
                throw new NotFoundException($"No se encontró la imagen con ID {updateDto.ImageId} para el producto {updateDto.ProductId}");
            }

            // Primero, quitar el estado IsMain de la imagen principal actual si existe
            var currentMainImage = product.ProductImages?
                .FirstOrDefault(i => i.IsMain);

            if (currentMainImage != null)
            {
                currentMainImage.IsMain = false;
            }

            // Establecer la nueva imagen como principal
            newMainImage.IsMain = true;

            await _context.SaveChangesAsync();
            return _mapper.Map<ProductResponseDTO>(product);
        }


        //remove image
        public async Task<ProductResponseDTO> DeleteProductImageAsync(int productId, int imageId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    throw new NotFoundException($"No se encontró el producto con ID {productId}");
                }

                if (product.ProductImages == null || !product.ProductImages.Any())
                {
                    throw new NotFoundException($"El producto {productId} no tiene imágenes");
                }

                // Validar que no sea la única imagen
                if (product.ProductImages.Count == 1)
                {
                    throw new BadRequestException("No se puede eliminar la única imagen del producto");
                }

                var imageToDelete = product.ProductImages
                    .FirstOrDefault(i => i.Id == imageId);

                if (imageToDelete == null)
                {
                    throw new NotFoundException($"No se encontró la imagen con ID {imageId} para el producto {productId}");
                }

                // Si la imagen a eliminar es la principal, asignar otra imagen como principal
                if (imageToDelete.IsMain && product.ProductImages.Count > 1)
                {
                    var newMainImage = product.ProductImages
                        .First(i => i.Id != imageId);
                    newMainImage.IsMain = true;
                }

                _context.ProductImages.Remove(imageToDelete);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<ProductResponseDTO>(product);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


    }
}
