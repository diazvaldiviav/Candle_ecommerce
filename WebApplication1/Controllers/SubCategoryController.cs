// Controllers/SubcategoriesController.cs
using Candle_API.Services.Interfaces;
using Candle_API.Data.DTOs.SubCategories;


using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador para la gestión de subcategorías
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SubcategoriesController : ControllerBase
{
    private readonly ISubCategories _subcategoryService;
    private readonly ILogger<SubcategoriesController> _logger;

    public SubcategoriesController(
        ISubCategories subcategoryService,
        ILogger<SubcategoriesController> logger)
    {
        _subcategoryService = subcategoryService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las subcategorías disponibles
    /// </summary>
    /// <returns>Lista de subcategorías</returns>
    /// <response code="200">Retorna la lista de subcategorías</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SubCategoryDto>>> GetSubcategories()
    {
        try
        {
            var subcategories = await _subcategoryService.GetAllSubcategoriesAsync();
            return Ok(subcategories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de subcategorías");
            return StatusCode(500, new
            {
                message = "Error interno del servidor al obtener las subcategorías"
            });
        }
    }

    /// <summary>
    /// Obtiene una subcategoría específica por su ID
    /// </summary>
    /// <param name="id">ID de la subcategoría</param>
    /// <returns>Información de la subcategoría solicitada</returns>
    /// <response code="200">Retorna la subcategoría solicitada</response>
    /// <response code="404">Si la subcategoría no existe</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SubCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubCategoryDto>> GetSubcategory(int id)
    {
        try
        {
            var subcategory = await _subcategoryService.GetSubcategoryByIdAsync(id);
            return Ok(subcategory);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la subcategoría {SubcategoryId}", id);
            return StatusCode(500, new
            {
                message = "Error interno del servidor al obtener la subcategoría"
            });
        }
    }
}