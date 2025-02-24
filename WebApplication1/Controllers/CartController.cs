using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using global::Candle_API.Data.DTOs.Cart;
using global::Candle_API.Services.Interfaces;



namespace Candle_API.Controllers
{
  

    namespace Candle_API.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        [Authorize]
        public class CartController : ControllerBase
        {
            private readonly ICartService _cartService;
            private readonly ILogger<CartController> _logger;

            public CartController(ICartService cartService, ILogger<CartController> logger)
            {
                _cartService = cartService;
                _logger = logger;
            }

            /// <summary>
            /// Obtiene el carrito del usuario actual
            /// </summary>
            [HttpGet]
            [ProducesResponseType(typeof(CartDTO), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<CartDTO>> GetCart()
            {
                try
                {
                    var userId = GetUserId();
                    var cart = await _cartService.GetCartAsync(userId);
                    return Ok(cart);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener el carrito");
                    return StatusCode(500, new { message = "Error al obtener el carrito" });
                }
            }

            /// <summary>
            /// Obtiene el resumen del carrito (cantidad de items y totales)
            /// </summary>
            [HttpGet("summary")]
            [ProducesResponseType(typeof(CartSummaryDTO), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<CartSummaryDTO>> GetCartSummary()
            {
                try
                {
                    var userId = GetUserId();
                    var summary = await _cartService.GetCartSummaryAsync(userId);
                    return Ok(summary);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener el resumen del carrito");
                    return StatusCode(500, new { message = "Error al obtener el resumen del carrito" });
                }
            }

            /// <summary>
            /// Agrega un producto al carrito
            /// </summary>
            [HttpPost("items")]
            [ProducesResponseType(typeof(CartOperationResponseDTO), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<CartOperationResponseDTO>> AddToCart([FromBody] AddToCartDTO request)
            {
                try
                {
                    var userId = GetUserId();

                    var validation = await _cartService.ValidateCartItemAsync(
                        request.ProductId,
                        request.ColorId,
                        request.SizeId,
                        request.AromaId,
                        request.Quantity
                    );

                    if (!validation.IsValid)
                    {
                        return BadRequest(new { message = validation.Message });
                    }

                    var result = await _cartService.AddToCartAsync(userId, request);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al agregar item al carrito");
                    return StatusCode(500, new { message = "Error al agregar item al carrito" });
                }
            }

            /// <summary>
            /// Actualiza la cantidad de un item en el carrito
            /// </summary>
            [HttpPut("items/{cartItemId}")]
            [ProducesResponseType(typeof(CartOperationResponseDTO), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<CartOperationResponseDTO>> UpdateCartItem(
                int cartItemId,
                [FromBody] UpdateCartItemDTO request)
            {
                try
                {
                    var userId = GetUserId();
                    var result = await _cartService.UpdateCartItemAsync(userId, cartItemId, request);

                    if (!result.Success)
                    {
                        return BadRequest(new { message = result.Message });
                    }

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar item del carrito");
                    return StatusCode(500, new { message = "Error al actualizar item del carrito" });
                }
            }

            /// <summary>
            /// Elimina un item del carrito
            /// </summary>
            [HttpDelete("items/{cartItemId}")]
            [ProducesResponseType(typeof(CartOperationResponseDTO), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<CartOperationResponseDTO>> RemoveFromCart(int cartItemId)
            {
                try
                {
                    var userId = GetUserId();
                    var result = await _cartService.RemoveFromCartAsync(userId, cartItemId);

                    if (!result.Success)
                    {
                        return NotFound(new { message = result.Message });
                    }

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar item del carrito");
                    return StatusCode(500, new { message = "Error al eliminar item del carrito" });
                }
            }

            /// <summary>
            /// Vacía el carrito completamente
            /// </summary>
            [HttpDelete]
            [ProducesResponseType(typeof(CartOperationResponseDTO), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<CartOperationResponseDTO>> ClearCart()
            {
                try
                {
                    var userId = GetUserId();
                    var result = await _cartService.ClearCartAsync(userId);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al vaciar el carrito");
                    return StatusCode(500, new { message = "Error al vaciar el carrito" });
                }
            }

            private int GetUserId()
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    throw new UnauthorizedAccessException("Usuario no autenticado");
                }
                return int.Parse(userIdClaim.Value);
            }
        }
    }
}
