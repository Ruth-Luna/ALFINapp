using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Derivacion;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class DerivacionController : Controller
    {
        private readonly DBServicesDerivacion _dBServicesDerivacion;
        private readonly DBServicesConsultasSupervisores _dBServicesConsultasSupervisores;
        private readonly DBServicesConsultasAdministrador _dBServicesConsultasAdministrador;
        private readonly DBServicesGeneral _dBServicesGeneral;
        private readonly IUseCaseGetDerivacion _useCaseGetDerivacion;
        public DerivacionController(
            DBServicesDerivacion dBServicesDerivacion,
            DBServicesConsultasSupervisores dBServicesConsultasSupervisores,
            DBServicesGeneral dBServicesGeneral,
            DBServicesConsultasAdministrador dBServicesConsultasAdministrador,
            IUseCaseGetDerivacion useCaseGetDerivacion)
        {
            _dBServicesDerivacion = dBServicesDerivacion;
            _dBServicesConsultasSupervisores = dBServicesConsultasSupervisores;
            _dBServicesGeneral = dBServicesGeneral;
            _dBServicesConsultasAdministrador = dBServicesConsultasAdministrador;
            _useCaseGetDerivacion = useCaseGetDerivacion;
        }

        [HttpGet]
        [PermissionAuthorization("Derivacion", "Derivacion")]
        public async Task<IActionResult> Derivacion()
        {
            var UsuarioIdSupervisor = HttpContext.Session.GetInt32("UsuarioId");
            if (UsuarioIdSupervisor == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión.";
                return RedirectToAction("Redireccionar", "Error");
            }
            var rolUsuario = HttpContext.Session.GetInt32("RolUser");
            if (rolUsuario == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión.";
                return RedirectToAction("Redireccionar", "Error");
            }
            var execute = await _useCaseGetDerivacion.Execute(UsuarioIdSupervisor.Value, rolUsuario.Value);
            if (!execute.success)
            {
                TempData["MessageError"] = execute.message;
                return RedirectToAction("Redireccionar", "Error");
            }
            var viewDerivaciones = execute.data;
            return View("Derivacion", viewDerivaciones);
        }
    }
}