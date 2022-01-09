using AutoRapide.Favoris.API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.Favoris.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavorisController : ControllerBase
    {
        private readonly IFavorisService _crudService;
        private readonly ILogger<FavorisController> _logger;

        public FavorisController(IFavorisService crudService, ILogger<FavorisController> logger)
        {
            _crudService = crudService;
            _logger = logger;
        }

        /// <summary>
        /// Premet l'obtention et le retour d'une liste de tous les usagers du site AutoRapide
        /// </summary>
        /// <remarks>Pas de remarques</remarks>  
        /// <response code="200">Liste complète des usagers de la bibliothèque Lipajoli trouvée et retournée</response>
        /// <response code="400">Liste complète des usagers de la bibliothèque Lipajoli introuvable</response>
        // GET: api/Usagers
        [HttpGet]
        public ActionResult<IEnumerable<int>> Get()
        {

            var favoris = _crudService.ObtenirLesFavoris();
            _logger.LogInformation(CustomLogEvents.Lecture, $"Obtention de {favoris.ToList().Count} favoris en cache.");

            if (favoris == null)
            {
                _logger.LogError(CustomLogEvents.Lecture, $"Échec d'obtention des usagers.");
                return BadRequest();
            }
            else
            {
                return Ok(favoris);
            }
        }

        /// <summary>
        /// Premet la création d'un nouvel usager du site AutoRapide
        /// </summary>
        /// <remarks>Pas de remarques</remarks>  
        /// <response code="201">L'usager a été créé avec succès!</response>
        /// <response code="400">L'usager n'a pas pu être enregistré. Veuillez vérifier la validité des informations.</response>
        [HttpPost]
        public ActionResult<int> Post(int idVehicule)
        {
            try
            {
                _crudService.AjouterFavori(idVehicule);
                _logger.LogInformation(CustomLogEvents.Creation, $"Ajout du véhicule avec l'ID: {idVehicule} aux favoris.");
                return new OkObjectResult(new { Message = $"Le véhicule avec l'id {idVehicule} a été ajouté aux favoris avec succès." });
            }
            catch (InvalidDataException ex)
            {
                _logger.LogError(CustomLogEvents.Creation, $"Échec de la création d'un nouvel usager: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Premet la création d'un nouvel usager du site AutoRapide
        /// </summary>
        /// <remarks>Pas de remarques</remarks>  
        /// <response code="201">L'usager a été créé avec succès!</response>
        /// <response code="400">L'usager à supprimer est inexistant.</response>
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int idVehicule)
        {
            try
            {
                _crudService.EffacerFavori(idVehicule);
                _logger.LogInformation(CustomLogEvents.Suppression, $"Supression du véhicule avec l'ID: {idVehicule} aux favoris.");
                return new OkObjectResult(new { Message = $"Le véhicule avec l'id {idVehicule} a été retiré aux favoris avec succès." });
            }
            catch (InvalidDataException ex)
            {
                _logger.LogError(CustomLogEvents.Modication, $"Échec de la suppression de l'usager avec l'ID: {idVehicule}");
                return BadRequest(ex.Message);
            }
        }
    }
}
