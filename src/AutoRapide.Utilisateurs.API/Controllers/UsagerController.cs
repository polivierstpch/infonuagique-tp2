using AutoRapide.Utilisateurs.API.Entities;
using AutoRapide.Utilisateurs.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.Utilisateurs.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsagerController : ControllerBase
    {
        private readonly IUsagerService _crudService;

        public UsagerController(IUsagerService crudService)
        {
            _crudService = crudService;
        }

        /// <summary>
        /// Premet l'obtention et le retour d'une liste de tous les usagers de la Bibliothèque Lipajoli
        /// </summary>
        /// <remarks>Pas de remarques</remarks>  
        /// <response code="200">Liste complète des usagers de la bibliothèque Lipajoli trouvée et retournée</response>
        /// <response code="404">Liste complète des usagers de la bibliothèque Lipajoli introuvable</response>
        /// <response code="500">Oups! Le service demandé est indisponible pour le moment</response>
        // GET: api/Usagers
        [HttpGet]
        public async Task<IEnumerable<Usager>> Get()
        {
            return await _crudService.ObtenirTousLesUsagers();
        }

        /// <summary>
        /// Premet l'obtention et le retour des informations d'un usager spécifique, ciblé par l'id passé en paramètre
        /// </summary>
        /// <param name="id">id de l'usager à retourner</param>
        /// <returns></returns>
        /// <remarks>Pas de remarques</remarks>  
        /// <response code="200">L'usager spécifié a été trouvé et retourné</response>
        /// <response code="404">Usager introuvable pour l'id specifié</response>
        /// <response code="500">Oups! Le service demandé est indisponible pour le moment</response>
        [HttpGet("{id:int}")]
        public async Task<Usager> Get(int id)
        { 
            return await _crudService.ObtenirUsagerParId(id);
        }

        [HttpPost]
        public async Task<ActionResult<Usager>> Post([FromBody] Usager usager)
        {
            await _crudService.AjouterUsager(usager);

            return CreatedAtAction(nameof(Get), new { id = usager.Id }, usager);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] Usager usager)
        {
            await _crudService.ModifierUsager(usager);

            return new OkObjectResult(new { Message = $"L'usager avec l'id {usager.Id} a été modifié avec succès."});
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _crudService.EffacerUsager(id);

            return new OkObjectResult(new { Message = $"L'usager avec l'id {id} a été supprimé avec succès." });
        }
    }
}
