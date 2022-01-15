using AutoRapide.MVC.Models;
using AutoRapide.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.MVC.Controllers;

[Route("commander")]
public class CommandesController : Controller
{
    private readonly ICommandesService _commandesService;
    private readonly IVehiculesService _vehiculesService;
    private readonly IUsagerService _usagerService;
    private readonly ILogger<CommandesController> _logger;

    public CommandesController(
        ICommandesService commandes,
        IVehiculesService vehiculesService,
        ILogger<CommandesController> logger,
        IUsagerService usagerService
    )
    {
        _commandesService = commandes;
        _vehiculesService = vehiculesService;
        _logger = logger;
        _usagerService = usagerService;
    }
    
    public async Task<IActionResult> Index(int vehiculeId)
    {
        var vehicule = await _vehiculesService.ObtenirParIdAsync(vehiculeId);

        if (vehicule is null)
        {
            return NotFound();
        }

        return View(vehicule);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Vehicule vehicule, string codeUsager)
    {
        if (!vehicule.EstDisponible)
        {
            ModelState.AddModelError("Vehicule", "Le véhicule n'est pas disponible.");
            return View(vehicule);
        }
        
        if (string.IsNullOrEmpty(codeUsager))
        {
            ModelState.AddModelError("CodeUsager", "Le code usager ne doit pas être vide.");
            return View(vehicule);
        }

        try
        {
            var codeUsagerGuid = Guid.Parse(codeUsager);

            var usager = await _usagerService.ObtenirUsagerParCodeUsager(codeUsager);
            if (usager is null)
            {
                ModelState.AddModelError("CodeUsager", "Le code usager était invalide.");
                return View(vehicule);
            }

            await _commandesService.AjouterAsync(new Commande {UsagerId = usager.Id, VehiculeId = vehicule.Id});
            vehicule.EstDisponible = false;
            var result = await _vehiculesService.ModifierAsync(vehicule);

            if (result.IsSuccessStatusCode)
                return RedirectToAction("Index", "Vehicules", new { id = vehicule.Id });

            ViewBag.Erreur = "Une erreur est survenue lors de la prise de la commande.";
        
            vehicule.EstDisponible = true;
            return View(vehicule);
        }
        catch (FormatException)
        {
            ModelState.AddModelError("CodeUsager", "Le code usager était invalide.");
            return View(vehicule);
        }
    }
    

}