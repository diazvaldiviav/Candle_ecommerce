using Candle_API.Data.DTOs.Product;
using Microsoft.AspNetCore.Mvc;
using Candle_API.Services.Interfaces;
using Candle_API.Tools;


/// <summary>
/// Controlador para la gestión de productos
/// </summary>

namespace Candle_API.Controllers
{
    
    // Controllers/ProductsController.cs
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Tags("Productos")]
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


        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Información detallada del producto</returns>
        /// <response code="200">Retorna el producto solicitado</response>
        /// <response code="404">Si el producto no existe</response>
        /// <response code="500">Error interno del servidor</response>

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


        //crear nuevo size para el producto
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

        /// <summary>
        /// Agrega un nuevo color a un producto existente
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="colorDto">Información del color a agregar</param>
        /// <returns>Producto actualizado con el nuevo color</returns>
        /// <remarks>
        /// Ejemplo de request:
        /// 
        ///     POST /api/products/1/color
        ///     {
        ///         "name": "Rojo",
        ///         "hexCode": "#FF0000"
        ///     }
        /// </remarks>

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

        /// <summary>
        ///You can change the category in the product that already exists
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="subcategoryId">Subcategory ID</param>
        /// <returns>Producto actualizado con el nuevo color</returns>
        /// <remarks>
        /// Ejemplo de request:
        /// 
        ///     PUT /api/{id}/subcategory
        ///     {
        ///         "ProductId": 1,
        ///         "SubcategoryId": 1
        ///     }
        /// </remarks>
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


       
        /// <summary>
        /// Asocia un color existente a un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="associateColorDto">Información del color a asociar</param>
        /// <returns>Detalles de la asociación creada</returns>
        /// <response code="200">Asociación creada exitosamente</response>
        /// <response code="400">Si ya existe la asociación o datos inválidos</response>
        /// <response code="404">Si el producto o el color no existen</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("{id}/colors")]
        [ProducesResponseType(typeof(ProductColorAssociationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductColorAssociationDto>> AssociateColor(
            int id,
            [FromBody] AssociateColorDto associateColorDto)
        {
            try
            {
                var result = await _productService.AssociateColorAsync(id, associateColorDto);
                _logger.LogInformation(
                    "Color {ColorId} asociado exitosamente al producto {ProductId}",
                    associateColorDto.ColorId,
                    id);
                return Ok(result);
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
                _logger.LogError(
                    ex,
                    "Error al asociar color {ColorId} al producto {ProductId}",
                    associateColorDto.ColorId,
                    id);
                return StatusCode(500, new
                {
                    message = "Error interno del servidor al asociar el color al producto"
                });
            }
        }


        // Controllers/ProductsController.cs
        /// <summary>
        /// Asocia un tamaño existente a un producto
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <param name="associateSizeDto">Información del tamaño a asociar</param>
        /// <returns>Detalles de la asociación creada</returns>
        /// <response code="200">Asociación creada exitosamente</response>
        /// <response code="400">Si ya existe la asociación o datos inválidos</response>
        /// <response code="404">Si el producto o el tamaño no existen</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("{id}/sizes")]
        [ProducesResponseType(typeof(ProductSizeAssociationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductSizeAssociationDto>> AssociateSize(
            int id,
            [FromBody] AssociateSizeDto associateSizeDto)
        {
            try
            {
                var result = await _productService.AssociateSizeAsync(id, associateSizeDto);
                _logger.LogInformation(
                    "Tamaño {SizeId} asociado exitosamente al producto {ProductId}",
                    associateSizeDto.SizeId,
                    id);
                return Ok(result);
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
                _logger.LogError(
                    ex,
                    "Error al asociar tamaño {SizeId} al producto {ProductId}",
                    associateSizeDto.SizeId,
                    id);
                return StatusCode(500, new
                {
                    message = "Error interno del servidor al asociar el tamaño al producto"
                });
            }
        }


        [HttpPost("{id}/images")]
        public async Task<ActionResult<ProductResponseDTO>> AddProductImages(int id, [FromBody] AddProductImagesDTO imagesDto)
        {
            try
            {
                if (id != imagesDto.ProductId)
                {
                    return BadRequest("El ID del producto no coincide");
                }

                var result = await _productService.AddProductImagesAsync(imagesDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }



        [HttpPatch("{productId}/images/{imageId}/main")] 
        public async Task<IActionResult> SetMainImage(int productId, int imageId)
        {
            try
            {
                var updateDto = new UpdateMainImageDTO
                {
                    ProductId = productId,
                    ImageId = imageId
                };

                var result = await _productService.SetMainImageAsync(updateDto);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(int productId, int imageId)
        {
            try
            {
                // Validar que no se pueda eliminar si es la única imagen
                await _productService.DeleteProductImageAsync(productId, imageId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
