
using AutoRapide.Commandes.API.Entities;
using AutoRapide.Commandes.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.Commandes.API.Controllers;

[Route("api/commandes")]
[ApiController]
public class GestionCommandesController : ControllerBase
{
    private readonly ICommandeService _commandeService;
    private readonly ILogger<GestionCommandesController> _logger;

    public GestionCommandesController(ICommandeService commandeService, ILogger<GestionCommandesController> logger)
    {
        _commandeService = commandeService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Commande>>> ObtenirTout()
    {
         var commandes = await _commandeService.ObtenirTout();
         _logger.LogInformation(CustomLogEvents.Lecture, "Obtention de {Compte} commande(s)", commandes.Count());
         
         return Ok(commandes);
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<ActionResult<Commande>> ObtenirUne(int id)
    {
        var commande = await _commandeService.ObtenirParId(id);
        _logger.LogInformation(CustomLogEvents.Lecture ,"Obtention de la commande avec l'identifiant {Id}", id);

        if (commande is null)
        {
            _logger.LogInformation(CustomLogEvents.Lecture, "La commande avec l'identifiant {Id} n'a pas été trouvée", id);
            return NotFound();
        }
        
        _logger.LogInformation(CustomLogEvents.Lecture, "La commande avec l'identifiant {Id} va être envoyée", id);
        return Ok(commande);
    }

    [Route("enregistrer")]
    [HttpPost]
    public async Task<IActionResult> Enregistrer(Commande commande)
    {
        bool existe = await _commandeService.CommandeAvecVehiculeExiste(commande.VehiculeId);
        _logger.LogInformation(CustomLogEvents.Creation,
            "Vérification de l'existance d'une commande avec l'identifiant de véhicule {IdVehicule}", commande.VehiculeId);

        if (existe)
        {
            _logger.LogInformation(CustomLogEvents.Creation, 
                "Une commande est déjà existante pour le véhicule (id: {Id})", commande.VehiculeId);
            return BadRequest($"Une commande pour le véhicule (id: {commande.VehiculeId}) existe déjà dans la base de données.");
        }

        if (commande.Date < DateTime.Now)
        {
            _logger.LogInformation(CustomLogEvents.Creation, 
                "La date ({Date:yy-MM-dd}) n'est pas une date valide", commande.Date);
            return BadRequest("Veuillez enregistrer une commande avec la date actuelle d'aujourd'hui.");
        }

        await _commandeService.Enregistrer(commande);
        _logger.LogInformation(CustomLogEvents.Creation,
            "Commande enregistrée avec l'identifiant {Id} dans la base de données", commande.Id);
        
        return CreatedAtAction(nameof(ObtenirUne), new {id = commande.Id}, commande);
    }

    [Route("modifier/{id:int}")]
    [HttpPut]
    public async Task<IActionResult> Modifier(int id, Commande commande)
    {
        if (id != commande.Id)
        {
            _logger.LogInformation(CustomLogEvents.Modification,
                "L'identifiant fourni ({Id}) ne correspond pas à la commande de la requête (id: {IdCommande})", 
                id, commande.Id);
            return BadRequest($"L'identifiant fourni ({id}) ne correspond pas à la commande passée (id: {commande.Id}) dans le corps de la requête.");
        }

        var ancienneCommande = await _commandeService.ObtenirParId(id);
        _logger.LogInformation(CustomLogEvents.Modification, 
            "Obtention de la commande avec l'identifiant {Id} pour vérification avant modification", id);

        if (ancienneCommande is null)
        {
            _logger.LogInformation(CustomLogEvents.Modification, 
                "La commande avec l'identifiant {Id} n'existe pas dans la base de données", id);
            return NotFound();
        }

        if (ancienneCommande.VehiculeId != commande.VehiculeId)
        {
            bool existe = await _commandeService.CommandeAvecVehiculeExiste(commande.VehiculeId);
            _logger.LogInformation(CustomLogEvents.Modification,
                "Vérification de l'existance d'une commande avec l'identifiant de véhicule {IdVehicule}", commande.VehiculeId);
            
            if (existe)
            {
                _logger.LogInformation(CustomLogEvents.Modification, 
                    "Une commande est déjà existante pour le véhicule (id: {Id})", commande.VehiculeId);
                return BadRequest($"Une commande pour le véhicule (id: {commande.VehiculeId}) existe déjà dans la base de données.");
            }
        }

        if (commande.Date != ancienneCommande.Date)
        {
            _logger.LogInformation(CustomLogEvents.Modification, 
                "La date de la commande modifiée ({Date:yy-MM-dd} n'est pas pareille à celle de la commande originale {DateOrig:yy-MM-dd}",
                commande.Date, ancienneCommande.Date);
            return BadRequest("La date d'une commande ne peut pas être modifiée.");
        }

        await _commandeService.Modifier(commande);
        _logger.LogInformation(CustomLogEvents.Modification, "Modification de la commande avec l'identifiant {Id}", id);

        return NoContent();
    }

    [Route("supprimer/{id:int}")]
    [HttpDelete]
    public async Task<IActionResult> Supprimer(int id)
    {
        var commandeExistante = await _commandeService.ObtenirParId(id);
        _logger.LogInformation(CustomLogEvents.Lecture,
            "Obtention de la commande avec l'identifiant {Id} pour vérification avant suppression", id);

        if (commandeExistante is null)
        {
            _logger.LogInformation(CustomLogEvents.Suppression, 
                "La commande avec l'identifiant {Id} n'existe pas dans la base de données", id);
            return NotFound();
        }

        await _commandeService.Supprimer(commandeExistante);
        _logger.LogInformation(CustomLogEvents.Suppression, "Suppression de la commande avec l'identifiant {Id}", id);
        
        return NoContent();
    }
}