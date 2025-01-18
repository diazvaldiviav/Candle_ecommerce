using AutoMapper;
using Candle_API.Services.Interfaces;
using Candle_API.Data.DTOs.SubCategories;
using Microsoft.EntityFrameworkCore;

namespace Candle_API.Services.Implementations
{
    public class SubCategoriesServices: ISubCategories
    {
        private readonly CandleDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<SubCategoriesServices> _logger;

        public SubCategoriesServices(
            CandleDbContext context,
            IMapper mapper,
            ILogger<SubCategoriesServices> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SubCategoryDto>> GetAllSubcategoriesAsync()
        {
            var subcategories = await _context.Subcategories
                .AsNoTracking()
                .Include(s => s.Category)
                .OrderBy(s => s.Category.Name)
                .ThenBy(s => s.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<SubCategoryDto>>(subcategories);
        }

        public async Task<SubCategoryDto> GetSubcategoryByIdAsync(int id)
        {
            var subcategory = await _context.Subcategories
                .AsNoTracking()
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subcategory == null)
                throw new KeyNotFoundException($"No se encontró la subcategoría con ID: {id}");

            return _mapper.Map<SubCategoryDto>(subcategory);
        }
    }
}
