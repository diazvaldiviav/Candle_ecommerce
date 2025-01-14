// Controllers/CategoriesController.cs
using Candle_API.Data.DTOs.Categories;
using Candle_API.Data.Entities;
using Candle_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategories _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategories categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    // GET: api/Categories
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Category>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    // GET: api/Categories/5
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la categoría debe ser mayor que 0" });
            }

            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound(new { message = $"No se encontró la categoría con ID: {id}" });
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la categoría con ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor al obtener la categoría" });
        }
    }

    // POST: api/Categories
    [HttpPost]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        try
        {
            if (createCategoryDto == null)
            {
                return BadRequest(new { message = "La categoría no puede ser nula" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos de categoría inválidos", errors = ModelState });
            }

            if (string.IsNullOrWhiteSpace(createCategoryDto.Name))
            {
                return BadRequest(new { message = "El nombre de la categoría es requerido" });
            }

            var createdCategory = await _categoryService.CreateCategoryAsync(createCategoryDto);

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = createdCategory.Id },
                createdCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la categoría: {@Category}", createCategoryDto);
            return StatusCode(500, new { message = "Error interno del servidor al crear la categoría" });
        }
    }

    // PUT: api/Categories/5
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Category), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la categoría debe ser mayor que 0" });
            }

            if (updateCategoryDto == null)
            {
                return BadRequest(new { message = "La categoría no puede ser nula" });
            }

            if (id != updateCategoryDto.Id)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del objeto" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Datos de categoría inválidos", errors = ModelState });
            }

            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);

            if (updatedCategory == null)
            {
                return NotFound(new { message = $"No se encontró la categoría con ID: {id}" });
            }

            return Ok(updatedCategory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la categoría con ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor al actualizar la categoría" });
        }
    }

    // DELETE: api/Categories/5
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID de la categoría debe ser mayor que 0" });
            }

            var result = await _categoryService.DeleteCategoryAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"No se encontró la categoría con ID: {id}" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la categoría con ID: {Id}", id);
            return StatusCode(500, new { message = "Error interno del servidor al eliminar la categoría" });
        }
    }


    // anadir subcategoria a categoria
    [HttpPost("{id}/subcategories")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> AddSubcategoryToCategory(int id, SubcategoryDto subcategoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Datos de subcategoría inválidos",
                    errors = ModelState
                });
            }

            var result = await _categoryService.AddSubcategoryToCategoryAsync(id, subcategoryDto);

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = result.Id },
                result
            );
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al añadir subcategoría a la categoría {CategoryId}", id);
            return StatusCode(500, new
            {
                message = "Error interno del servidor al añadir la subcategoría"
            });
        }
    }

    // Ver categorías con subcategorías
    [HttpGet("with-subcategories/{id}")]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> GetCategoriesWithSubcategories(int id)
    {
        try
        {
            var categorie = await _categoryService.GetCategorieWithSubcategoriesAsync(id);

            if (categorie == null)
            {
                return NotFound("No se encontro la categoria que estas buscando"); // Retorna 404
            }

            return Ok(categorie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las categorías con subcategorías");
            return StatusCode(500, new
            {
                message = "Error interno del servidor al obtener las categorías con subcategorías",
                error = ex.Message
            });
        }
    }


    // Eliminar subcategoria de categoria
    [HttpDelete("{categoryId}/subcategories/{subcategoryId}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> RemoveSubcategoryFromCategory(int categoryId, int subcategoryId)
    {
        try
        {
            if (categoryId <= 0)
                return BadRequest(new { message = "El ID de la categoría debe ser mayor que 0" });

            if (subcategoryId <= 0)
                return BadRequest(new { message = "El ID de la subcategoría debe ser mayor que 0" });

            var result = await _categoryService.RemoveSubcategoryFromCategoryAsync(categoryId, subcategoryId);

            return Ok(new
            {
                message = $"Subcategoría {subcategoryId} eliminada exitosamente de la categoría {categoryId}",
                category = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la subcategoría {SubcategoryId} de la categoría {CategoryId}",
                subcategoryId, categoryId);

            return StatusCode(500, new
            {
                message = "Error interno del servidor al eliminar la subcategoría",
                error = ex.Message
            });
        }
    }


    // Actualizar subcategoria de categoria
    // Controllers/CategoriesController.cs
    [HttpPut("{categoryId}/subcategories/{subcategoryId}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> UpdateSubcategory(
        int categoryId,
        int subcategoryId,
        UpdateSubcategoryDto updateSubcategoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Datos de subcategoría inválidos",
                    errors = ModelState
                });
            }

            if (categoryId <= 0)
                return BadRequest(new { message = "El ID de la categoría debe ser mayor que 0" });

            if (subcategoryId <= 0)
                return BadRequest(new { message = "El ID de la subcategoría debe ser mayor que 0" });

            var result = await _categoryService.UpdateSubcategoryAsync(
                categoryId,
                subcategoryId,
                updateSubcategoryDto
            );

            return Ok(new
            {
                message = $"Subcategoría {subcategoryId} actualizada exitosamente",
                category = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error al actualizar la subcategoría {SubcategoryId} de la categoría {CategoryId}",
                subcategoryId,
                categoryId);

            return StatusCode(500, new
            {
                message = "Error interno del servidor al actualizar la subcategoría",
                error = ex.Message
            });
        }
    }
}