
using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.Commandes.API.Controllers;

[Route("api/commandes")]
[ApiController]
public class GestionCommandesController : ControllerBase
{
    // GET
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
}