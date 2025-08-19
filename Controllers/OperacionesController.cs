using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.Controllers
{
    public class OperacionesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAllDerivaciones()
        {
            
            return Json(new { success = true, data = new List<string> { "Derivacion1", "Derivacion2" } });
        }
    }
}
