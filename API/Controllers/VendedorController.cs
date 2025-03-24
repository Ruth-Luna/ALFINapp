using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Vendedor;
using ALFINapp.Application.UseCases.Tipificacion;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class VendedorController : Controller
    {
        private readonly MDbContext _context;
        private readonly IUseCaseGetInicio _useCaseGetInicio;
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesConsultasAsesores _dbServicesAsesores;
        private readonly DBServicesTipificaciones _dbServicesTipificaciones;
        private readonly DBServicesConsultasClientes _dbServicesConsultasClientes;
        public VendedorController(
            MDbContext context,
            DBServicesConsultasAsesores dbServicesAsesores,
            DBServicesGeneral dbServicesGeneral,
            DBServicesTipificaciones dbServicesTipificaciones,
            DBServicesConsultasClientes dbServicesConsultasClientes,
            IUseCaseGetInicio useCaseGetInicio
            )
        {
            _context = context;
            _dbServicesAsesores = dbServicesAsesores;
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesTipificaciones = dbServicesTipificaciones;
            _dbServicesConsultasClientes = dbServicesConsultasClientes;
            this._useCaseGetInicio = useCaseGetInicio;
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
            ViewData["TotalClientes"] = dataInicio.clientesTotal;
            ViewData["ClientesPendientes"] = dataInicio.clientesPendientes;
            ViewData["ClientesTipificados"] = dataInicio.clientesTipificados;
            // Asignar el nombre del usuario a la vista
            ViewData["UsuarioNombre"] = dataInicio.Vendedor!=null ? dataInicio.Vendedor.NombresCompletos : "Usuario No Encontrado";
            ViewData["ClientesTraidosDBALFIN"] = dataInicio.ClientesAlfin;
            return View("Main", dataInicio.ClientesA365);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarCliente(BaseCliente model)
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["MessageError"] = "No ha iniciado sesion, por favor inicie sesion.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Dni) ||
                    string.IsNullOrWhiteSpace(model.XAppaterno) ||
                    string.IsNullOrWhiteSpace(model.XApmaterno) ||
                    string.IsNullOrWhiteSpace(model.XNombre) ||
                    model.Edad == null ||
                    string.IsNullOrWhiteSpace(model.Departamento))
                {
                    TempData["MessageError"] = "Debe llenar todos los campos!";
                    return RedirectToAction("Redireccionar", "Error");
                }

                Regex regexNoNumeros = new Regex(@"^\D+$");

                if (!regexNoNumeros.IsMatch(model.XAppaterno))
                {
                    TempData["MessageError"] = "El apellido paterno no debe contener números.";
                    return RedirectToAction("Redireccionar", "Error");
                }

                if (!regexNoNumeros.IsMatch(model.XApmaterno))
                {
                    TempData["MessageError"] = "El apellido materno no debe contener números.";
                    return RedirectToAction("Redireccionar", "Error");
                }

                if (!regexNoNumeros.IsMatch(model.XNombre))
                {
                    TempData["MessageError"] = "El nombre no debe contener números.";
                    return RedirectToAction("Redireccionar", "Error");
                }

                if (!regexNoNumeros.IsMatch(model.Departamento))
                {
                    TempData["MessageError"] = "El departamento no debe contener números.";
                    return RedirectToAction("Redireccionar", "Error");
                }

                // Validar que la edad sea un número positivo.
                if (!int.TryParse(model.Edad.ToString(), out int edad) || edad <= 0)
                {
                    TempData["MessageError"] = "La edad debe ser un número válido y mayor que cero.";
                    return RedirectToAction("Redireccionar", "Error");
                }

                var idSupervisor = (from u in _context.usuarios
                                    where u.IdUsuario == HttpContext.Session.GetInt32("UsuarioId")
                                    select u.IDUSUARIOSUP
                                ).FirstOrDefault();

                if (idSupervisor == null)
                {
                    TempData["MessageError"] = "Usted no tiene un Supervisor Asignado, no puede agregar clientes.";
                    return RedirectToAction("Redireccionar", "Error");
                }


                _context.base_clientes.Add(model);
                _context.SaveChanges();

                var idBase = model.IdBase;

                ClientesEnriquecido model2 = new ClientesEnriquecido
                {
                    IdBase = idBase,
                    Telefono1 = "0",
                    Telefono2 = "0",
                    Telefono3 = "0",
                    Telefono4 = "0",
                    Telefono5 = "0",
                    FechaEnriquecimiento = DateTime.Now
                };
                _context.clientes_enriquecidos.Add(model2);
                _context.SaveChanges();

                var idCliente = (from ce in _context.clientes_enriquecidos
                                 where ce.IdBase == idBase
                                 select ce.IdCliente
                                ).FirstOrDefault();

                ClientesAsignado model3 = new ClientesAsignado
                {
                    IdCliente = idCliente,
                    IdUsuarioV = HttpContext.Session.GetInt32("UsuarioId") ?? 0,
                    FechaAsignacionVendedor = DateTime.Now,
                    FechaAsignacionSup = null,
                    IdUsuarioS = idSupervisor,
                    FuenteBase = "BASE_ASESORES"
                };

                _context.clientes_asignados.Add(model3);
                _context.SaveChanges();

                DetalleBase model4 = new DetalleBase
                {
                    IdBase = idBase,
                    TipoClienteRiegos = "3.NO CLIENTE",
                    FechaCarga = DateTime.Now,
                    TipoBase = "BASE_ASESORES"
                };

                _context.detalle_base.Add(model4);
                _context.SaveChanges();

                TempData["Message"] = "Cliente agregado con exito!";
                return RedirectToAction("Redireccionar", "Error");
            }
            TempData["MessageError"] = "El modelo enviado no es valido Comunicarse con servicio tecnico.";
            return RedirectToAction("Redireccionar", "Error");
        }

        [HttpGet]
        public async Task<IActionResult> TipificarClienteView(int id_base)
        {
            try
            {
                var IdUsuario = HttpContext.Session.GetInt32("UsuarioId");
                if (IdUsuario == null)
                {
                    return Json(new { success = false, message = "No ha iniciado sesion, por favor inicie sesion." });
                }

                var detallesClientes = await _dbServicesConsultasClientes.GetDataParaTipificarClienteA365(id_base, IdUsuario.Value);
                if (!detallesClientes.IsSuccess || detallesClientes.Data == null)
                {
                    return Json(new { success = false, message = detallesClientes.message });
                }
                var tipificaciones = await _dbServicesTipificaciones.ObtenerTipificaciones();
                if (!tipificaciones.IsSuccess || tipificaciones.Data == null)
                {
                    return Json(new { success = false, message = tipificaciones.Message });
                }
                var telefonosManuales = await _dbServicesConsultasClientes.GetTelefonosTraidosManualmente(detallesClientes.Data.IdCliente != null ? detallesClientes.Data.IdCliente.Value : 0);
                if (!telefonosManuales.IsSuccess)
                {
                    return Json(new { success = false, message = telefonosManuales.message });
                }
                var getAgenciasDisponibles = await _dbServicesGeneral.GetUAgenciasConNumeros();
                if (getAgenciasDisponibles.IsSuccess == false || getAgenciasDisponibles.data == null)
                {
                    return Json(new { success = false, message = getAgenciasDisponibles.Message });
                }

                ViewData["Tipificaciones"] = tipificaciones.Data;
                ViewData["AgenciasDisponibles"] = getAgenciasDisponibles.data;
                ViewData["numerosCreadosPorElUsuario"] = telefonosManuales.Data != null ? telefonosManuales.Data : null;
                ViewData["DNIcliente"] = detallesClientes.Data.Dni;
                ViewData["ID_asignacion"] = detallesClientes.Data.IdAsignacion ?? 0;
                ViewData["Fuente_BD"] = detallesClientes.Data.FuenteBase ?? "Fuente no disponible";
                ViewData["ID_cliente"] = detallesClientes.Data.IdCliente ?? 0;
                return PartialView("_Tipificarcliente", detallesClientes.Data);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TipificarClienteDBALFINView(int id_base)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "No ha iniciado sesion, por favor inicie sesion." });
                }

                var detallesClientes = await _dbServicesConsultasClientes.GetDataParaTipificarClienteAlfin(id_base, usuarioId.Value);
                if (!detallesClientes.IsSuccess || detallesClientes.Data == null)
                {
                    return Json(new { success = false, message = detallesClientes.message });
                }

                var tipificaciones = await _dbServicesTipificaciones.ObtenerTipificaciones();
                if (!tipificaciones.IsSuccess || tipificaciones.Data == null)
                {
                    return Json(new { success = false, message = tipificaciones.Message });
                }
                var telefonosManuales = await _dbServicesConsultasClientes.GetTelefonosTraidosManualmente(detallesClientes.Data.IdCliente != null ? detallesClientes.Data.IdCliente.Value : 0);
                if (!telefonosManuales.IsSuccess)
                {
                    return Json(new { success = false, message = telefonosManuales.message });
                }

                var getAgenciasDisponibles = await _dbServicesGeneral.GetUAgenciasConNumeros();
                if (getAgenciasDisponibles.IsSuccess == false || getAgenciasDisponibles.data == null)
                {
                    return Json(new { success = false, message = getAgenciasDisponibles.Message });
                }

                ViewData["Tipificaciones"] = tipificaciones.Data;
                ViewData["AgenciasDisponibles"] = getAgenciasDisponibles.data;
                ViewData["numerosCreadosPorElUsuario"] = telefonosManuales.Data != null ? telefonosManuales.Data : null;
                ViewData["DNIcliente"] = detallesClientes.Data.Dni;
                ViewData["ID_asignacion"] = detallesClientes.Data.IdAsignacion ?? 0;
                ViewData["Fuente_BD"] = "BASE_ASESORES";
                ViewData["ID_cliente"] = detallesClientes.Data.IdCliente ?? 0;

                return PartialView("_Tipificarcliente", detallesClientes.Data);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        /*public async Task<(bool success, string errorMessage)> EnviarFormularioRemoto(
                    string DNIAsesor,
                    string DNICliente,
                    string NombreCliente,
                    string CelularCliente,
                    string AgenciaComercial)
        {
            var agenciasMapeadas = new Dictionary<string, string>
            {
                { "738363 - CAJAMARCA", "1" },
                { "737490 - CASTILLA", "2" },
                { "734281 - CHICLAYO BALTA", "3" },
                { "734272 - CHIMBOTE", "4" },
                { "738360 - MOSHOQUEQUE", "5" },
            };

            if (!agenciasMapeadas.TryGetValue(AgenciaComercial, out var agenciaMapeada))
            {
                return (false, "La agencia comercial no es válida.");
            }

            try
            {
                // Validar datos
                if (string.IsNullOrEmpty(DNIAsesor) || string.IsNullOrEmpty(DNICliente) || string.IsNullOrEmpty(AgenciaComercial))
                {
                    throw new ArgumentException("Faltan datos obligatorios para enviar el formulario.");
                }

                // Inicializar Playwright
                var playwright = await Playwright.CreateAsync();
                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Devtools = false
                });
                var page = await browser.NewPageAsync();

                // Navegar al formulario
                await page.GotoAsync("https://forms.office.com/r/UyLj0fGD0R");

                // Completar el formulario
                await page.WaitForSelectorAsync("input[aria-labelledby='QuestionId_red7efe31020b490b8edb703f5d535d7e QuestionInfo_red7efe31020b490b8edb703f5d535d7e']");
                await page.FillAsync("input[aria-labelledby='QuestionId_red7efe31020b490b8edb703f5d535d7e QuestionInfo_red7efe31020b490b8edb703f5d535d7e']", DNIAsesor);
                await page.FillAsync("input[aria-labelledby='QuestionId_r972e879cbae54f07a4e712b4d2629d6e QuestionInfo_r972e879cbae54f07a4e712b4d2629d6e']", "A365");
                await page.FillAsync("input[aria-labelledby='QuestionId_r99924cac2a1d41ff8545bb4b36a8b2b5 QuestionInfo_r99924cac2a1d41ff8545bb4b36a8b2b5']", DNICliente);
                await page.FillAsync("input[aria-labelledby='QuestionId_r6c5c337ce40846d781b1c4325e32c959 QuestionInfo_r6c5c337ce40846d781b1c4325e32c959']", NombreCliente);
                await page.FillAsync("input[aria-labelledby='QuestionId_rd258a5d0f47745368dcfb72d622e3a41 QuestionInfo_rd258a5d0f47745368dcfb72d622e3a41']", CelularCliente);

                // Seleccionar la agencia
                await page.ClickAsync("div[aria-labelledby='QuestionId_re78f9559e1fd4c88bc8b73c80f551108 QuestionInfo_re78f9559e1fd4c88bc8b73c80f551108']");
                await page.WaitForSelectorAsync("div[aria-expanded='true']");

                var sucursalLocator = page.Locator($"div[aria-selected='false'][role='option'][aria-posinset='{agenciaMapeada}']");

                if (await sucursalLocator.CountAsync() == 0)
                {
                    throw new Exception($"No se encontró la opción de agencia con aria-posinset='{agenciaMapeada}'");
                }

                await sucursalLocator.ClickAsync();
                await page.ClickAsync("input[name='r5567eb2c5c214f07aff54dbdd756b157']");
                await page.ClickAsync("button[data-automation-id='submitButton']");

                await browser.CloseAsync();

                return (true, "Formulario enviado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Imprime el error en la consola del servidor
                return (false, ex.Message); // Retorna el mensaje de error
            }
        }*/


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

        [HttpPost]
        public async Task<IActionResult> GuardarTipificacionesNumPersonales(List<TipificarClienteDTO> tipificaciones, int IdAsignacionCliente)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            var ClienteAsignado = await _dbServicesAsesores.ObtenerAsignacion(usuarioId.Value, IdAsignacionCliente);
            if (!ClienteAsignado.IsSuccess || ClienteAsignado.Data == null)
            {
                TempData["MessageError"] = ClienteAsignado.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            var obtenerDataUsuario = await _dbServicesGeneral.GetUserInformation(usuarioId.Value);
            if (!obtenerDataUsuario.IsSuccess || obtenerDataUsuario.Data == null)
            {
                TempData["MessageError"] = obtenerDataUsuario.Message;
                return RedirectToAction("Redireccionar", "Error");
            }

            if (tipificaciones == null || !tipificaciones.Any())
            {
                TempData["MessageError"] = "No se estan enviando datos para guardar.";
                return RedirectToAction("Redireccionar", "Error");
            }

            int? tipificacionMayorPeso = null;
            int? pesoMayor = 0;
            string? descripcionTipificacionMayorPeso = null;
            var agregado = false;

            foreach (var tipificacion in tipificaciones)
            {
                if (tipificacion.TipificacionId == 0)
                {
                    Console.WriteLine("TipificacionId es 0, no se revisara este campo.");
                    continue; // Salta al siguiente registro sin hacer la inserción
                }

                if (tipificacion.FechaVisita == null && tipificacion.TipificacionId == 2)
                {

                    TempData["MessageError"] = "Debe ingresar una fecha de derivación para la tipificación CLIENTE ACEPTO OFERTA DERIVACION (Se obviaron las inserciones).";
                    return RedirectToAction("Redireccionar", "Error");
                }
                if (tipificacion.TipificacionId == 2)
                {
                    var verificarTipificacion = _context.derivaciones_asesores
                        .Where(d => d.DniAsesor == obtenerDataUsuario.Data.Dni
                            && d.IdCliente == ClienteAsignado.Data.IdCliente
                            && d.FechaDerivacion.Year == DateTime.Now.Year
                            && d.FechaDerivacion.Month == DateTime.Now.Month)
                        .ToList();

                    if (verificarTipificacion == null || verificarTipificacion.Count == 0)
                    {
                        TempData["MessageError"] = "No ha enviado la derivacion correspondiente. No se guardara ninguna Tipificacion";
                        return RedirectToAction("Redireccionar", "Error");
                    }
                    if (verificarTipificacion.Count > 1)
                    {
                        TempData["MessageError"] = "Usted ya ha derivado previamente a este cliente durante este mes, para ver su estado puede dirigirse a la pestana de Derivaciones. No se han subido al sistema ninguna de las tipificaciones.";
                        return RedirectToAction("Redireccionar", "Error");
                    }

                    var checkGestionDetalle = _context.GESTION_DETALLE
                        .Where(gd => gd.DocCliente == verificarTipificacion[0].DniCliente
                            && gd.FechaGestion.Year == DateTime.Now.Year
                            && gd.FechaGestion.Month == DateTime.Now.Month
                            && gd.DocAsesor == obtenerDataUsuario.Data.Dni)
                        .ToList();
                    if (checkGestionDetalle.Count > 0 || checkGestionDetalle != null)
                    {
                        TempData["MessageError"] = "Usted ya ha derivado previamente a este cliente durante este mes, para ver su estado puede dirigirse a la pestana de Derivaciones. No se han subido al sistema ninguna de las tipificaciones.";
                        return RedirectToAction("Redireccionar", "Error");
                    }
                }
                agregado = true;
            }

            if (agregado == false)
            {
                TempData["MessageError"] = "No se ha llenado ningun campo o se han llenado incorrectamente.";
                return RedirectToAction("Redireccionar", "Error");
            }

            foreach (var tipificacion in tipificaciones)
            {
                if (tipificacion.TipificacionId == 0)
                {
                    Console.WriteLine("TipificacionId es 0, omitiendo inserción.");
                    continue; // Salta al siguiente registro sin hacer la inserción
                }

                var nuevaTipificacion = new ClientesTipificado
                {
                    IdAsignacion = IdAsignacionCliente,
                    IdTipificacion = tipificacion.TipificacionId,
                    FechaTipificacion = DateTime.Now,
                    Origen = "nuevo",
                    TelefonoTipificado = tipificacion.Telefono,
                    DerivacionFecha = tipificacion.FechaVisita
                };

                var resultGuardarClienteTipificado = await _dbServicesTipificaciones.GuardarNuevaTipificacion(nuevaTipificacion);
                if (!resultGuardarClienteTipificado.IsSuccess)
                {
                    TempData["MessageError"] = resultGuardarClienteTipificado.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                // Verificar si esta tipificación tiene el mayor peso
                var tipificacionActual = _context.tipificaciones.FirstOrDefault(t => t.IdTipificacion == tipificacion.TipificacionId);
                if (tipificacionActual != null && tipificacionActual.Peso.HasValue)
                {
                    int pesoActual = tipificacionActual.Peso.Value;

                    if (pesoMayor == 0 || pesoActual > pesoMayor)
                    {
                        pesoMayor = pesoActual;
                        tipificacionMayorPeso = tipificacion.TipificacionId;
                        descripcionTipificacionMayorPeso = tipificacionActual.DescripcionTipificacion;
                    }
                }
                var telefonoTipificado = _context.telefonos_agregados.FirstOrDefault(ta => ta.Telefono == tipificacion.Telefono && ta.IdCliente == ClienteAsignado.Data.IdCliente);
                if (telefonoTipificado == null)
                {
                    TempData["MessageError"] = "El Numero de Telefono Agregado no ha sido encontrado.";
                    return RedirectToAction("Redireccionar", "Error");
                }
                else
                {
                    var tipificacionUltima = _context.tipificaciones.FirstOrDefault(t => t.IdTipificacion == tipificacion.TipificacionId);
                    if (tipificacionUltima == null)
                    {
                        TempData["MessageError"] = "La tipificacion no ha sido encontrada.";
                        return RedirectToAction("Redireccionar", "Error");
                    }
                    telefonoTipificado.UltimaTipificacion = tipificacionUltima.DescripcionTipificacion;
                    telefonoTipificado.FechaUltimaTipificacion = DateTime.Now;
                    telefonoTipificado.IdClienteTip = nuevaTipificacion.IdClientetip;
                    _context.telefonos_agregados.Update(telefonoTipificado);
                }

                var clienteEnriquecido = await _dbServicesAsesores.ObtenerEnriquecido(ClienteAsignado.Data.IdCliente);
                if (!clienteEnriquecido.IsSuccess || clienteEnriquecido.Data == null)
                {
                    TempData["MessageError"] = clienteEnriquecido.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                var guardarGestionDetalle = await _dbServicesTipificaciones.GuardarGestionDetalle(ClienteAsignado.Data, nuevaTipificacion, clienteEnriquecido.Data, usuarioId.Value);
                if (!guardarGestionDetalle.IsSuccess)
                {
                    TempData["MessageError"] = guardarGestionDetalle.Message;
                    return RedirectToAction("Redireccionar", "Error");
                }
                await _context.SaveChangesAsync();
            }

            if (tipificacionMayorPeso.HasValue && pesoMayor != 0)
            {
                if (pesoMayor > (ClienteAsignado.Data.PesoTipificacionMayor ?? 0))
                {
                    ClienteAsignado.Data.TipificacionMayorPeso = descripcionTipificacionMayorPeso;
                    ClienteAsignado.Data.PesoTipificacionMayor = pesoMayor;
                    ClienteAsignado.Data.FechaTipificacionMayorPeso = DateTime.Now;
                    _context.clientes_asignados.Update(ClienteAsignado.Data);
                }
            }
            _context.SaveChanges();
            TempData["Message"] = "Las tipificaciones se han guardado correctamente (Se han Obviado los campos Vacios y los campos que fueron llenados con datos incorrectos).";
            return RedirectToAction("Redireccionar", "Error");
        }
    }
}