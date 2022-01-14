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
        
        public UsagersController(IUsagerService usagerProxy)
        {
            _usagersProxy = usagerProxy;
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
                    usager.CodeUniqueUsager = GenererCodeUniqueUsager(usager);
                    var response = await _usagersProxy.AjouterUsager(usager);
                    var content = response.Content.ReadAsStringAsync();
                    var usagerCree = JsonConvert.DeserializeObject<Usager>(content.Result);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        // GET: Usagers/Edit
        [HttpGet]
        public async Task<ActionResult> ModifierUsager(int? id)
        {
            if (id == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            var utilisateurReponse = await _usagersProxy.ObtenirUsagerParId(id.Value);

            if (utilisateurReponse == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            return View(utilisateurReponse);
        }

        //POST: Usagers/Edit
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ModifierUsager(int? id, Usager usager)
        {
            if (id == null)
            {
                ViewBag.MessageErreur = _MSG_USAGER_INEXISTANT;
                return View("Error");
            }

            var response = await _usagersProxy.ObtenirUsagerParId(id.Value);

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

        private string GenererCodeUniqueUsager(Usager usager)
        {
            int userNumber = 0;
            string codeUnique = usager.Nom.Substring(0,3).ToUpper() + usager.Prenom.Substring(0,1) + DateTime.Now.ToString("ddMMyyyy") + userNumber.ToString("000000");
            //Ajouter boucle pour comparer les codes existant pour valider unicité? Requête trop intense?
            userNumber += 1;
            
            return codeUnique;
        }

    }
}
