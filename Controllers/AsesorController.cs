using ALFINapp.API.Filters;
using ALFINapp.API.Models;
using ALFINapp.Datos;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.API.Controllers
{
    [RequireSession]

    public class AsesorController : Controller
    {
        private readonly DA_Usuario _da_usuario = new DA_Usuario();
        public AsesorController(){}

        [HttpPost]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> ActivarAsesor(string DNI, int idUsuario)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            int? rol = HttpContext.Session.GetInt32("RolUser");
            if (usuarioId == null || rol == null)
            {
                TempData["Message"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                if (string.IsNullOrEmpty(DNI))
                {
                    return Json(new { success = false, message = "Debe ingresar el DNI del asesor" });
                }
                if (idUsuario == 0)
                {
                    return Json(new { success = false, message = "Debe ingresar el Id del asesor" });
                }

                var asesor = _da_usuario.getUsuario(idUsuario);
                if (asesor == null)
                {
                    return Json(new { success = false, message = "No se encontró el asesor" });
                }
                if (asesor.Estado == "ACTIVO")
                {
                    return Json(new { success = false, message = "El asesor ya se encuentra activo" });
                }
                var asesorv = new ViewUsuario(asesor);
                asesorv.Estado = "ACTIVO";
                var estadoActivacion = await _da_usuario.ActualizarUsuario(asesorv);
                if (!estadoActivacion.IsSuccess)
                {
                    return Json(new { success = false, message = "No se pudo activar el asesor" });
                }
                return Json(new { success = true, message = "Asesor activado correctamente" });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Ha ocurrido un error al activar el asesor" });
                throw;
            }
        }

        [HttpPost]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> DesactivarAsesor(string DNI, int idUsuario)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["Message"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                //Verificar Datos Enviados
                if (string.IsNullOrEmpty(DNI))
                {
                    return Json(new { success = false, message = "Debe ingresar el DNI del asesor" });
                }
                if (idUsuario == 0)
                {
                    return Json(new { success = false, message = "Debe ingresar el Id del asesor" });
                }

                var asesor = _da_usuario.getUsuario(idUsuario);
                if (asesor == null)
                {
                    return Json(new { success = false, message = "No se encontró el asesor" });
                }
                if (asesor.Estado == "INACTIVO")
                {
                    return Json(new { success = false, message = "El asesor ya se encuentra inactivo" });
                }
                var asesorD = new ViewUsuario(asesor);
                asesorD.Estado = "INACTIVO";
                var estado = await _da_usuario.ActualizarUsuario(asesorD);
                if (!estado.IsSuccess)
                {
                    return Json(new { success = false, message = "No se pudo desactivar el asesor" });
                }
                return Json(new { success = true, message = "Asesor desactivado correctamente" });
            }
            catch (System.Exception)
            {
                return Json(new { success = false, message = "Ha ocurrido un error al desactivar el asesor" });
                throw;
            }
        }

        /// <summary>
        /// Agrega un nuevo usuario al sistema con rol de asesor u otros roles permitidos.
        /// </summary>
        /// <param name="nuevoUsuario">Objeto Usuario que contiene la información del nuevo usuario a crear.
        /// Debe incluir DNI, nombres completos, rol y datos de ubicación.</param>
        /// <returns>
        /// IActionResult que contiene un objeto JSON con:
        /// - success: true si la operación fue exitosa, false si hubo errores
        /// - message: Mensaje descriptivo del resultado de la operación
        /// </returns>
        /// <remarks>
        /// El método realiza las siguientes validaciones:
        /// - Verifica que el usuario no sea nulo
        /// - Comprueba que el DNI no esté ya registrado
        /// - Valida que el DNI tenga al menos 8 dígitos
        /// - Verifica que haya un supervisor activo en la sesión
        /// - Asigna automáticamente el supervisor actual como responsable
        /// - Convierte los datos de texto a mayúsculas
        /// - Asigna el rol correspondiente según el IdRol proporcionado
        /// 
        /// La contraseña se genera automáticamente usando el formato: DNI$clave123
        /// </remarks>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var nuevoUsuario = new Usuario
        /// {
        ///     Dni = "12345678",
        ///     NombresCompletos = "Juan Pérez",
        ///     IdRol = 3,
        ///     Departamento = "Lima",
        ///     Provincia = "Lima",
        ///     Distrito = "San Isidro",
        ///     REGION = "Costa"
        /// };
        /// var resultado = await AgregarNuevoAsesor(nuevoUsuario);
        /// </code>
        /// </example>
        /// <exception cref="System.Exception">
        /// Captura y maneja excepciones generales durante el proceso de creación del usuario.
        /// Los mensajes de error se devuelven en la respuesta JSON.
        /// </exception>
        [HttpPost]
        public async Task<IActionResult> AgregarNuevoAsesor([FromBody] Usuario nuevoUsuario)
        {
            if (nuevoUsuario == null)
            {
                return Json(new { success = false, message = "El usuario no puede ser nulo." });
            }
            try
            {
                if (string.IsNullOrEmpty(nuevoUsuario.Dni) ||
                    (nuevoUsuario.Dni.Length != 8 && nuevoUsuario.Dni.Length != 9) ||
                    !nuevoUsuario.Dni.All(char.IsDigit))
                {
                    return Json(new { success = false, message = "El DNI o documento de Identidad debe contener 8 o 9 dígitos numéricos." });
                }

                var usuarioExistente = _da_usuario.getUsuarioPorDni(nuevoUsuario.Dni );
                if (usuarioExistente != null)
                {
                    return Json(new { success = false, message = "El Documento ya está registrado en la base de datos." });
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Los datos enviados no son válidos" });
                }

                int? idsupervisoractual = HttpContext.Session.GetInt32("UsuarioId");

                if (idsupervisoractual == null)
                {
                    return Json(new { success = false, message = "El ID Supervisor a asignar automaticamente es invalido. Comunicarse con Soporte Tecnico." });
                }
                else
                {
                    var supervisorData = _da_usuario.getUsuario(idsupervisoractual.Value);
                    if (supervisorData == null)
                    {
                        return Json(new { success = false, message = "No se encontró el supervisor actual." });
                    }
                    nuevoUsuario.IDUSUARIOSUP = idsupervisoractual.Value;
                    nuevoUsuario.RESPONSABLESUP = supervisorData.NombresCompletos;
                }

                if (!string.IsNullOrEmpty(nuevoUsuario.NombresCompletos) && nuevoUsuario.NombresCompletos.Split(' ').Length > 2)
                {
                    nuevoUsuario.usuario = nuevoUsuario.NombresCompletos.Split(' ')[0] + "." + nuevoUsuario.NombresCompletos.Split(' ')[1];
                    nuevoUsuario.Apellido_Materno = nuevoUsuario.NombresCompletos.Split(' ')[2];
                    nuevoUsuario.Apellido_Paterno = nuevoUsuario.NombresCompletos.Split(' ')[1];
                    nuevoUsuario.Nombres = string.Join(" ", nuevoUsuario.NombresCompletos.Split(' ').Skip(3));
                    nuevoUsuario.NombresCompletos = nuevoUsuario.NombresCompletos?.ToUpper();
                }
                else
                {
                    return Json(new { success = false, message = "El nombre completo debe contener al menos tres palabras." });
                }

                if (nuevoUsuario.IdRol == 0)
                {
                    return Json(new { success = false, message = "Debe seleccionar un Rol para el nuevo usuario" });
                }
                if (nuevoUsuario.IdRol == 1)
                {
                    nuevoUsuario.Rol = "ADMINISTRADOR";
                }
                else if (nuevoUsuario.IdRol == 2)
                {
                    nuevoUsuario.Rol = "SUPERVISOR";
                }
                else if (nuevoUsuario.IdRol == 3)
                {
                    nuevoUsuario.Rol = "ASESOR";
                }
                else
                {
                    nuevoUsuario.Rol = "DESCONOCIDO";
                }
                nuevoUsuario.Departamento = nuevoUsuario.Departamento?.ToUpper();
                nuevoUsuario.Provincia = nuevoUsuario.Provincia?.ToUpper();
                nuevoUsuario.Distrito = nuevoUsuario.Distrito?.ToUpper();
                nuevoUsuario.REGION = nuevoUsuario.REGION?.ToUpper();

                nuevoUsuario.FechaRegistro = DateTime.Now;
                nuevoUsuario.Estado = "ACTIVO";
                nuevoUsuario.contraseña = $"{nuevoUsuario.Dni}$clave123";
                
                if (idsupervisoractual == null)
                {
                    return Json(new { success = false, message = "El ID Supervisor a asignar automaticamente es invalido. Comunicarse con Soporte Tecnico." });
                }
                var usuariov = new ViewUsuario(nuevoUsuario);
                var EnviarNuevoUsuario = await _da_usuario.CrearUsuario(usuariov, idsupervisoractual ?? 0);
                if (!EnviarNuevoUsuario.IsSuccess)
                {
                    return Json(new { success = false, message = EnviarNuevoUsuario.Message });
                }
                return Json(new { success = true, message = $"Se ha agregado al nuevo Usuario {nuevoUsuario.NombresCompletos} con el Rol {nuevoUsuario.Rol}. Usted ha sido asignado como Supervisor del Asesor de Creditos" });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}