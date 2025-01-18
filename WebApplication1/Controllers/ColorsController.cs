// Controllers/ColorsController.cs
using Candle_API.Data.DTOs.Colors;
using Candle_API.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador para la gestión de colores
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ColorsController : ControllerBase
{
    private readonly IColors _colorService;
    private readonly ILogger<ColorsController> _logger;

    public ColorsController(
        IColors colorService,
        ILogger<ColorsController> logger)
    {
        _colorService = colorService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los colores disponibles
    /// </summary>
    /// <returns>Lista de colores</returns>
    /// <response code="200">Retorna la lista de colores</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ColorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ColorDto>>> GetColors()
    {
        try
        {
            var colors = await _colorService.GetAllColorsAsync();
            return Ok(colors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la lista de colores");
            return StatusCode(500, new
            {
                message = "Error interno del servidor al obtener los colores"
            });
        }
    }

    /// <summary>
    /// Obtiene un color específico por su ID
    /// </summary>
    /// <param name="id">ID del color</param>
    /// <returns>Información del color solicitado</returns>
    /// <response code="200">Retorna el color solicitado</response>
    /// <response code="404">Si el color no existe</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ColorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ColorDto>> GetColor(int id)
    {
        try
        {
            var color = await _colorService.GetColorByIdAsync(id);
            return Ok(color);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el color {ColorId}", id);
            return StatusCode(500, new
            {
                message = "Error interno del servidor al obtener el color"
            });
        }
    }
}