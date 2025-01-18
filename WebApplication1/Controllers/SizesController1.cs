// Controllers/SizesController.cs
using Candle_API.Data.DTOs.Size;
using Candle_API.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador para la gestión de tallas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SizesController : ControllerBase
{
    private readonly ISizes _sizeService;
    private readonly ILogger<SizesController> _logger;

    public SizesController(
        ISizes sizeService,
        ILogger<SizesController> logger)
    {
        _sizeService = sizeService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las tallas disponibles
    /// </summary>
    /// <returns>Lista de tallas</returns>
    /// <response code="200">Retorna la lista de tallas</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SizeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<SizeDto>>> GetSizes()
    {
        try
        {
            var sizes = await _sizeService.GetAllSizesAsync();
            return Ok(sizes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de tallas");
            return StatusCode(500, new
            {
                message = "Error interno del servidor al obtener las tallas"
            });
        }
    }

    /// <summary>
    /// Obtiene una talla específica por su ID
    /// </summary>
    /// <param name="id">ID de la talla</param>
    /// <returns>Información de la talla solicitada</returns>
    /// <response code="200">Retorna la talla solicitada</response>
    /// <response code="404">Si la talla no existe</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SizeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SizeDto>> GetSize(int id)
    {
        try
        {
            var size = await _sizeService.GetSizeByIdAsync(id);
            return Ok(size);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la talla {SizeId}", id);
            return StatusCode(500, new
            {
                message = "Error interno del servidor al obtener la talla"
            });
        }
    }
}