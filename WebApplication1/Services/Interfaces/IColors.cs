using Candle_API.Data.DTOs.Colors;

namespace Candle_API.Services.Interfaces
{
    public interface IColors
    {
        Task<IEnumerable<ColorDto>> GetAllColorsAsync();
        Task<ColorDto> GetColorByIdAsync(int id);

    }
}
