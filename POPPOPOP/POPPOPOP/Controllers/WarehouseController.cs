using Microsoft.AspNetCore.Mvc;
using POPPOPOP.Repositories;

namespace POPPOPOP.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IDbrepository _warehouseRepository;
    public WarehouseController(IDbrepository animalsRepository)
    {
        _warehouseRepository = animalsRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        return Ok();
    }
    
}