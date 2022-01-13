using AutoRapide.MVC.Models;
using AutoRapide.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AutoRapide.MVC.Controllers
{
    public class UsagersController : Controller
    {
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
            if (ModelState.IsValid)
            {
                var response = await _usagersProxy.AjouterUsager(usager);
                var content = response.Content.ReadAsStringAsync();
                var usagerCree = JsonConvert.DeserializeObject<Usager>(content.Result);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //TODO : Modification
    }
}
