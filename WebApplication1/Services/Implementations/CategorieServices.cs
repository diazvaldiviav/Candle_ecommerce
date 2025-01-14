// Services/CategoryService.cs
using Candle_API.Data.Entities;
using Candle_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Candle_API.Data.DTOs.Categories;

public class CategoryService : ICategories
{
    private readonly CandleDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        CandleDbContext context,
        IMapper mapper,
        ILogger<CategoryService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }
    // Implement the methods from the interface


    //get all categories
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }


    //get category by id
    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
        return _mapper.Map<CategoryDto>(category);
    }



    //create category

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        var category = _mapper.Map<Category>(createCategoryDto);
        category.CreatedAt = DateTime.UtcNow;

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }

    //update category
    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return null;

        _mapper.Map(updateCategoryDto, category);
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }


    //delete category

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CategoryExistsAsync(int id)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id);
    }

    public async Task<CategoryDto> GetCategorieWithSubcategoriesAsync(int id)
    {
        var categorie = await _context.Categories
            .Include(c => c.Subcategories)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => id == c.Id);

        return _mapper.Map<CategoryDto>(categorie);
    }


    //anadir subcategoria a categoria
    public async Task<CategoryDto> AddSubcategoryToCategoryAsync(int categoryId, SubcategoryDto subcategoryDto)
    {
        var category = await _context.Categories
            .Include(c => c.Subcategories)
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            throw new KeyNotFoundException($"No se encontró la categoría con ID: {categoryId}");

        // Verificar si ya existe una subcategoría con el mismo nombre
        if (category.Subcategories.Any(s => s.Name.ToLower() == subcategoryDto.Name.ToLower()))
            throw new InvalidOperationException($"Ya existe una subcategoría con el nombre: {subcategoryDto.Name}");

        var subcategory = new SubCategory
        {
            Name = subcategoryDto.Name,
            Description = subcategoryDto.Description,
            ImageUrl = subcategoryDto.ImageUrl,
            CategoryId = categoryId,
            CreatedAt = DateTime.UtcNow
        };

        category.Subcategories.Add(subcategory);
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }


    //eliminar la subcategoria de la categoria
    public async Task<CategoryDto> RemoveSubcategoryFromCategoryAsync(int categoryId, int subcategoryId)
    {
        var category = await _context.Categories
            .Include(c => c.Subcategories)
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            throw new KeyNotFoundException($"No se encontró la categoría con ID: {categoryId}");

        var subcategory = category.Subcategories
            .FirstOrDefault(s => s.Id == subcategoryId);

        if (subcategory == null)
            throw new KeyNotFoundException($"No se encontró la subcategoría con ID: {subcategoryId} en la categoría {categoryId}");

        category.Subcategories.Remove(subcategory);
        _context.Subcategories.Remove(subcategory);
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }


    //actualizar subcategoria
    public async Task<CategoryDto> UpdateSubcategoryAsync(int categoryId, int subcategoryId, UpdateSubcategoryDto updateSubcategoryDto)
    {
        var category = await _context.Categories
            .Include(c => c.Subcategories)
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            throw new KeyNotFoundException($"No se encontró la categoría con ID: {categoryId}");

        var subcategory = category.Subcategories
            .FirstOrDefault(s => s.Id == subcategoryId);

        if (subcategory == null)
            throw new KeyNotFoundException($"No se encontró la subcategoría con ID: {subcategoryId} en la categoría {categoryId}");

        // Verificar si el nuevo nombre ya existe en otras subcategorías de la misma categoría
        if (category.Subcategories.Any(s => s.Id != subcategoryId &&
                                          s.Name.ToLower() == updateSubcategoryDto.Name.ToLower()))
        {
            throw new InvalidOperationException($"Ya existe una subcategoría con el nombre: {updateSubcategoryDto.Name}");
        }

        // Actualizar los campos
        subcategory.Name = updateSubcategoryDto.Name;
        subcategory.Description = updateSubcategoryDto.Description;

        await _context.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(category);
    }


    
}
