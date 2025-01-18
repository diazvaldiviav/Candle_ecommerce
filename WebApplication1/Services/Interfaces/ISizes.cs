using Candle_API.Data.DTOs.Size;

namespace Candle_API.Services.Interfaces
{
    public interface ISizes
    {
        Task<IEnumerable<SizeDto>> GetAllSizesAsync();
        Task<SizeDto> GetSizeByIdAsync(int id);
    }
}
