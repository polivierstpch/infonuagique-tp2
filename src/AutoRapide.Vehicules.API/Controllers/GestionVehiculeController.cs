using AutoRapide.Vehicules.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.Vehicules.API.Controllers;

public class GestionVehiculeController : ControllerBase
{
    private readonly IVehiculeService _vehiculeService;
    private readonly ILogger<GestionVehiculeController> _logger;

    public GestionVehiculeController(IVehiculeService vehiculeService, ILogger<GestionVehiculeController> logger)
    {
        _vehiculeService = vehiculeService;
        _logger = logger;
    }
    // GET
    public IActionResult Index()
    {
        return NotFound();
    }
}