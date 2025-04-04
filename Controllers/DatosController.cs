using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using ALFINapp.API.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class DatosController : Controller
    {
        private readonly MDbContext _context;

        public DatosController(MDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult EnviarComentario(string Telefono, int IdCliente, string Comentario)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Lógica para agregar o actualizar el comentario en la base de datos
                var registro = _context.telefonos_agregados.FirstOrDefault(ta => ta.Telefono == Telefono && ta.IdCliente == IdCliente);
                if (string.IsNullOrEmpty(Telefono) || string.IsNullOrEmpty(Comentario))
                {
                    return Json(new { success = false, message = "Los Campos enviados estan Vacios." });
                }

                if (registro != null)
                {
                    registro.Comentario = Comentario;
                    _context.SaveChanges();
                }

                else
                {
                    return Json(new { success = false, message = "Número no encontrado." });
                }

                return Json(new { success = true, message = "Comentario guardado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult EnviarComentarioTelefonoDB(string Telefono, int IdCliente, string Comentario, int numeroTelefono)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (string.IsNullOrEmpty(Telefono) || string.IsNullOrEmpty(Comentario))
                {
                    return Json(new { success = false, message = "Datos inválidos." });
                }

                var clienteEnriquecido = _context.clientes_enriquecidos.FirstOrDefault(ce => ce.IdCliente == IdCliente);

                if (clienteEnriquecido == null)
                {
                    return Json(new { success = false, message = "Cliente no encontrado." });
                }

                switch (numeroTelefono)
                {
                    case 1:
                        if (clienteEnriquecido.Telefono1 == Telefono)
                        {
                            clienteEnriquecido.ComentarioTelefono1 = Comentario;
                        }
                        break;
                    case 2:
                        if (clienteEnriquecido.Telefono2 == Telefono)
                        {
                            clienteEnriquecido.ComentarioTelefono2 = Comentario;
                        }
                        break;
                    case 3:
                        if (clienteEnriquecido.Telefono3 == Telefono)
                        {
                            clienteEnriquecido.ComentarioTelefono3 = Comentario;
                        }
                        break;
                    case 4:
                        if (clienteEnriquecido.Telefono4 == Telefono)
                        {
                            clienteEnriquecido.ComentarioTelefono4 = Comentario;
                        }
                        break;
                    case 5:
                        if (clienteEnriquecido.Telefono5 == Telefono)
                        {
                            clienteEnriquecido.ComentarioTelefono5 = Comentario;
                        }
                        break;
                    default:
                        return Json(new { success = false, message = "Número de teléfono inválido." });
                }

                _context.SaveChanges();
                return Json(new { success = true, message = "Comentario guardado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult EnviarComentarioGeneral(int idAsignacion, string comentarioGeneral)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var modificarComentarioGeneral = _context.clientes_asignados.FirstOrDefault(ca => ca.IdAsignacion == idAsignacion);
                if (modificarComentarioGeneral != null)
                {
                    modificarComentarioGeneral.ComentarioGeneral = comentarioGeneral;
                    _context.SaveChanges();
                }
                else
                {
                    return Json(new { success = false, message = "Número no encontrado." });
                }

                return Json(new { success = true, message = "Comentario guardado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}