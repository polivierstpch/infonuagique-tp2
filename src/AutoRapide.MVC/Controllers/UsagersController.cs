using AutoRapide.MVC.Models;
using AutoRapide.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.DataProtection;
using System.Text.RegularExpressions;

namespace AutoRapide.MVC.Controllers
{
    public class UsagersController : Controller
    {
        private const string _MSG_USAGER_INEXISTANT = "L'utilisateur spécifié est introuvable.";
        private const string _MSG_USAGER_SUPPRESSION_IMPOSSIBLE = "Une commande à été effectuée par cet usager. Celui-ci ne peux pas être supprimé";
        
        private readonly IUsagerService _usagersProxy;
        //private readonly ICommandesService _commandesProxy;
        //private readonly IFavorisService _favorisProxy;

        public UsagersController(IUsagerService usagerProxy /*, ICommandesService commandesProxy, IFavorisService _favorisProxy*/)
        {
            _usagersProxy = usagerProxy;
            //_commandesProxy = commandesProxy;
        }

        public async Task<ActionResult> Index()
        {
            var utilisateurReponse = await _usagersProxy.ObtenirTousLesUsagers();

            List<Usager> utilisateurs = utilisateurReponse.ToList();

            return View(utilisateurs);

        }

        public async Task<ActionResult> Details(string code)
        {

            if (code == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            var utilisateurReponse = await _usagersProxy.ObtenirUsagerParCodeUsager(code);

            if (utilisateurReponse == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            return View(utilisateurReponse);
        }

        // GET: Usagers/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        { 
            return View();
        }

        // POST: Usagers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nom, Prenom, Email, Adresse")] Usager usager)
        {
            if (InputValidation(usager))
            {
                if (ModelState.IsValid)
                {
                    var response = await _usagersProxy.AjouterUsager(usager);
                    var content = response.Content.ReadAsStringAsync();
                    var usagerCree = JsonConvert.DeserializeObject<Usager>(content.Result);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(usager);
        }

        // GET: Usagers/Edit
        [HttpGet]
        public async Task<ActionResult> ModifierUsager(string code)
        {
            if (code == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            var utilisateurReponse = await _usagersProxy.ObtenirUsagerParCodeUsager(code);

            if (utilisateurReponse == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            return View(utilisateurReponse);
        }

        //POST: Usagers/Edit
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ModifierUsager(string code, Usager usager)
        {
            if (code == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            var response = await _usagersProxy.ObtenirUsagerParCodeUsager(code);

            if (response == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            if (InputValidation(usager))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _usagersProxy.ModifierUsager(usager);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Une erreur est survenue lors de l'enregistrement des modifications.");
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            return View(usager);
        }


        private bool InputValidation(Usager usager)
        {
            string formatNom = @"([- ]{1}[\p{Lu}\p{P}]{1}[ \.]{1}){0,1}([- ]{0,1}([\p{Lu}']{1,3}[\p{Ll}]+))*$";
            string formatAdresse = @"^[-0-9]+ [-\.#\w ]+";
            string formatCourriel = @"^([a-zA-Z0-9]+)(([-_\.]){1}[a-zA-Z0-9]+)*@([a-z0-9\-]+\.)+[a-z]{2,}$";

            if (!Regex.IsMatch(usager.Prenom, formatNom) || !Regex.IsMatch(usager.Nom, formatNom))
            {
                ModelState.AddModelError("", "Le nom et le prénom doivent débuter par une lettre majuscule et n'être compris que de lettres.");
                return false;
            }

            if(usager.Adresse != null)
            {
                if (!Regex.IsMatch(usager.Adresse, formatAdresse))
                {
                    ModelState.AddModelError("Adresse", "Votre adresse doit débuter avec un numéro civique.");
                    return false;
                }
            }


            if (!Regex.IsMatch(usager.Email, formatCourriel))
            {
                ModelState.AddModelError("AdresseCourriel", "L'adresse courriel doit être valide. Par exemple: mon_adresse@courriel.com");
                return false;
            }

            return true;
        }

        public async Task<ActionResult> Supprimer(string code)
        {
            if (code == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            var response = await _usagersProxy.ObtenirUsagerParCodeUsager(code);

            if (response == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            //var commandesResponse = await _commandesProxy.ObtenirToutesLesCommandes();
            //List<Commande> allCommandes = commandesResponse.ToString();

            //if (allCommandes.Any(c => c.IdUtilisateur == response.Id))
            //{
            //    ViewBag.MessageErreur = _MSG_USAGER_SUPPRESSION_IMPOSSIBLE;
            //    return View("Error");
            //}

            return View(response);
        }

        [HttpPost, ActionName("Supprimer")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SupprimerUtilisateur(string code, Usager utilisateur)
        {
            if (code == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            var response = await _usagersProxy.ObtenirUsagerParCodeUsager(code);

            if (response == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            var responseSuppression = await _usagersProxy.EffacerUsager(response.CodeUniqueUsager);

            return RedirectToAction(nameof(Index));

        }

    }
}
