// Services/SizeService.cs
using AutoMapper;
using Candle_API.Data.DTOs.Size;
using Candle_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Candle_API.Services.Implementations;
public class SizeServices : ISizes
{
    private readonly CandleDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<SizeServices> _logger;

    public SizeServices(
        CandleDbContext context,
        IMapper mapper,
        ILogger<SizeServices> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<SizeDto>> GetAllSizesAsync()
    {
        var sizes = await _context.Sizes
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync();

        return _mapper.Map<IEnumerable<SizeDto>>(sizes);
    }

    public async Task<SizeDto> GetSizeByIdAsync(int id)
    {
        var size = await _context.Sizes
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (size == null)
            throw new KeyNotFoundException($"No se encontró la talla con ID: {id}");

        return _mapper.Map<SizeDto>(size);
    }
}