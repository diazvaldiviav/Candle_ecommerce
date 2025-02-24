using Candle_API.Data.DTOs.Aromas;

namespace Candle_API.Services.Interfaces
{
    public interface IAromas
    {
        Task<IEnumerable<AromaDTO>> GetAllAromasAsync();
        Task<AromaDTO> CreateAromaAsync(CreateAromaDTO dto);
        Task<ProductAromaDTO> AddAromaToProductAsync(int productId, AddAromaToProductDTO dto);
    }
}
