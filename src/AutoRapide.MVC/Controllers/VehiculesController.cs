using AutoRapide.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.MVC.Controllers;

public class VehiculesController : Controller
{
    private readonly IVehiculesService _vehiculesService;

    public VehiculesController(IVehiculesService vehiculesService)
    {
        _vehiculesService = vehiculesService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var vehicules = await _vehiculesService.ObtenirToutAsync();
        return View(vehicules);
    }
}