using ALFINapp.API.DTOs;
using ALFINapp.API.Models;
using ALFINapp.Datos;
using ALFINapp.Datos.DAO.Derivaciones;
using ALFINapp.Datos.DAO.Operaciones;
using ALFINapp.Datos.DAO.Reagendacion;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;

namespace ALFINapp.Controllers
{
    public class OperacionesController : Controller
    {
        private readonly DAO_Operaciones _dao_Operaciones = new DAO_Operaciones();
        private readonly DA_Usuario _dao_Usuario = new DA_Usuario();

        [Route("operaciones")]
        public IActionResult Operaciones()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDerivaciones(int? idAsesor = null, int? idSupervisor = null, string? agencia = null, DateTime? fecha_derivacion = null, DateTime? fecha_visita = null)
        {
            // Recuperar el ID USUSARIO
            int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            int? idRol = HttpContext.Session.GetInt32("RolUser");
            if (idRol == null || idUsuario == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }

            //Logica segun el rol
            if (idRol == 1 || idRol == 4) // ADMINISTRADOR Y GERENTE ZONAL
            {
                // SIN FILTROS SOBREESCRITOS
            }
            else if (idRol == 2) // SUPERVISOR
            {
                idSupervisor = idUsuario;
            }
            else if (idRol == 3) // ASESOR
            {
                idSupervisor = null;
                idAsesor = idUsuario;
            }

            (bool success, List<ViewDerivaciones>? data) = await _dao_Operaciones.GetAllDerivaciones(idAsesor, idSupervisor, agencia, fecha_derivacion, fecha_visita);

            return Json(new { success, data });

        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllReagendamientos(int? idAsesor = null, int? idSupervisor = null, DateTime? fecha_reagendamiento = null, DateTime? fecha_visita = null, string? agencia = null)
        {
            // Recuperar el ID USUSARIO
            int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            int? idRol = HttpContext.Session.GetInt32("RolUser");
            if (idRol == null || idUsuario == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }

            //Logica segun el rol
            if (idRol == 1 || idRol == 4) // ADMINISTRADOR Y GERENTE ZONAL
            {
                // SIN FILTROS SOBREESCRITOS
            }
            else if (idRol == 2) // SUPERVISOR
            {
                idSupervisor = idUsuario;
            }
            else if (idRol == 3) // ASESOR
            {
                idSupervisor = null;
                idAsesor = idUsuario;
            }

            (bool success, List<ViewReagendamientos>? data) = await _dao_Operaciones.GetAllReagendamientos(idAsesor, idSupervisor, fecha_reagendamiento, fecha_visita, agencia);

            return Json(new { success, data });
        }

        [HttpGet]
        public IActionResult ObtenerAsesoresRelacionados()
        {
            // Recuperar el ID USUSARIO
            int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            int? idRol = HttpContext.Session.GetInt32("RolUser");
            if (idRol == null || idUsuario == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }

            int? idSupervisor = null;
            //Logica segun el rol
            if (idRol == 1 || idRol == 4) // ADMINISTRADOR Y GERENTE ZONAL
            {
                // SIN FILTROS SOBREESCRITOS
            }
            else if (idRol == 2) // SUPERVISOR
            {
                idSupervisor = idUsuario;
            }
            else // ASESOR
            {
                return Json(new { success = false, message = "No tiene permisos para ver esta información." });
            }
            var asesores = _dao_Usuario.ListarAsesores(idSupervisor);
            return Json(new { success = true, data = asesores });
        }

        [HttpGet]
        public IActionResult ObtenerSupervisoresRelacionados()
        {
            // Recuperar el ID USUSARIO
            int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            int? idRol = HttpContext.Session.GetInt32("RolUser");
            if (idRol == null || idUsuario == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }

            if (idRol == 1 || idRol == 4) // ADMINISTRADOR Y GERENTE ZONAL
            {
                var supervisores = _dao_Usuario.ListarSupervisores();
                return Json(new { success = true, data = supervisores });
            }
            else
            {
                return Json(new { success = false, message = "No tiene permisos para ver esta información." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarAgencias()
        {

            (bool success, List<ViewAgencias>? data) = await _dao_Operaciones.GetAgencias();
            return Json(new { success, data });
        }

        public async Task<IActionResult> Reagendar(
            [FromBody] DtoVReagendar dtovreagendar)
        {
            /*             if (dtovreagendar.FechaReagendamiento == null || dtovreagendar.FechaReagendamiento == DateTime.MinValue)
                        {
                            return Json(new { success = false, message = "La fecha de reagendamiento es obligatoria." });
                        }
                        if (dtovreagendar.IdDerivacion == null || dtovreagendar.IdDerivacion == 0)
                        {
                            return Json(new { success = false, message = "El id de derivación es obligatorio." });
                        }
                        var exec = await _useCaseReagendar.exec(
                            dtovreagendar.IdDerivacion.Value,
                            dtovreagendar.FechaReagendamiento.Value,
                            dtovreagendar.urlEvidencias);
                        if (!exec.IsSuccess)
                        {
                            return Json(new { success = false, message = exec.Message });
                        }
                        return Json(new { success = true, message = exec.Message }); */
            throw new NotImplementedException();
        }
        public async Task<IActionResult> GetHistoricoReagendamientos(int idDerivacion)
        {
            if (idDerivacion <= 0)
            {
                return Json(new { success = false, message = "El id de derivación es obligatorio." });
            }

            (bool success, List<ViewReagendamientos>? data) = await _dao_Operaciones.GetHistoricoReagendamientos(idDerivacion);

            return Json(new { success, data });

        }
    }
}