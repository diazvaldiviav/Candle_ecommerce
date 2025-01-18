using AutoMapper;
using Candle_API.Data.DTOs.Colors;
using Candle_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Candle_API.Services.Implementations
{
    public class ColorServices : IColors
    {
        private readonly CandleDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ColorServices> _logger;

        public ColorServices(
            CandleDbContext context,
            IMapper mapper,
            ILogger<ColorServices> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ColorDto>> GetAllColorsAsync()
        {
            var colors = await _context.Colors
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ColorDto>>(colors);
        }

        public async Task<ColorDto> GetColorByIdAsync(int id)
        {
            var color = await _context.Colors
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (color == null)
                throw new KeyNotFoundException($"No se encontró el color con ID: {id}");

            return _mapper.Map<ColorDto>(color);
        }
    }
}
