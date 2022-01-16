using AutoRapide.MVC.Interfaces;
using AutoRapide.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.MVC.Controllers;

public class VehiculesController : Controller
{
    private readonly IVehiculesService _vehiculesService;
    private readonly IFichiersService _fichiersService;
    private readonly IConfiguration _config;
    private readonly IFavorisService _favorisProxy;

    public VehiculesController(IVehiculesService vehiculesService, IFichiersService fichiersService, IConfiguration config, IFavorisService favProxy)
    {
        _vehiculesService = vehiculesService;
        _fichiersService = fichiersService;
        _config = config;
        _favorisProxy = favProxy;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var vehicules = await _vehiculesService.ObtenirToutAsync();
        return View(vehicules);
    }

    public async Task<IActionResult> Details(int id)
    {
        var vehicule = await _vehiculesService.ObtenirParIdAsync(id);

        if (vehicule is null)
            return NotFound();
        var favoris = await _favorisProxy.ObtenirLesFavoris();
        ViewBag.IsFavori = favoris.Contains(id);
        ViewBag.IdVehicule = id;
        return View(vehicule);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Constructeur", "Modele", "AnneeFabrication", "Type", "NombreSiege", "Couleur", "Description", "Prix")]
        Vehicule vehicule,
        IFormFile[] fichiers
    )
    {
        if (!ModelState.IsValid)
            return View(vehicule);

        if (fichiers is null || fichiers.Length != 2)
        {
            ViewBag.Erreur = "Il faut fournir deux images de format jpeg/jpg ou png.";
            return View(vehicule);
        }

        vehicule.NIV = ConstruireNIV(vehicule);

        var urlsFichiers = (await _fichiersService.EnvoyerFichiers(vehicule.NIV, fichiers)).ToList();
        
        if (urlsFichiers.Count != 2)
        {
            throw new HttpRequestException("Une erreur est survenue lors de l'envoi des fichiers.");
        }

        var fichierApiUrl = _config.GetValue<string>("UrlFichiersAPI") + "/api/fichiers/";
        vehicule.Image1Url = fichierApiUrl + urlsFichiers[0];
        vehicule.Image2Url = fichierApiUrl + urlsFichiers[1];

        vehicule.EstDisponible = true;

        var reponse = await _vehiculesService.AjouterAsync(vehicule);

        reponse.EnsureSuccessStatusCode();

        return RedirectToAction(nameof(Index));
    }
    
    private string ConstruireNIV(Vehicule vehicule)
    {
        var rng = new Random(Guid.NewGuid().GetHashCode());
        const int maximumNumero = 1000000;
        
        string codeConstructeur = vehicule.Constructeur.Trim()[..3];
        string codeSiege = vehicule.NombreSiege.ToString("D2");
        string typeVehicule = ((int)vehicule.Type).ToString("D2");
        
        string anneeRaccourcie = vehicule.AnneeFabrication.ToString()[2..];
        string codeAnneeModel = $"{vehicule.Modele.Trim().Replace("-", "")[..2]}{anneeRaccourcie}";
        string numero = rng.Next(0, maximumNumero).ToString("D6");
        
        return $"{codeConstructeur}{codeSiege}{typeVehicule}{codeAnneeModel}{numero}".ToUpper();
    }
}