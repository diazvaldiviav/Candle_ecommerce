using Candle_API.Data.DTOs.Product;
using Microsoft.AspNetCore.Mvc;
using Candle_API.Services.Interfaces;

namespace Candle_API.Controllers
{
    // Controllers/ProductsController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProduct _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProduct productService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los productos");
                return StatusCode(500, new { message = "Error interno del servidor al obtener los productos" });
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto {ProductId}", id);
                return StatusCode(500, new { message = "Error interno del servidor al obtener el producto" });
            }
        }

        // GET: api/Products/subcategory/5
        [HttpGet("subcategory/{subcategoryId}")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsBySubcategory(int subcategoryId)
        {
            try
            {
                var products = await _productService.GetProductsBySubcategoryAsync(subcategoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos de la subcategoría {SubcategoryId}", subcategoryId);
                return StatusCode(500, new { message = "Error interno del servidor al obtener los productos por subcategoría" });
            }
        }

        // POST: api/Products
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Datos del producto inválidos",
                        errors = ModelState
                    });
                }

                var product = await _productService.CreateProductAsync(createProductDto);

                return CreatedAtAction(
                    nameof(GetProduct),
                    new { id = product.Id },
                    product
                );
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto");
                return StatusCode(500, new { message = "Error interno del servidor al crear el producto" });
            }
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Datos del producto inválidos",
                        errors = ModelState
                    });
                }

                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto {ProductId}", id);
                return StatusCode(500, new { message = "Error interno del servidor al actualizar el producto" });
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);

                if (!result)
                {
                    return NotFound(new { message = $"No se encontró el producto con ID: {id}" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto {ProductId}", id);
                return StatusCode(500, new { message = "Error interno del servidor al eliminar el producto" });
            }
        }


        // POST: api/Products/5/sizes
        // Controllers/ProductsController.cs
        [HttpPost("{id}/size/new")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> AddNewSizeToProduct(int id, [FromBody] CreateSizeDto sizeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Datos inválidos",
                        errors = ModelState
                    });
                }

                if (id <= 0)
                {
                    return BadRequest(new { message = "El ID del producto debe ser mayor que 0" });
                }

                var product = await _productService.AddSizeToProductAsync(id, sizeDto);

                return Ok(new
                {
                    message = $"Tamaño '{sizeDto.Name}' agregado exitosamente al producto {id}",
                    product
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
                _logger.LogError(ex, "Error al agregar nuevo tamaño al producto {ProductId}", id);
                return StatusCode(500, new
                {
                    message = "Error interno del servidor al agregar el tamaño al producto"
                });
            }
        }


        //update size
        // Controllers/ProductsController.cs
        [HttpPut("{productId}/size/{sizeId}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> UpdateProductSize(
            int productId,
            int sizeId,
            [FromBody] UpdateSizeDto updateSizeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Datos inválidos",
                        errors = ModelState
                    });
                }

                if (productId <= 0)
                {
                    return BadRequest(new { message = "El ID del producto debe ser mayor que 0" });
                }

                if (sizeId <= 0)
                {
                    return BadRequest(new { message = "El ID del tamaño debe ser mayor que 0" });
                }

                var product = await _productService.UpdateProductSizeAsync(productId, sizeId, updateSizeDto);

                return Ok(new
                {
                    message = $"Tamaño actualizado exitosamente a '{updateSizeDto.Name}'",
                    product
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
                    "Error al actualizar el tamaño {SizeId} del producto {ProductId}",
                    sizeId,
                    productId);

                return StatusCode(500, new
                {
                    message = "Error interno del servidor al actualizar el tamaño"
                });
            }
        }




        //eliminar el size del producto
        [HttpDelete("{productId}/size/{sizeId}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> RemoveProductSize(int productId, int sizeId)
        {
            try
            {
                if (productId <= 0)
                {
                    return BadRequest(new { message = "El ID del producto debe ser mayor que 0" });
                }

                if (sizeId <= 0)
                {
                    return BadRequest(new { message = "El ID del tamaño debe ser mayor que 0" });
                }

                var product = await _productService.RemoveProductSizeAsync(productId, sizeId);

                return Ok(new
                {
                    message = $"Tamaño eliminado exitosamente del producto {productId}",
                    product
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al eliminar el tamaño {SizeId} del producto {ProductId}",
                    sizeId,
                    productId);

                return StatusCode(500, new
                {
                    message = "Error interno del servidor al eliminar el tamaño"
                });
            }
        }


        //Anadir color al producto
        // Controllers/ProductsController.cs
[HttpPost("{id}/color")]
[ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<ProductDto>> AddColorToProduct(int id, [FromBody] CreateColorDto colorDto)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new 
            { 
                message = "Datos inválidos",
                errors = ModelState
            });
        }

        if (id <= 0)
        {
            return BadRequest(new { message = "El ID del producto debe ser mayor que 0" });
        }

        var product = await _productService.AddColorToProductAsync(id, colorDto);
        
        return Ok(new
        {
            message = $"Color '{colorDto.Name}' agregado exitosamente al producto {id}",
            product
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
        _logger.LogError(ex, "Error al agregar color al producto {ProductId}", id);
        return StatusCode(500, new 
        { 
            message = "Error interno del servidor al agregar el color al producto"
        });
    }
}


        //update color
        [HttpPut("{productId}/color/{colorId}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> UpdateProductColor(
            int productId,
            int colorId,
            [FromBody] UpdateColorDto updateColorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Datos inválidos",
                        errors = ModelState
                    });
                }

                if (productId <= 0)
                {
                    return BadRequest(new { message = "El ID del producto debe ser mayor que 0" });
                }

                if (colorId <= 0)
                {
                    return BadRequest(new { message = "El ID del color debe ser mayor que 0" });
                }

                var product = await _productService.UpdateProductColorAsync(productId, colorId, updateColorDto);

                return Ok(new
                {
                    message = $"Color actualizado exitosamente a '{updateColorDto.Name}'",
                    product
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
                    "Error al actualizar el color {ColorId} del producto {ProductId}",
                    colorId,
                    productId);

                return StatusCode(500, new
                {
                    message = "Error interno del servidor al actualizar el color"
                });
            }
        }



        //Remove color from product
        // Controllers/ProductsController.cs
        [HttpDelete("{productId}/color/{colorId}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> RemoveProductColor(int productId, int colorId)
        {
            try
            {
                if (productId <= 0)
                {
                    return BadRequest(new { message = "El ID del producto debe ser mayor que 0" });
                }

                if (colorId <= 0)
                {
                    return BadRequest(new { message = "El ID del color debe ser mayor que 0" });
                }

                var product = await _productService.RemoveProductColorAsync(productId, colorId);

                return Ok(new
                {
                    message = $"Color eliminado exitosamente del producto {productId}",
                    product
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
                    "Error al eliminar el color {ColorId} del producto {ProductId}",
                    colorId,
                    productId);

                return StatusCode(500, new
                {
                    message = "Error interno del servidor al eliminar el color"
                });
            }
        }


        //change product subcategory
        // Controllers/ProductsController.cs
        [HttpPut("{productId}/subcategory")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> ChangeProductSubcategory(
            int productId,
            [FromBody] ChangeSubcategoryDto changeSubcategoryDto)
        {
            try{
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _productService.ChangeProductSubcategoryAsync(
                    productId,
                    changeSubcategoryDto.NewSubcategoryId);

                if (productId <= 0)
                {
                    return BadRequest(new { message = "El ID del producto debe ser mayor que 0" });
                }

                return Ok(new
                {
                    message = $"Producto movido exitosamente de la subcategoría '{result.ChangeInfo.OldSubcategory}' " +
                             $"({result.ChangeInfo.OldCategory}) a '{result.ChangeInfo.NewSubcategory}' " +
                             $"({result.ChangeInfo.NewCategory})",
                    product = result.Product
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
                    "Error al cambiar la subcategoría del producto {ProductId}",
                    productId);

                return StatusCode(500, new
                {
                    message = "Error interno del servidor al cambiar la subcategoría del producto"
                });
            }
        }
    }
}
