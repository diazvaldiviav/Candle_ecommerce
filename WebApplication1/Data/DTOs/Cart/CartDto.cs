using System.ComponentModel.DataAnnotations;

namespace Candle_API.Data.DTOs.Cart
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDTO> Items { get; set; } = new List<CartItemDTO>();
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CartItemDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }

        // Propiedades opcionales de personalización
        public int? ColorId { get; set; }
        public string? ColorName { get; set; }
        public int? SizeId { get; set; }
        public string? SizeName { get; set; }
        public int? AromaId { get; set; }
        public string? AromaName { get; set; }
    }

    public class AddToCartDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100")]
        public int Quantity { get; set; }

        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int? AromaId { get; set; }
    }

    public class UpdateCartItemDTO
    {
        [Required]
        [Range(0, 100, ErrorMessage = "La cantidad debe estar entre 0 y 100")]
        public int Quantity { get; set; }

        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int? AromaId { get; set; }
    }

    public class CartSummaryDTO
    {
        public int ItemCount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }

    public class CartOperationResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public CartSummaryDTO CartSummary { get; set; }
    }

    public class CartItemValidationDTO
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public bool IsInStock { get; set; }
        public int AvailableStock { get; set; }
        public decimal CurrentPrice { get; set; }
        public bool HasValidCombination { get; set; }
    }
}
