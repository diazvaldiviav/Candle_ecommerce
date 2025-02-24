using Candle_API.Data.DTOs.Aromas;
using Candle_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Candle_API.Controllers
{
    // Controllers/AromaController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class AromaController : ControllerBase
    {
        private readonly IAromas _aromaService;

        public AromaController(IAromas aromaService)
        {
            _aromaService = aromaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AromaDTO>>> GetAllAromas()
        {
            var aromas = await _aromaService.GetAllAromasAsync();
            return Ok(aromas);
        }

        [HttpPost]
        public async Task<ActionResult<AromaDTO>> CreateAroma(CreateAromaDTO dto)
        {
            var aroma = await _aromaService.CreateAromaAsync(dto);
            return CreatedAtAction(nameof(GetAllAromas), new { id = aroma.Id }, aroma);
        }

        [HttpPost("{productId}/aromas")]
        public async Task<ActionResult<ProductAromaDTO>> AddAromaToProduct(
            int productId,
            AddAromaToProductDTO dto)
        {
            var productAroma = await _aromaService.AddAromaToProductAsync(productId, dto);
            return Ok(productAroma);
        }
    }
}
