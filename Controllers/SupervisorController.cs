using ALFINapp.API.Filters;
using Microsoft.AspNetCore.Mvc;
using ALFINapp.Datos;
using ALFINapp.API.Models;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class SupervisorController : Controller
    {
        private DA_Usuario _da_usuario = new DA_Usuario();
        public SupervisorController(){}
        /// <summary>
        /// Obtiene y muestra la página de inicio del supervisor con información sobre los leads y clientes asignados.
        /// </summary>
        /// <param name="page">Número de página actual para la paginación. Valor predeterminado: 1</param>
        /// <param name="pageSize">Cantidad de elementos por página. Valor predeterminado: 20</param>
        /// <returns>
        /// Vista "Inicio" con la lista de supervisores y datos estadísticos en ViewData:
        /// - UsuarioNombre: Nombre completo del supervisor actual
        /// - ClientesPendientesSupervisor: Número de clientes sin asesor asignado
        /// - DestinoBases: Lista de bases de datos disponibles
        /// - clientesAsignadosSupervisor: Número de clientes con asesor asignado
        /// - totalClientes: Número total de clientes
        /// </returns>
        /// <remarks>
        /// El método realiza las siguientes operaciones:
        /// - Verifica la autenticación del usuario
        /// - Consulta los leads asignados al supervisor
        /// - Calcula estadísticas de asignación de clientes
        /// - Obtiene las bases de datos disponibles para el supervisor
        /// - Limita la lista de supervisores a 200 registros
        /// 
        /// Los datos se almacenan en ViewData para su uso en la vista.
        /// </remarks>
        /// <example>
        /// Uso típico desde una vista:
        /// <code>
        /// @{
        ///     var nombreSupervisor = ViewData["UsuarioNombre"] as string;
        ///     var clientesPendientes = (int)ViewData["ClientesPendientesSupervisor"];
        ///     var totalClientes = (int)ViewData["totalClientes"];
        /// }
        /// </code>
        /// </example>
        /// <exception cref="Exception">
        /// Pueden ocurrir excepciones durante:
        /// - La consulta a la base de datos
        /// - La obtención de datos de sesión
        /// - El procesamiento de los datos del supervisor
        /// </exception>
        [HttpGet]
        [PermissionAuthorization("Supervisor", "Inicio")]
        public IActionResult Inicio()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? IdRol = HttpContext.Session.GetInt32("RolUser");
            if (IdRol == null || usuarioId == null)
            {
                TempData["MessageError"] = "No se ha iniciado sesión";
                return RedirectToAction("Index", "Home");
            }

            var supervisor = _da_usuario.getUsuario(usuarioId.Value);
            if (supervisor == null)
            {
                TempData["MessageError"] = "No se encontró el usuario";
                return RedirectToAction("Index", "Home");
            }
            var supervisorv = new ViewUsuario(supervisor);
            return View("Inicio", supervisorv);
        }

        [HttpGet]
        public IActionResult ObtenerInterfazAsesor(int idUsuario)
        {
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var asesor = _da_usuario.getUsuario(idUsuario);
            
            if (asesor == null)
            {
                TempData["MessageError"] = "Asesor no encontrado";
                return RedirectToAction("Inicio");
            }
            return PartialView("_InterfazActivarAsesor", new ViewUsuario(asesor));
        }
    }
}