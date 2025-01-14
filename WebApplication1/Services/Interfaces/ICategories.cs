using Candle_API.Data.DTOs.Categories;

namespace Candle_API.Services.Interfaces
{
    public interface ICategories
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> CategoryExistsAsync(int id);
        Task <CategoryDto> GetCategorieWithSubcategoriesAsync(int id);
        Task<CategoryDto> AddSubcategoryToCategoryAsync(int categoryId, SubcategoryDto subcategoryDto);
        Task<CategoryDto> RemoveSubcategoryFromCategoryAsync(int categoryId, int subcategoryId);
        Task<CategoryDto> UpdateSubcategoryAsync(int categoryId, int subcategoryId, UpdateSubcategoryDto updateSubcategoryDto);
    }
}
