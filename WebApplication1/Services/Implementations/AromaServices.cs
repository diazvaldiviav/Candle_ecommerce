using AutoMapper;
using Candle_API.Data.DTOs.Aromas;
using Candle_API.Data.Entities;
using Candle_API.Services.Interfaces;
using Candle_API.Tools;
using Microsoft.EntityFrameworkCore;

namespace Candle_API.Services.Implementations;
public class AromaService : IAromas
{
    private readonly CandleDbContext _context;
    private readonly IMapper _mapper;

    public AromaService(CandleDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AromaDTO>> GetAllAromasAsync()
    {
        var aromas = await _context.Aromas.ToListAsync();
        return _mapper.Map<IEnumerable<AromaDTO>>(aromas);
    }

    public async Task<AromaDTO> CreateAromaAsync(CreateAromaDTO dto)
    {
        var aroma = _mapper.Map<Aroma>(dto);
        await _context.Aromas.AddAsync(aroma);
        await _context.SaveChangesAsync();

        return _mapper.Map<AromaDTO>(aroma);
    }


    public async Task<ProductAromaDTO> AddAromaToProductAsync(int productId, AddAromaToProductDTO dto)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            throw new NotFoundException($"Producto con ID {productId} no encontrado");

        var aroma = await _context.Aromas.FindAsync(dto.AromaId);
        if (aroma == null)
            throw new NotFoundException($"Aroma con ID {dto.AromaId} no encontrado");

        var productAroma = new ProductAroma
        {
            ProductId = productId,
            AromaId = dto.AromaId,
            AdditionalPrice = dto.AdditionalPrice
        };

        await _context.ProductAromas.AddAsync(productAroma);
        await _context.SaveChangesAsync();

        // Cargar las relaciones para el mapeo
        await _context.Entry(productAroma)
            .Reference(pa => pa.Product)
            .LoadAsync();

        await _context.Entry(productAroma)
            .Reference(pa => pa.Aroma)
            .LoadAsync();

        return _mapper.Map<ProductAromaDTO>(productAroma);
    }


}