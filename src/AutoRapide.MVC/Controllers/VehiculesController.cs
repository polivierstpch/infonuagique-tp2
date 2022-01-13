using Microsoft.AspNetCore.Mvc;

namespace AutoRapide.MVC.Controllers
{
    public class VehiculesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    }
}
