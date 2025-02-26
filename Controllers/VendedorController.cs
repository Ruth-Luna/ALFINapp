using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Filters;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ALFINapp.Services;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class VendedorController : Controller
    {
        private readonly MDbContext _context;
        private readonly DBServicesGeneral _dbServicesGeneral;
        private readonly DBServicesConsultasAsesores _dbServicesAsesores;
        private readonly DBServicesTipificaciones _dbServicesTipificaciones;
        private readonly DBServicesConsultasClientes _dbServicesConsultasClientes;
        public VendedorController(
            MDbContext context,
            DBServicesConsultasAsesores dbServicesAsesores,
            DBServicesGeneral dbServicesGeneral,
            DBServicesTipificaciones dbServicesTipificaciones,
            DBServicesConsultasClientes dbServicesConsultasClientes)
        {
            _context = context;
            _dbServicesAsesores = dbServicesAsesores;
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesTipificaciones = dbServicesTipificaciones;
            _dbServicesConsultasClientes = dbServicesConsultasClientes;
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

            // Coger los resultados para mostrar la página de Inicio
            var detallesClientes = await _dbServicesAsesores.DetallesClientesParaVentas(usuarioId.Value);

            // Obtener el usuario actual
            var usuario = await _context.usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);

            var ClientesTraidosDBALFIN = await _dbServicesAsesores.ClientesTraidosDBALFIN(usuarioId.Value);

            // Clientes aún no tipificados (su campo 'TipificacionDeMayorPeso' es null o vacío)
            int clientesPendientes = detallesClientes.Count(dc =>
                (dc.FechaTipificacionDeMayorPeso.HasValue
                && dc.FechaTipificacionDeMayorPeso.Value.Year != DateTime.Now.Year
                && dc.FechaTipificacionDeMayorPeso.Value.Month != DateTime.Now.Month)
                || (!dc.FechaTipificacionDeMayorPeso.HasValue)) +
                (ClientesTraidosDBALFIN.Data == null ? 0 :
                ClientesTraidosDBALFIN.Data.Count(da =>
                (da.FechaTipificacionDeMayorPeso.HasValue
                 && da.FechaTipificacionDeMayorPeso.Value.Year != DateTime.Now.Year
                 && da.FechaTipificacionDeMayorPeso.Value.Month != DateTime.Now.Month)
                 || (!da.FechaTipificacionDeMayorPeso.HasValue)));

            // Clientes ya tipificados (su campo 'TipificacionDeMayorPeso' no es null ni vacío)
            int clientesTipificados = detallesClientes.Count(dc =>
                dc.FechaTipificacionDeMayorPeso.HasValue
                && dc.FechaTipificacionDeMayorPeso.Value.Year == DateTime.Now.Year
                && dc.FechaTipificacionDeMayorPeso.Value.Month == DateTime.Now.Month) +
                (ClientesTraidosDBALFIN.Data == null ? 0 :
                ClientesTraidosDBALFIN.Data.Count(da =>
                da.FechaTipificacionDeMayorPeso.HasValue
                && da.FechaTipificacionDeMayorPeso.Value.Year == DateTime.Now.Year
                && da.FechaTipificacionDeMayorPeso.Value.Month == DateTime.Now.Month));

            // Total de clientes
            int totalClientes = detallesClientes.Count +
             (ClientesTraidosDBALFIN.Data == null ? 0 : ClientesTraidosDBALFIN.Data.Count);

            ViewData["TotalClientes"] = totalClientes;
            ViewData["ClientesPendientes"] = clientesPendientes;
            ViewData["ClientesTipificados"] = clientesTipificados;
            // Asignar el nombre del usuario a la vista
            ViewData["UsuarioNombre"] = usuario != null ? usuario.NombresCompletos : "Usuario No Encontrado";
            ViewData["ClientesTraidosDBALFIN"] = ClientesTraidosDBALFIN.Data;
            return View("Main", detallesClientes);
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
                    return RedirectToAction("Inicio");
                }

                Regex regexNoNumeros = new Regex(@"^\D+$");

                if (!regexNoNumeros.IsMatch(model.XAppaterno))
                {
                    TempData["MessageError"] = "El apellido paterno no debe contener números.";
                    return RedirectToAction("Inicio");
                }

                if (!regexNoNumeros.IsMatch(model.XApmaterno))
                {
                    TempData["MessageError"] = "El apellido materno no debe contener números.";
                    return RedirectToAction("Inicio");
                }

                if (!regexNoNumeros.IsMatch(model.XNombre))
                {
                    TempData["MessageError"] = "El nombre no debe contener números.";
                    return RedirectToAction("Inicio");
                }

                if (!regexNoNumeros.IsMatch(model.Departamento))
                {
                    TempData["MessageError"] = "El departamento no debe contener números.";
                    return RedirectToAction("Inicio");
                }

                // Validar que la edad sea un número positivo.
                if (!int.TryParse(model.Edad.ToString(), out int edad) || edad <= 0)
                {
                    TempData["MessageError"] = "La edad debe ser un número válido y mayor que cero.";
                    return RedirectToAction("Inicio");
                }

                var idSupervisor = (from u in _context.usuarios
                                    where u.IdUsuario == HttpContext.Session.GetInt32("UsuarioId")
                                    select u.IDUSUARIOSUP
                                ).FirstOrDefault();

                if (idSupervisor == null)
                {
                    TempData["MessageError"] = "Usted no tiene un Supervisor Asignado, no puede agregar clientes.";
                    return RedirectToAction("Inicio");
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
                return RedirectToAction("Inicio");
            }
            TempData["MessageError"] = "El modelo enviado no es valido Comunicarse con servicio tecnico.";
            return RedirectToAction("Inicio");
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
        [HttpPost]
        public async Task<IActionResult> TipificarMotivo(int IdAsignacion, int? Tipificacion_1, int? Tipificacion_2,
                                                int? Tipificacion_3, int? Tipificacion_4, int? Tipificacion_5,
                                                string? Telefono_1, string? Telefono_2, string? Telefono_3,
                                                string? Telefono_4, string? Telefono_5, DateTime? DerivacionDb_1,
                                                DateTime? DerivacionDb_2, DateTime? DerivacionDb_3, DateTime? DerivacionDb_4,
                                                DateTime? DerivacionDb_5, string AgenciaComercialDb_1, string AgenciaComercialDb_2,
                                                string AgenciaComercialDb_3, string AgenciaComercialDb_4, string AgenciaComercialDb_5)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            var obtenerDataUsuario = await _dbServicesGeneral.GetUserInformation(usuarioId.Value);
            if (!obtenerDataUsuario.IsSuccess || obtenerDataUsuario.Data == null)
            {
                TempData["MessageError"] = obtenerDataUsuario.Message;
                return RedirectToAction("Inicio");
            }

            var ClienteAsignado = await _dbServicesAsesores.ObtenerAsignacion(usuarioId.Value, IdAsignacion);
            if (ClienteAsignado.Data == null || !ClienteAsignado.IsSuccess)
            {
                TempData["MessageError"] = ClienteAsignado.Message;
                return RedirectToAction("Inicio");
            }

            var telefonos = new List<string?> { Telefono_1, Telefono_2, Telefono_3, Telefono_4, Telefono_5 };
            var tipificaciones = new List<int?> { Tipificacion_1, Tipificacion_2, Tipificacion_3, Tipificacion_4, Tipificacion_5 };
            var derivaciones = new List<DateTime?> { DerivacionDb_1, DerivacionDb_2, DerivacionDb_3, DerivacionDb_4, DerivacionDb_5 };
            var agenciasComerciales = new List<string> { AgenciaComercialDb_1, AgenciaComercialDb_2, AgenciaComercialDb_3, AgenciaComercialDb_4, AgenciaComercialDb_5 };

            // Fecha de tipificación
            var fechaTipificacion = DateTime.Now;
            string? descripcionTipificacionMayorPeso = null;
            int? pesoMayor = 0;

            var ClientesEnriquecido = await _dbServicesAsesores.ObtenerEnriquecido(ClienteAsignado.Data.IdCliente);
            if (!ClientesEnriquecido.IsSuccess || ClientesEnriquecido.Data == null)
            {
                TempData["MessageError"] = ClientesEnriquecido.Message;
                return RedirectToAction("Inicio");
            }
            var agregado = false;
            int countNonNull = 0;

            for (int i = 0; i < telefonos.Count; i++)
            {
                var telefono = telefonos[i];
                var tipificacion = tipificaciones[i];
                var derivacion = derivaciones[i];

                if (!tipificacion.HasValue)
                {
                    Console.WriteLine("TipificacionId es NULL, no se revisara este campo.");
                    continue; // Salta al siguiente registro sin hacer la inserción
                }

                if (string.IsNullOrEmpty(telefono) || telefono == "0")
                {
                    Console.WriteLine("El Telefono es NULL o 0, este campo debe tener un numero.");
                    TempData["MessageError"] = "Se debe mandar un número de teléfono válido (Se obviaron las inserciones).";
                    return RedirectToAction("Inicio");
                }

                if (derivacion == null && tipificacion == 2)
                {
                    TempData["MessageError"] = "Debe ingresar una fecha de derivación para la tipificación CLIENTE ACEPTO OFERTA DERIVACION (Se obviaron las inserciones).";
                    return RedirectToAction("Inicio");
                }

                if (agenciasComerciales[i] == null && tipificacion == 2)  // Verificamos si el valor no es null
                {
                    TempData["MessageError"] = "Debe ingresar una agencia comercial para la tipificación CLIENTE ACEPTO OFERTA DERIVACION (Se obviaron las inserciones).";
                    return RedirectToAction("Inicio");
                }

                if (tipificacion == 2)
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
                        return RedirectToAction("Inicio");
                    }
                    if (verificarTipificacion.Count > 1)
                    {
                        TempData["MessageError"] = "Usted ya ha derivado previamente a este cliente durante este mes, para ver su estado puede dirigirse a la pestana de Derivaciones. No se han subido al sistema ninguna de las tipificaciones.";
                        return RedirectToAction("Inicio");
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
                        return RedirectToAction("Inicio");
                    }

                }

                if (tipificacion == 2 && derivacion != null && agenciasComerciales[i] != null)
                {
                    countNonNull++;
                }
                agregado = true;
            }

            if (agregado == false)
            {
                TempData["MessageError"] = "No se ha llenado ningun campo o se han llenado incorrectamente.";
                return RedirectToAction("Inicio");
            }

            for (int i = 0; i < telefonos.Count; i++)
            {
                var telefono = telefonos[i];
                var tipificacion = tipificaciones[i];
                var derivacion = derivaciones[i];

                if (!tipificacion.HasValue)
                {
                    Console.WriteLine($"Tipificacion {i + 1} es nulo, se omite la inserción.");
                    continue; // Salta al siguiente registro sin hacer la inserción
                }
                agregado = true;
                var tipificacionInfo = await _dbServicesTipificaciones.ObtenerTipificacion(tipificacion.Value);

                if (!tipificacionInfo.IsSuccess || tipificacionInfo.Data == null)
                {
                    TempData["MessageError"] = tipificacionInfo.Message;
                    return RedirectToAction("Inicio");
                }
                // Actualizar tipificación de mayor peso si aplica
                if (tipificacionInfo.Data.Peso > pesoMayor)
                {
                    pesoMayor = tipificacionInfo.Data.Peso;
                    descripcionTipificacionMayorPeso = tipificacionInfo.Data.DescripcionTipificacion;
                }
                // Agregar un nuevo registro
                var nuevoClienteTipificado = new ClientesTipificado
                {
                    IdAsignacion = IdAsignacion,
                    IdTipificacion = tipificacion.Value,
                    FechaTipificacion = fechaTipificacion,
                    Origen = "nuevo",
                    TelefonoTipificado = telefono,
                    DerivacionFecha = derivacion
                };
                var result = await _dbServicesTipificaciones.GuardarNuevaTipificacion(nuevoClienteTipificado);
                if (!result.IsSuccess)
                {
                    TempData["MessageError"] = result.Message;
                    return RedirectToAction("Inicio");
                }

                // Actualizar última tipificación en clientes_enriquecidos
                var tipificacion_guardada = _context.tipificaciones.FirstOrDefault(t => t.IdTipificacion == tipificacion.Value);
                if (tipificacion_guardada == null)
                {
                    TempData["MessageError"] = "No se ha encontrado la tipificación en la base de datos.";
                    return RedirectToAction("Inicio");
                }
                switch (i + 1)
                {
                    case 1:
                        ClientesEnriquecido.Data.UltimaTipificacionTelefono1 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.Data.FechaUltimaTipificacionTelefono1 = DateTime.Now;
                        ClientesEnriquecido.Data.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 2:
                        ClientesEnriquecido.Data.UltimaTipificacionTelefono2 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.Data.FechaUltimaTipificacionTelefono2 = DateTime.Now;
                        ClientesEnriquecido.Data.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 3:
                        ClientesEnriquecido.Data.UltimaTipificacionTelefono3 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.Data.FechaUltimaTipificacionTelefono3 = DateTime.Now;
                        ClientesEnriquecido.Data.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 4:
                        ClientesEnriquecido.Data.UltimaTipificacionTelefono4 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.Data.FechaUltimaTipificacionTelefono4 = DateTime.Now;
                        ClientesEnriquecido.Data.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 5:
                        ClientesEnriquecido.Data.UltimaTipificacionTelefono5 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.Data.FechaUltimaTipificacionTelefono5 = DateTime.Now;
                        ClientesEnriquecido.Data.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                }
                // Guardar cambios en la base de datos
                _context.clientes_enriquecidos.Update(ClientesEnriquecido.Data);
                await _context.SaveChangesAsync();
                var guardarGestionDetalle = await _dbServicesTipificaciones.GuardarGestionDetalle(ClienteAsignado.Data, nuevoClienteTipificado, ClientesEnriquecido.Data);
                if (!guardarGestionDetalle.IsSuccess)
                {
                    TempData["MessageError"] = guardarGestionDetalle.Message;
                    return RedirectToAction("Inicio");
                }
            }
            if (!string.IsNullOrEmpty(descripcionTipificacionMayorPeso) && pesoMayor > (ClienteAsignado.Data.PesoTipificacionMayor ?? 0))
            {
                ClienteAsignado.Data.TipificacionMayorPeso = descripcionTipificacionMayorPeso; // Almacena la descripción
                ClienteAsignado.Data.PesoTipificacionMayor = pesoMayor; // Almacena el peso
                ClienteAsignado.Data.FechaTipificacionMayorPeso = fechaTipificacion; // Almacena la fecha
                _context.clientes_asignados.Update(ClienteAsignado.Data);
                await _context.SaveChangesAsync();
            }
            string message = "No se han encontrado tipificaciones de Derivacion";
            if (countNonNull >= 1)
            {
                message = "Hay una Tipificacion de Derivacion enviada, el formulario fue enviado correctamente.";
            }

            TempData["Message"] = "Las tipificaciones se han guardado correctamente (Se han Obviado los campos Vacios y los campos que fueron llenados con datos incorrectos). " + message;
            return RedirectToAction("Inicio");
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

        [HttpPost]
        public async Task<IActionResult> GuardarTipificacionesNumPersonales(List<TipificarClienteDTO> tipificaciones, int IdAsignacionCliente)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            var obtenerDataUsuario = await _dbServicesGeneral.GetUserInformation(usuarioId.Value);
            if (!obtenerDataUsuario.IsSuccess || obtenerDataUsuario.Data == null)
            {
                TempData["MessageError"] = obtenerDataUsuario.Message;
                return RedirectToAction("Inicio");
            }

            if (tipificaciones == null || !tipificaciones.Any())
            {
                TempData["MessageError"] = "No se estan enviando datos para guardar.";
                return RedirectToAction("Inicio");
            }

            var ClienteAsignado = await _dbServicesAsesores.ObtenerAsignacion(usuarioId.Value, IdAsignacionCliente);
            if (!ClienteAsignado.IsSuccess || ClienteAsignado.Data == null)
            {
                TempData["MessageError"] = ClienteAsignado.Message;
                return RedirectToAction("Inicio");
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
                    return RedirectToAction("Inicio");
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
                        return RedirectToAction("Inicio");
                    }
                    if (verificarTipificacion.Count > 1)
                    {
                        TempData["MessageError"] = "Usted ya ha derivado previamente a este cliente durante este mes, para ver su estado puede dirigirse a la pestana de Derivaciones. No se han subido al sistema ninguna de las tipificaciones.";
                        return RedirectToAction("Inicio");
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
                        return RedirectToAction("Inicio");
                    }
                }
                agregado = true;
            }

            if (agregado == false)
            {
                TempData["MessageError"] = "No se ha llenado ningun campo o se han llenado incorrectamente.";
                return RedirectToAction("Inicio");
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
                    return RedirectToAction("Inicio");
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
                    return RedirectToAction("Inicio");
                }
                else
                {
                    var tipificacionUltima = _context.tipificaciones.FirstOrDefault(t => t.IdTipificacion == tipificacion.TipificacionId);
                    if (tipificacionUltima == null)
                    {
                        TempData["MessageError"] = "La tipificacion no ha sido encontrada.";
                        return RedirectToAction("Inicio");
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
                    return RedirectToAction("Inicio");
                }
                var guardarGestionDetalle = await _dbServicesTipificaciones.GuardarGestionDetalle(ClienteAsignado.Data, nuevaTipificacion, clienteEnriquecido.Data);
                if (!guardarGestionDetalle.IsSuccess)
                {
                    TempData["MessageError"] = guardarGestionDetalle.Message;
                    return RedirectToAction("Inicio");
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
            return RedirectToAction("Inicio");
        }
    }
}