using Candle_API.Data.DTOs.Cart;

namespace Candle_API.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDTO> GetCartAsync(int userId);
        Task<CartOperationResponseDTO> AddToCartAsync(int userId, AddToCartDTO request);
        Task<CartOperationResponseDTO> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDTO request);
        Task<CartOperationResponseDTO> RemoveFromCartAsync(int userId, int cartItemId);
        Task<CartOperationResponseDTO> ClearCartAsync(int userId);
        Task<CartItemValidationDTO> ValidateCartItemAsync(int productId, int? colorId, int? sizeId, int? aromaId, int quantity);
        Task<CartSummaryDTO> GetCartSummaryAsync(int userId);
    }
}
