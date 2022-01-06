using AutoRapide.Vehicules.API.Entities;
using AutoRapide.Vehicules.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.Vehicules.API.Controllers;

[Route("api/vehicules")]
[ApiController]
public class GestionVehiculeController : ControllerBase
{
    private readonly IVehiculeService _vehiculeService;
    private readonly ILogger<GestionVehiculeController> _logger;
    
    public GestionVehiculeController(IVehiculeService vehiculeService, ILogger<GestionVehiculeController> logger)
    {
        _vehiculeService = vehiculeService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehicule>>> Get()
    {
        var vehicules = await _vehiculeService.ObtenirListe();
        return Ok(vehicules);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<Vehicule>> Get(int id)
    {
        var vehicule = await _vehiculeService.TrouverParIdAsync(id);

        if (vehicule is null)
            return NotFound();

        return Ok(vehicule);
    }

    [HttpPost]
    [Route("enregistrer")]
    public async Task<IActionResult> Post([FromBody] Vehicule vehicule)
    {
        await _vehiculeService.EnregistrerAsync(vehicule);
        return CreatedAtAction(nameof(Get), new {id = vehicule.Id}, vehicule);
    }

    [HttpDelete]
    [Route("supprimer/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var vehiculeASupprimer = await _vehiculeService.TrouverParIdAsync(id);

        if (vehiculeASupprimer is null)
            return NotFound();

        await _vehiculeService.SupprimerAsync(vehiculeASupprimer);

        return NoContent();
    }

    [HttpPut]
    [Route("modifier/{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] Vehicule vehicule)
    {
        if (id != vehicule.Id)
            return BadRequest();

        var vehiculeExistant = await _vehiculeService.TrouverParIdAsync(id);

        if (vehiculeExistant is null)
            return NotFound();

        await _vehiculeService.ModifierAsync(vehicule);

        return NoContent();
    }
}