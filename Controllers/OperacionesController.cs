using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.Controllers
{
    public class OperacionesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
