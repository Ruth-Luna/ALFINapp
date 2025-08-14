using ALFINapp.API.Filters;
using ALFINapp.Datos.DAO.Asesores;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class VendedorController : Controller
    {
        private readonly MDbContext _context;
        private readonly DAO_AsesorInicioView _dao_asesorInicioView;
        public VendedorController(
            MDbContext context,
            DAO_AsesorInicioView dao_asesorInicioView)
        {
            _context = context;
            _dao_asesorInicioView = dao_asesorInicioView;
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

            var inicio = await _dao_asesorInicioView.Execute(usuarioId.Value);
            if (!inicio.IsSuccess || inicio.Data == null)
            {
                TempData["MessageError"] = inicio.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            var dataInicio = inicio.Data;
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