using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using ALFINapp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class AsesoresSecundariosController : Controller
    {
        private readonly MDbContext _context;
        public AsesoresSecundariosController(MDbContext context)
        {
            _context = context;
        }
    
        [HttpGet]
        public IActionResult AsignarAsesoresSecundariosView()
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                var asesorNombre = _context.usuarios
                                .Where(u => u.IdUsuario == usuarioId)
                                .Select(u => u.NombresCompletos)
                                .FirstOrDefault();

                var asesoresAsignados = ( from u in _context.usuarios
                                        where u.IDUSUARIOSUP == usuarioId
                                        select new 
                                        {
                                            idUsuario = u.IdUsuario,
                                            NombresCompletos = u.NombresCompletos,
                                        }).ToList();
                
                return PartialView("_AsignarAsesoresSecundarios");
            }
            catch (System.Exception ex)
            {
                return Json(new { error = true, message = ex.Message });
            }
        }

    }
}