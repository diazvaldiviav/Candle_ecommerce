using Candle_API.Data.DTOs.Product;

namespace Candle_API.Services.Interfaces
{
    public interface IProduct
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<ProductDto>> GetProductsBySubcategoryAsync(int subcategoryId);
        Task<bool> ProductExistsAsync(int id);
        Task<ProductDto> AddSizeToProductAsync(int productId, CreateSizeDto sizeDto);
        Task<ProductDto> UpdateProductSizeAsync(int productId, int sizeId, UpdateSizeDto updateSizeDto);
        Task<ProductDto> RemoveProductSizeAsync(int productId, int sizeId);

        Task<ProductDto> AddColorToProductAsync(int productId, CreateColorDto colorDto);
        Task<ProductDto> UpdateProductColorAsync(int productId, int colorId, UpdateColorDto updateColorDto);

        Task<ProductDto> RemoveProductColorAsync(int productId, int colorId);

        Task<ProductSubcategoryChangeDto> ChangeProductSubcategoryAsync(int productId, int newSubcategoryId);

        Task<ProductColorAssociationDto> AssociateColorAsync(int productId, AssociateColorDto associateColorDto);
        Task<ProductSizeAssociationDto> AssociateSizeAsync(int productId, AssociateSizeDto associateSizeDto);
    }

}

