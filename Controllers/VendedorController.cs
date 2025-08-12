using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Tipificacion;
using ALFINapp.Application.Interfaces.Vendedor;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class VendedorController : Controller
    {
        private readonly MDbContext _context;
        private readonly IUseCaseGetInicio _useCaseGetInicio;
        public VendedorController(
            MDbContext context,
            IUseCaseGetInicio useCaseGetInicio
            )
        {
            _context = context;
            _useCaseGetInicio = useCaseGetInicio;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario(Usuario model)
        {
            if (string.IsNullOrWhiteSpace(model.Dni) || !long.TryParse(model.Dni, out _))
            {
                ModelState.AddModelError("dni", "El DNI debe contener solo números.");
            }
            if (ModelState.IsValid)
            {
                var usuarioExistente = await _context.usuarios
                    .FirstOrDefaultAsync(usuario => usuario.Dni == model.Dni);
                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("dni", "El DNI ya está registrado.");
                    return View(model); // Devuelve el modelo con errores
                }
                model.NombresCompletos = model.NombresCompletos;
                _context.usuarios.Add(model);
                TempData["Message"] = "Usuario agregado con exito";
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }

        // Acción para mostrar la página de Inicio
        [HttpGet]
        [PermissionAuthorization("Vendedor", "Inicio")]
        public async Task<IActionResult> Inicio()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            var executeInicio = await _useCaseGetInicio.Execute(usuarioId.Value);
            if (!executeInicio.IsSuccess || executeInicio.Data == null)
            {
                TempData["MessageError"] = executeInicio.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            var dataInicio = executeInicio.Data;
            return View("Main", dataInicio);
        }

        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AgregarTelefonoView(int idCliente)
        {
            try
            {
                TempData["idCliente"] = idCliente;
                return PartialView("_AgregarTelefono");
            }
            catch (System.Exception)
            {
                return Json(new { error = true });
                throw;
            }
        }
    }
}