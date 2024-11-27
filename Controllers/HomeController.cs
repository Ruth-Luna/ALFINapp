using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.Models;

namespace ALFINapp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Buscar(string dni)
    {
        if (string.IsNullOrEmpty(dni))
        {
            return RedirectToAction("Index");
        }
        else
        {
            return View("Main");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult PersonalUser()
    {
        return View("Modifyinguser");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Addingclient()
    {
        return View("Addingclient");
    }
}
