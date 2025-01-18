using Candle_API.Data.DTOs.SubCategories;

namespace Candle_API.Services.Interfaces
{
    public interface ISubCategories
    {
        Task<IEnumerable<SubCategoryDto>> GetAllSubcategoriesAsync();
        Task<SubCategoryDto> GetSubcategoryByIdAsync(int id);
    }
}
