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

        public VendedorController(
            MDbContext context,
            DBServicesConsultasAsesores dbServicesAsesores,
            DBServicesGeneral dbServicesGeneral,
            DBServicesTipificaciones dbServicesTipificaciones)
        {
            _context = context;
            _dbServicesAsesores = dbServicesAsesores;
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesTipificaciones = dbServicesTipificaciones;
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
        public IActionResult TipificarClienteView(int id_base)
        {
            try
            {
                if (HttpContext.Session.GetInt32("UsuarioId") == null)
                {
                    return Json(new { success = false, message = "No ha iniciado sesion, por favor inicie sesion." });
                }

                var detallesClientes = (from bc in _context.base_clientes
                                        join db in _context.detalle_base on bc.IdBase equals db.IdBase
                                        join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                        join ca in _context.clientes_asignados on ce.IdCliente equals ca.IdCliente

                                        where bc.IdBase == id_base && db.TipoBase == ca.FuenteBase &&
                                              ca.IdUsuarioV == HttpContext.Session.GetInt32("UsuarioId")
                                        select new
                                        {
                                            bc,
                                            db,
                                            ce,
                                            ca
                                        }).FirstOrDefault();
                if (detallesClientes == null)
                {
                    return Json(new { success = false, message = "No se encontró el cliente en la base de datos." });
                }

                var detalleTipificarCliente = detallesClientes != null
                    ? new DetalleTipificarClienteDTO
                    {
                        // Propiedades de BaseCliente
                        Dni = detallesClientes.bc.Dni,
                        XAppaterno = detallesClientes.bc.XAppaterno,
                        XApmaterno = detallesClientes.bc.XApmaterno,
                        XNombre = detallesClientes.bc.XNombre,
                        Edad = detallesClientes.bc.Edad,
                        Departamento = detallesClientes.bc.Departamento,
                        Provincia = detallesClientes.bc.Provincia,
                        Distrito = detallesClientes.bc.Distrito,
                        IdBase = detallesClientes.bc.IdBase,

                        // Propiedades de DetalleBase
                        Campaña = detallesClientes.db.Campaña,
                        OfertaMax = detallesClientes.db.OfertaMax,
                        TasaMinima = detallesClientes.db.TasaMinima,
                        Sucursal = detallesClientes.db.Sucursal,
                        AgenciaComercial = detallesClientes.db.AgenciaComercial,
                        Plazo = detallesClientes.db.Plazo,
                        Cuota = detallesClientes.db.Cuota,
                        GrupoTasa = detallesClientes.db.GrupoTasa,
                        GrupoMonto = detallesClientes.db.GrupoMonto,
                        Propension = detallesClientes.db.Propension,
                        TipoCliente = detallesClientes.db.TipoCliente,
                        ClienteNuevo = detallesClientes.db.ClienteNuevo,
                        Color = detallesClientes.db.Color,
                        ColorFinal = detallesClientes.db.ColorFinal,

                        // Propiedades de ClientesEnriquecido
                        Telefonos = new List<TelefonoDTO>
                        {
                        new TelefonoDTO { Numero = detallesClientes.ce.Telefono1,
                                        Comentario = detallesClientes.ce.ComentarioTelefono1,
                                        DescripcionTipificacion = detallesClientes.ce.UltimaTipificacionTelefono1,
                                        FechaTipificacion = detallesClientes.ce.FechaUltimaTipificacionTelefono1 },
                        new TelefonoDTO { Numero = detallesClientes.ce.Telefono2,
                                        Comentario = detallesClientes.ce.ComentarioTelefono2,
                                        DescripcionTipificacion = detallesClientes.ce.UltimaTipificacionTelefono2,
                                        FechaTipificacion = detallesClientes.ce.FechaUltimaTipificacionTelefono2  },
                        new TelefonoDTO { Numero = detallesClientes.ce.Telefono3,
                                        Comentario = detallesClientes.ce.ComentarioTelefono3,
                                        DescripcionTipificacion = detallesClientes.ce.UltimaTipificacionTelefono3,
                                        FechaTipificacion = detallesClientes.ce.FechaUltimaTipificacionTelefono3 },
                        new TelefonoDTO { Numero = detallesClientes.ce.Telefono4,
                                        Comentario = detallesClientes.ce.ComentarioTelefono4,
                                        DescripcionTipificacion = detallesClientes.ce.UltimaTipificacionTelefono4,
                                        FechaTipificacion = detallesClientes.ce.FechaUltimaTipificacionTelefono4 },
                        new TelefonoDTO { Numero = detallesClientes.ce.Telefono5,
                                        Comentario = detallesClientes.ce.ComentarioTelefono5,
                                        DescripcionTipificacion = detallesClientes.ce.UltimaTipificacionTelefono5,
                                        FechaTipificacion = detallesClientes.ce.FechaUltimaTipificacionTelefono5 }
                        },

                        //Propiedades Tasas y Detalles
                        Oferta12m = detallesClientes.db.Oferta12m,
                        Tasa12m = detallesClientes.db.Tasa12m,
                        Cuota12m = detallesClientes.db.Cuota12m,
                        Oferta18m = detallesClientes.db.Oferta18m,
                        Tasa18m = detallesClientes.db.Tasa18m,
                        Cuota18m = detallesClientes.db.Cuota18m,
                        Oferta24m = detallesClientes.db.Oferta24m,
                        Tasa24m = detallesClientes.db.Tasa24m,
                        Cuota24m = detallesClientes.db.Cuota24m,
                        Oferta36m = detallesClientes.db.Oferta36m,
                        Tasa36m = detallesClientes.db.Tasa36m,
                        Cuota36m = detallesClientes.db.Cuota36m,
                        Usuario = detallesClientes.db.Usuario,
                        SegmentoUser = detallesClientes.db.SegmentoUser,
                    }
                    : null;

                if (detalleTipificarCliente == null)
                {
                    return Json(new { success = false, message = "No se encontró el cliente en la base de datos." });
                }

                var tipificaciones = _context.tipificaciones.Select(t => new { t.IdTipificacion, t.DescripcionTipificacion }).ToList();
                ViewData["Tipificaciones"] = tipificaciones;

                var resultados_telefonos_tipificados_vendedor = (from ta in _context.telefonos_agregados
                                                                 where detallesClientes != null && ta.IdCliente == detallesClientes.ca.IdCliente
                                                                 select new
                                                                 {
                                                                     TelefonoTipificado = ta.Telefono,
                                                                     ComentarioTelefono = ta.Comentario,
                                                                     DescripcionTipificacion = ta.UltimaTipificacion,
                                                                     FechaTipificacionSup = ta.FechaUltimaTipificacion
                                                                 }).ToList();
                var agenciasDisponibles = (from db in _context.detalle_base
                                           where !string.IsNullOrEmpty(db.SucursalComercial)
                                                   && !string.IsNullOrEmpty(db.AgenciaComercial)
                                                   && db.AgenciaComercial != "None"
                                                   && db.AgenciaComercial != "NULL"
                                                   && db.AgenciaComercial != ""
                                           select new
                                           {
                                               numSucursal = db.SucursalComercial,
                                               agenciaComercial = db.AgenciaComercial
                                           })
                                                    .Distinct()
                                                    .ToList();

                var dni = _context.base_clientes.FirstOrDefault(bc => bc.IdBase == id_base);
                ViewData["AgenciasDisponibles"] = agenciasDisponibles;
                ViewData["numerosCreadosPorElUsuario"] = resultados_telefonos_tipificados_vendedor;
                ViewData["DNIcliente"] = dni != null ? dni.Dni : "El usuario No tiene DNI Registrado";
                ViewData["ID_asignacion"] = detallesClientes?.ca?.IdAsignacion ?? 0;
                ViewData["Fuente_BD"] = detallesClientes?.ca?.FuenteBase ?? "Fuente no disponible";
                ViewData["ID_cliente"] = detallesClientes?.ca?.IdCliente ?? 0;

                return PartialView("_Tipificarcliente", detalleTipificarCliente);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult TipificarClienteDBALFINView(int id_base)
        {
            try
            {
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                {
                    return Json(new { success = false, message = "No ha iniciado sesion, por favor inicie sesion." });
                }
                var detallesClientes = (from bc in _context.base_clientes
                                        join bcb in _context.base_clientes_banco on bc.IdBaseBanco equals bcb.IdBaseBanco
                                        join bcg in _context.base_clientes_banco_campana_grupo on bcb.IdCampanaGrupoBanco equals bcg.IdCampanaGrupo into bcgGroup
                                        from bcg in bcgGroup.DefaultIfEmpty()
                                        join bcc in _context.base_clientes_banco_color on bcb.IdColorBanco equals bcc.IdColor into bccGroup
                                        from bcc in bccGroup.DefaultIfEmpty()
                                        join bcp in _context.base_clientes_banco_plazo on bcb.IdPlazoBanco equals bcp.IdPlazo into bcpGroup
                                        from bcp in bcpGroup.DefaultIfEmpty()
                                        join bcrd in _context.base_clientes_banco_rango_deuda on bcb.IdRangoDeuda equals bcrd.IdRangoDeuda into bcrdGroup
                                        from bcrd in bcrdGroup.DefaultIfEmpty()
                                        join bcu in _context.base_clientes_banco_usuario on bcb.IdUsuarioBanco equals bcu.IdUsuario into bcuGroup
                                        from bcu in bcuGroup.DefaultIfEmpty()
                                        join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                        join ca in _context.clientes_asignados on ce.IdCliente equals ca.IdCliente
                                        where bc.IdBase == id_base
                                            && bc.IdBaseBanco != null
                                            && ca.FechaAsignacionVendedor.HasValue
                                            && ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                            && ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month
                                            && ca.IdUsuarioV == usuarioId
                                        select new
                                        {
                                            bc,
                                            bcb,
                                            bcg,
                                            bcc,
                                            bcp,
                                            bcrd,
                                            bcu,
                                            ce,
                                            ca
                                        }).FirstOrDefault();

                if (detallesClientes == null)
                {
                    return Json(new { success = false, message = "No se encontró el cliente en la base de datos." });
                }

                var detalleTipificarCliente = new DetalleTipificarClienteDTO
                {
                    // Propiedades de BaseCliente
                    Dni = detallesClientes.bc.Dni,
                    XAppaterno = detallesClientes.bc.XAppaterno,
                    XApmaterno = detallesClientes.bc.XApmaterno,
                    XNombre = detallesClientes.bc.XNombre,
                    Edad = 0,
                    Departamento = "DESCONOCIDO",
                    Provincia = "DESCONOCIDO",
                    Distrito = "DESCONOCIDO",
                    IdBase = detallesClientes.bc.IdBase,

                    // Propiedades de DetalleBase
                    Campaña = detallesClientes.bcg?.NombreCampana ?? "DESCONOCIDO",
                    OfertaMax = detallesClientes.bcb?.OfertaMax * 100 ?? 0,
                    TasaMinima = 0,
                    Sucursal = "DESCONOCIDO",
                    AgenciaComercial = detallesClientes.bcb != null ? $"Numero Entidades: {detallesClientes.bcb.NumEntidades}" : "Numero Entidades: Desconocido",
                    Plazo = detallesClientes.bcp.NumMeses,
                    Cuota = 0,
                    GrupoTasa = detallesClientes.bcb?.TasasEspeciales ?? "DESCONOCIDO",
                    GrupoMonto = "DESCONIDO",
                    Propension = 0,
                    TipoCliente = detallesClientes.bcu?.TipoUsuario ?? "DESCONOCIDO",
                    ClienteNuevo = "ANTIGUO",
                    Color = detallesClientes.bcc?.NombreColor ?? "DESCONOCIDO",
                    ColorFinal = detallesClientes.bcc?.NombreColor ?? "DESCONOCIDO",

                    // Propiedades de ClientesEnriquecido
                    Telefonos = new List<TelefonoDTO>
                        {
                            new TelefonoDTO
                            {
                                Numero = detallesClientes.ce?.Telefono1 ?? "0",
                                Comentario = detallesClientes.ce?.ComentarioTelefono1,
                                DescripcionTipificacion = detallesClientes.ce?.UltimaTipificacionTelefono1,
                                FechaTipificacion = detallesClientes.ce?.FechaUltimaTipificacionTelefono1
                            },
                            new TelefonoDTO
                            {
                                Numero = detallesClientes.ce?.Telefono2 ?? "0",
                                Comentario = detallesClientes.ce?.ComentarioTelefono2,
                                DescripcionTipificacion = detallesClientes.ce?.UltimaTipificacionTelefono2,
                                FechaTipificacion = detallesClientes.ce?.FechaUltimaTipificacionTelefono2
                            },
                            new TelefonoDTO
                            {
                                Numero = detallesClientes.ce?.Telefono3 ?? "0",
                                Comentario = detallesClientes.ce?.ComentarioTelefono3,
                                DescripcionTipificacion = detallesClientes.ce?.UltimaTipificacionTelefono3,
                                FechaTipificacion = detallesClientes.ce?.FechaUltimaTipificacionTelefono3
                            },
                            new TelefonoDTO
                            {
                                Numero = detallesClientes.ce?.Telefono4 ?? "0",
                                Comentario = detallesClientes.ce?.ComentarioTelefono4,
                                DescripcionTipificacion = detallesClientes.ce?.UltimaTipificacionTelefono4,
                                FechaTipificacion = detallesClientes.ce?.FechaUltimaTipificacionTelefono4
                            },
                            new TelefonoDTO
                            {
                                Numero = detallesClientes.ce?.Telefono5 ?? "0",
                                Comentario = detallesClientes.ce?.ComentarioTelefono5,
                                DescripcionTipificacion = detallesClientes.ce?.UltimaTipificacionTelefono5,
                                FechaTipificacion = detallesClientes.ce?.FechaUltimaTipificacionTelefono5
                            },
                        },

                    //Propiedades Tasas y Detalles
                    Tasa12m = detallesClientes.bcb?.Tasa1 ?? 0,
                    Tasa18m = detallesClientes.bcb?.Tasa2 ?? 0,
                    Tasa24m = detallesClientes.bcb?.Tasa3 ?? 0,
                    Tasa36m = detallesClientes.bcb?.Tasa4 ?? 0,
                    Usuario = detallesClientes.bcu?.NombreUsuario ?? "DESCONOCIDO",
                    SegmentoUser = detallesClientes.bcu?.TipoUsuario ?? "DESCONOCIDO",
                };

                var tipificaciones = _context.tipificaciones.Select(t => new { t.IdTipificacion, t.DescripcionTipificacion }).ToList();
                ViewData["Tipificaciones"] = tipificaciones;

                var resultados_telefonos_tipificados_vendedor = (from ta in _context.telefonos_agregados
                                                                 where detallesClientes != null && ta.IdCliente == detallesClientes.ca.IdCliente
                                                                 select new
                                                                 {
                                                                     TelefonoTipificado = ta.Telefono,
                                                                     ComentarioTelefono = ta.Comentario,
                                                                     DescripcionTipificacion = ta.UltimaTipificacion,
                                                                     FechaTipificacionSup = ta.FechaUltimaTipificacion
                                                                 }).ToList();
                var agenciasDisponibles = (from db in _context.detalle_base
                                           where !string.IsNullOrEmpty(db.SucursalComercial)
                                               && !string.IsNullOrEmpty(db.AgenciaComercial)
                                               && db.AgenciaComercial != "None"
                                               && db.AgenciaComercial != "NULL"
                                               && db.AgenciaComercial != ""
                                           select new
                                           {
                                               numSucursal = db.SucursalComercial,
                                               agenciaComercial = db.AgenciaComercial
                                           })
                                            .Distinct()
                                            .ToList();

                var dni = _context.base_clientes.FirstOrDefault(bc => bc.IdBase == id_base);
                ViewData["AgenciasDisponibles"] = agenciasDisponibles;
                ViewData["numerosCreadosPorElUsuario"] = resultados_telefonos_tipificados_vendedor;
                ViewData["DNIcliente"] = dni != null ? dni.Dni : "El usuario No tiene DNI Registrado";
                ViewData["ID_asignacion"] = detallesClientes?.ca?.IdAsignacion ?? 0;
                ViewData["Fuente_BD"] = "BASE_ASESORES";
                ViewData["ID_cliente"] = detallesClientes?.ca?.IdCliente ?? 0;

                return PartialView("_Tipificarcliente", detalleTipificarCliente);
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

            var ClienteAsignado = _context.clientes_asignados.FirstOrDefault(ca => ca.IdAsignacion == IdAsignacion && ca.IdUsuarioV == HttpContext.Session.GetInt32("UsuarioId"));

            if (ClienteAsignado == null)
            {
                TempData["MessageError"] = "No tiene permiso para tipificar este cliente";
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

            var ClientesEnriquecido = _context.clientes_enriquecidos.FirstOrDefault(ce => ce.IdCliente == ClienteAsignado.IdCliente);
            if (ClientesEnriquecido == null)
            {
                TempData["MessageError"] = "Cliente enriquecido no encontrado.";
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

                if (tipificacion == 2 && derivacion != null && agenciasComerciales[i] != null)
                {
                    countNonNull++;
                }
                agregado = true;
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

                var tipificacionInfo = _context.tipificaciones
                    .Where(t => t.IdTipificacion == tipificacion.Value)
                    .Select(t => new { t.Peso, t.DescripcionTipificacion })
                    .FirstOrDefault();

                if (tipificacionInfo != null)
                {
                    // Actualizar tipificación de mayor peso si aplica
                    if (tipificacionInfo.Peso > pesoMayor)
                    {
                        pesoMayor = tipificacionInfo.Peso;
                        descripcionTipificacionMayorPeso = tipificacionInfo.DescripcionTipificacion;
                    }
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
                _context.clientes_tipificados.Add(nuevoClienteTipificado);

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
                        ClientesEnriquecido.UltimaTipificacionTelefono1 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.FechaUltimaTipificacionTelefono1 = DateTime.Now;
                        ClientesEnriquecido.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 2:
                        ClientesEnriquecido.UltimaTipificacionTelefono2 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.FechaUltimaTipificacionTelefono2 = DateTime.Now;
                        ClientesEnriquecido.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 3:
                        ClientesEnriquecido.UltimaTipificacionTelefono3 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.FechaUltimaTipificacionTelefono3 = DateTime.Now;
                        ClientesEnriquecido.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 4:
                        ClientesEnriquecido.UltimaTipificacionTelefono4 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.FechaUltimaTipificacionTelefono4 = DateTime.Now;
                        ClientesEnriquecido.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                    case 5:
                        ClientesEnriquecido.UltimaTipificacionTelefono5 = tipificacion_guardada.DescripcionTipificacion;
                        ClientesEnriquecido.FechaUltimaTipificacionTelefono5 = DateTime.Now;
                        ClientesEnriquecido.IdClientetipTelefono1 = nuevoClienteTipificado.IdClientetip;
                        break;
                }
                // Guardar cambios en la base de datos
                _context.clientes_enriquecidos.Update(ClientesEnriquecido);
                await _context.SaveChangesAsync();
                var guardarGestionDetalle = await _dbServicesTipificaciones.GuardarGestionDetalle(ClienteAsignado, nuevoClienteTipificado, ClientesEnriquecido);
                if (!guardarGestionDetalle.IsSuccess)
                {
                    TempData["MessageError"] = guardarGestionDetalle.Message;
                    return RedirectToAction("Inicio");
                }
            }

            if (agregado == false)
            {
                TempData["MessageError"] = "No se ha llenado ningun campo o se han llenado incorrectamente.";
                return RedirectToAction("Inicio");
            }

            if (!string.IsNullOrEmpty(descripcionTipificacionMayorPeso) && pesoMayor > (ClienteAsignado.PesoTipificacionMayor ?? 0))
            {
                ClienteAsignado.TipificacionMayorPeso = descripcionTipificacionMayorPeso; // Almacena la descripción
                ClienteAsignado.PesoTipificacionMayor = pesoMayor; // Almacena el peso
                ClienteAsignado.FechaTipificacionMayorPeso = fechaTipificacion; // Almacena la fecha
                _context.clientes_asignados.Update(ClienteAsignado);
                await _context.SaveChangesAsync();
            }
            string message = "No se han encontrado tipificaciones de Derivacion";
            if (countNonNull >= 1)
            {
                message = "Hay una Tipificacion de Derivacion enviada, el formulario fue enviado correctamente.";
            }

            TempData["Message"] = "Las tipificaciones se han guardado correctamente (Se han Obviado los campos Vacios y los campos que fueron llenados con datos incorrectos)." + message;
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

            if (tipificaciones == null || !tipificaciones.Any())
            {
                TempData["MessageError"] = "No se estan enviando datos para guardar (Comunicarse con Servicio Tecnico).";
                return RedirectToAction("Inicio");
            }

            DateTime fechaTipificacion = DateTime.Now;

            var ClienteAsignado = _context.clientes_asignados.FirstOrDefault(ca => ca.IdAsignacion == IdAsignacionCliente);
            if (ClienteAsignado == null)
            {
                TempData["MessageError"] = "El cliente asignado no ha sido encontrado (Probablemente fue eliminado Manualmente).";
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
                agregado = true;
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
                    FechaTipificacion = fechaTipificacion,
                    Origen = "nuevo",
                    TelefonoTipificado = tipificacion.Telefono,
                    DerivacionFecha = tipificacion.FechaVisita
                };

                _context.clientes_tipificados.Add(nuevaTipificacion);
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
                var telefonoTipificado = _context.telefonos_agregados.FirstOrDefault(ta => ta.Telefono == tipificacion.Telefono && ta.IdCliente == ClienteAsignado.IdCliente);
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
                
                var clienteEnriquecido = _context.clientes_enriquecidos.FirstOrDefault(ce => ce.IdCliente == ClienteAsignado.IdCliente);
                if (clienteEnriquecido == null)
                {
                    TempData["MessageError"] = "El cliente enriquecido no ha sido encontrado.";
                    return RedirectToAction("Inicio");
                }
                var guardarGestionDetalle = await _dbServicesTipificaciones.GuardarGestionDetalle(ClienteAsignado, nuevaTipificacion, clienteEnriquecido);
                if (!guardarGestionDetalle.IsSuccess)
                {
                    TempData["MessageError"] = guardarGestionDetalle.Message;
                    return RedirectToAction("Inicio");
                }
                await _context.SaveChangesAsync();
            }

            if (agregado == false)
            {
                TempData["MessageError"] = "No se ha llenado ningun campo o se han llenado incorrectamente.";
                return RedirectToAction("Inicio");
            }

            else
            {
                if (tipificacionMayorPeso.HasValue && pesoMayor != 0)
                {
                    if (pesoMayor > (ClienteAsignado.PesoTipificacionMayor ?? 0))
                    {
                        ClienteAsignado.TipificacionMayorPeso = descripcionTipificacionMayorPeso;
                        ClienteAsignado.PesoTipificacionMayor = pesoMayor;
                        ClienteAsignado.FechaTipificacionMayorPeso = fechaTipificacion;
                        _context.clientes_asignados.Update(ClienteAsignado);
                    }
                }

                _context.SaveChanges();
                TempData["Message"] = "Las tipificaciones se han guardado correctamente (Se han Obviado los campos Vacios y los campos que fueron llenados con datos incorrectos).";
                return RedirectToAction("Inicio");
            }
        }
    }
}