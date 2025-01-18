// Controllers/TestController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly CandleDbContext _context;

    public TestController(CandleDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            // Intenta conectar a la base de datos
            bool canConnect = await _context.Database.CanConnectAsync();
            return Ok(new { message = "Conexión exitosa", canConnect });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error de conexión", error = ex.Message });
        }
    }
}