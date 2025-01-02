using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Filters;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
    [RequireSession]
    public class UserController : Controller
    {
        private readonly MDbContext _context;

        public UserController(MDbContext context)
        {
            _context = context;
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

        // Acción para mostrar la página de ventas
        [HttpGet]
        public async Task<IActionResult> Ventas()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            // Obtener los clientes asignados al usuario
            /*var clientesAsignados = await _context.clientes_asignados
                                            .Where(ca => ca.IdUsuarioV == usuarioId)
                                            .Select(ca => ca.IdCliente)
                                            .ToListAsync();*/

            // Obtener las IdBase correspondientes a los clientes asignados
            /*var clientesGralBase = await (from ce in _context.clientes_enriquecidos
                                          where clientesAsignados.Contains(ce.IdCliente)
                                          select ce.IdBase
                                            ).ToListAsync();*/

            var clientes = await (from bc in _context.base_clientes.AsNoTracking()
                                  join db in _context.detalle_base.AsNoTracking() on bc.IdBase equals db.IdBase
                                  join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                  join ca in _context.clientes_asignados on ce.IdCliente equals ca.IdCliente
                                  where (ca.ClienteDesembolso != true) // Excluimos clientes con ClienteDesembolso == true
                                        && (ca.ClienteRetirado != true)
                                        && (db.TipoBase == ca.FuenteBase) // Filtramos por FuenteBase
                                        && (ca.IdUsuarioV == usuarioId) // Filtramos por IdUsuarioV
                                        && ca.FechaAsignacionVendedor.HasValue
                                        && ca.FechaAsignacionVendedor.Value.Year == 2025
                                        && ca.FechaAsignacionVendedor.Value.Month == 1
                                  group new { db, bc, ca } by db.IdBase into grouped
                                  select new
                                  {
                                      IdBase = grouped.Key, // Este es el valor agrupado (db.IdBase)
                                      LatestRecord = grouped.OrderByDescending(x => x.db.FechaCarga) // Ordenamos por FechaCarga
                                                            .FirstOrDefault() // Obtenemos el primer (más reciente) registro
                                  }).ToListAsync();

            // Mapear los resultados a DTO
            var detallesClientes = clientes.Select(cliente => new DetalleBaseClienteDTO
            {
                Dni = cliente.LatestRecord?.bc.Dni ?? "",  // Default value in case of null
                XAppaterno = cliente.LatestRecord?.bc.XAppaterno ?? "",
                XApmaterno = cliente.LatestRecord?.bc.XApmaterno ?? "",
                XNombre = cliente.LatestRecord?.bc.XNombre ?? "",
                OfertaMax = cliente.LatestRecord?.db.OfertaMax ?? 0, // Default value in case of null
                Campaña = cliente.LatestRecord?.db.Campaña ?? "",  // Default value in case of null
                IdBase = cliente.IdBase,
                IdAsignacion = cliente.LatestRecord?.ca.IdAsignacion,
                FechaAsignacionVendedor = cliente.LatestRecord?.ca.FechaAsignacionVendedor,
                ComentarioGeneral = cliente.LatestRecord.ca.ComentarioGeneral,
                TipificacionDeMayorPeso = cliente.LatestRecord.ca.TipificacionMayorPeso,
                PesoTipificacionMayor = cliente.LatestRecord.ca.PesoTipificacionMayor,
                FechaTipificacionDeMayorPeso = cliente.LatestRecord.ca.FechaTipificacionMayorPeso,
            }).ToList();

            // Total de clientes
            int totalClientes = detallesClientes.Count;

            // Clientes aún no tipificados (su campo 'TipificacionDeMayorPeso' es null o vacío)
            int clientesPendientes = detallesClientes.Count(dc => string.IsNullOrEmpty(dc.TipificacionDeMayorPeso));

            // Clientes ya tipificados (su campo 'TipificacionDeMayorPeso' no es null ni vacío)
            int clientesTipificados = detallesClientes.Count(dc => !string.IsNullOrEmpty(dc.TipificacionDeMayorPeso));

            // Obtener el usuario actual
            var usuario = await _context.usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);

            ViewData["TotalClientes"] = totalClientes;
            ViewData["ClientesPendientes"] = clientesPendientes;
            ViewData["ClientesTipificados"] = clientesTipificados;
            // Asignar el nombre del usuario a la vista
            ViewData["UsuarioNombre"] = usuario != null ? usuario.NombresCompletos : "Usuario No Encontrado";
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
                    return RedirectToAction("Ventas");
                }

                Regex regexNoNumeros = new Regex(@"^\D+$");

                if (!regexNoNumeros.IsMatch(model.XAppaterno))
                {
                    TempData["MessageError"] = "El apellido paterno no debe contener números.";
                    return RedirectToAction("Ventas");
                }

                if (!regexNoNumeros.IsMatch(model.XApmaterno))
                {
                    TempData["MessageError"] = "El apellido materno no debe contener números.";
                    return RedirectToAction("Ventas");
                }

                if (!regexNoNumeros.IsMatch(model.XNombre))
                {
                    TempData["MessageError"] = "El nombre no debe contener números.";
                    return RedirectToAction("Ventas");
                }

                if (!regexNoNumeros.IsMatch(model.Departamento))
                {
                    TempData["MessageError"] = "El departamento no debe contener números.";
                    return RedirectToAction("Ventas");
                }

                // Validar que la edad sea un número positivo.
                if (!int.TryParse(model.Edad.ToString(), out int edad) || edad <= 0)
                {
                    TempData["MessageError"] = "La edad debe ser un número válido y mayor que cero.";
                    return RedirectToAction("Ventas");
                }

                var idSupervisor = (from u in _context.usuarios
                                    where u.IdUsuario == HttpContext.Session.GetInt32("UsuarioId")
                                    select u.IDUSUARIOSUP
                                ).FirstOrDefault();

                if (idSupervisor == null)
                {
                    TempData["MessageError"] = "Usted no tiene un Supervisor Asignado, no puede agregar clientes.";
                    return RedirectToAction("Ventas");
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
                return RedirectToAction("Ventas");
            }
            TempData["MessageError"] = "El modelo enviado no es valido Comunicarse con servicio tecnico.";
            return RedirectToAction("Ventas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TipificarCliente(ClientesEnriquecido model)
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["MessageError"] = "No ha iniciado sesion, por favor inicie sesion.";
                return RedirectToAction("Index", "Home");
            }

            var ClientesAsignado = _context.clientes_asignados.
                                        FirstOrDefault(ca => ca.IdCliente == model.IdCliente &&
                                                        ca.IdUsuarioV == HttpContext.Session.GetInt32("UsuarioId"));
            if (ClientesAsignado == null)
            {
                TempData["MessageError"] = "No tiene permiso para tipificar este cliente (se ha hecho el envio de datos manualmente, esta accion sera notificada).";
                return RedirectToAction("Ventas");
            }
            if (ModelState.IsValid)
            {
                var clienteEnriquecido = await _context.clientes_enriquecidos
                    .FirstOrDefaultAsync(c => c.IdBase == model.IdBase);
                if (clienteEnriquecido != null)
                {
                    clienteEnriquecido.Telefono1 = model.Telefono1;
                    clienteEnriquecido.Telefono2 = model.Telefono2;
                    clienteEnriquecido.Telefono3 = model.Telefono3;
                    clienteEnriquecido.Telefono4 = model.Telefono4;
                    clienteEnriquecido.Telefono5 = model.Telefono5;
                    clienteEnriquecido.Email1 = model.Email1;
                    clienteEnriquecido.Email2 = model.Email2;
                    clienteEnriquecido.FechaEnriquecimiento = model.FechaEnriquecimiento;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    TempData["MessageError"] = "El cliente enriquecido no se ha encontrado.";
                    return RedirectToAction("Ventas");
                }
                TempData["Message"] = "Cliente actualizado con exito!";
                return RedirectToAction("Ventas");
            }
            return RedirectToAction("Ventas");
        }

        [HttpGet]
        public IActionResult TipificarClienteView(int id_base)
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["MessageError"] = "No ha iniciado sesion, por favor inicie sesion.";
                return RedirectToAction("Index", "Home");
            }

            Console.WriteLine($"DNI {id_base}");

            var detallesClientes = (from bc in _context.base_clientes
                                    join db in _context.detalle_base on bc.IdBase equals db.IdBase
                                    join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                    join ca in _context.clientes_asignados on ce.IdCliente equals ca.IdCliente

                                    where bc.IdBase == id_base && db.TipoBase == ca.FuenteBase &&
                                          ca.IdUsuarioV == HttpContext.Session.GetInt32("UsuarioId")
                                    group new { bc, db, ce } by db.IdBase into grouped
                                    select new
                                    {
                                        // Propiedades de BaseCliente
                                        IdBase = grouped.Key,
                                        LatestRecord = grouped.FirstOrDefault(),
                                    }).FirstOrDefault();

            var detalleTipificarCliente = detallesClientes?.LatestRecord != null
                ? new DetalleTipificarClienteDTO
                {
                    // Propiedades de BaseCliente
                    Dni = detallesClientes.LatestRecord.bc.Dni,
                    XAppaterno = detallesClientes.LatestRecord.bc.XAppaterno,
                    XApmaterno = detallesClientes.LatestRecord.bc.XApmaterno,
                    XNombre = detallesClientes.LatestRecord.bc.XNombre,
                    Edad = detallesClientes.LatestRecord.bc.Edad,
                    Departamento = detallesClientes.LatestRecord.bc.Departamento,
                    Provincia = detallesClientes.LatestRecord.bc.Provincia,
                    Distrito = detallesClientes.LatestRecord.bc.Distrito,
                    IdBase = detallesClientes.LatestRecord.bc.IdBase,

                    // Propiedades de DetalleBase
                    Campaña = detallesClientes.LatestRecord.db.Campaña,
                    OfertaMax = detallesClientes.LatestRecord.db.OfertaMax,
                    TasaMinima = detallesClientes.LatestRecord.db.TasaMinima,
                    Sucursal = detallesClientes.LatestRecord.db.Sucursal,
                    AgenciaComercial = detallesClientes.LatestRecord.db.AgenciaComercial,
                    Plazo = detallesClientes.LatestRecord.db.Plazo,
                    Cuota = detallesClientes.LatestRecord.db.Cuota,
                    GrupoTasa = detallesClientes.LatestRecord.db.GrupoTasa,
                    GrupoMonto = detallesClientes.LatestRecord.db.GrupoMonto,
                    Propension = detallesClientes.LatestRecord.db.Propension,
                    TipoCliente = detallesClientes.LatestRecord.db.TipoCliente,
                    ClienteNuevo = detallesClientes.LatestRecord.db.ClienteNuevo,
                    Color = detallesClientes.LatestRecord.db.Color,
                    ColorFinal = detallesClientes.LatestRecord.db.ColorFinal,

                    // Propiedades de ClientesEnriquecido
                    Telefono1 = detallesClientes.LatestRecord.ce.Telefono1,
                    Telefono2 = detallesClientes.LatestRecord.ce.Telefono2,
                    Telefono3 = detallesClientes.LatestRecord.ce.Telefono3,
                    Telefono4 = detallesClientes.LatestRecord.ce.Telefono4,
                    Telefono5 = detallesClientes.LatestRecord.ce.Telefono5,
                    ComentarioTelefono1 = detallesClientes.LatestRecord.ce.ComentarioTelefono1,
                    ComentarioTelefono2 = detallesClientes.LatestRecord.ce.ComentarioTelefono2,
                    ComentarioTelefono3 = detallesClientes.LatestRecord.ce.ComentarioTelefono3,
                    ComentarioTelefono4 = detallesClientes.LatestRecord.ce.ComentarioTelefono4,
                    ComentarioTelefono5 = detallesClientes.LatestRecord.ce.ComentarioTelefono5,

                    //Propiedades Tasas y Detalles
                    Oferta12m = detallesClientes.LatestRecord.db.Oferta12m,
                    Tasa12m = detallesClientes.LatestRecord.db.Tasa12m,
                    Cuota12m = detallesClientes.LatestRecord.db.Cuota12m,
                    Oferta18m = detallesClientes.LatestRecord.db.Oferta18m,
                    Tasa18m = detallesClientes.LatestRecord.db.Tasa18m,
                    Cuota18m = detallesClientes.LatestRecord.db.Cuota18m,
                    Oferta24m = detallesClientes.LatestRecord.db.Oferta24m,
                    Tasa24m = detallesClientes.LatestRecord.db.Tasa24m,
                    Cuota24m = detallesClientes.LatestRecord.db.Cuota24m,
                    Oferta36m = detallesClientes.LatestRecord.db.Oferta36m,
                    Tasa36m = detallesClientes.LatestRecord.db.Tasa36m,
                    Cuota36m = detallesClientes.LatestRecord.db.Cuota36m,
                }
                : null;

            if (detalleTipificarCliente == null)
            {
                TempData["MessageError"] = "Al cliente no se le permite tipificarlo.";
                Console.WriteLine("El cliente fue Eliminado manualmente de la Tabla clientes_tipificado");
                return RedirectToAction("Ventas");
            }

            var clienteEnriquecidoConsult = _context.clientes_enriquecidos.FirstOrDefault(ce => ce.IdBase == id_base);
            if (clienteEnriquecidoConsult == null)
            {
                TempData["MessageError"] = "No se encontró un cliente enriquecido con los criterios proporcionados.";
                return RedirectToAction("Ventas");
            }

            var ClienteAsignado = _context.clientes_asignados.FirstOrDefault(ca => ca.IdCliente == clienteEnriquecidoConsult.IdCliente && ca.IdUsuarioV == HttpContext.Session.GetInt32("UsuarioId"));

            if (ClienteAsignado == null)
            {
                TempData["MessageError"] = "No tiene permiso para tipificar este cliente (este incidente sera notificado).";
                return RedirectToAction("Ventas");
            }

            var tipificaciones = _context.tipificaciones.Select(t => new { t.IdTipificacion, t.DescripcionTipificacion }).ToList();
            ViewData["Tipificaciones"] = tipificaciones;

            var resultados_telefonos_tipificados_bd = (from ct in _context.clientes_tipificados
                                                       join t in _context.tipificaciones on ct.IdTipificacion equals t.IdTipificacion
                                                       join ca in _context.clientes_asignados on ct.IdAsignacion equals ca.IdAsignacion
                                                       join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                                       join ta in _context.telefonos_agregados.Where(x => x.AgregadoPor != "VENDEDOR")
                                                         on ce.IdCliente equals ta.IdCliente into telefonosAgregados
                                                       from ta in telefonosAgregados.DefaultIfEmpty()
                                                       where ct.IdAsignacion == ClienteAsignado.IdAsignacion &&
                                                             ta.IdCliente == null &&
                                                             ct.FechaTipificacion == (
                                                                 from c in _context.clientes_tipificados
                                                                 where c.TelefonoTipificado == ct.TelefonoTipificado
                                                                 select c.FechaTipificacion
                                                             ).Max()
                                                       orderby ct.FechaTipificacion descending
                                                       select new
                                                       {
                                                           ct.TelefonoTipificado,
                                                           t.DescripcionTipificacion,
                                                           ct.IdAsignacion
                                                       }).Take(5).ToList();

            var tipificaciones_asignadas = (from t in _context.tipificaciones
                                            join ct in _context.clientes_tipificados on t.IdTipificacion equals ct.IdTipificacion
                                            where ct.IdAsignacion == ClienteAsignado.IdAsignacion
                                            select new
                                            {
                                                t.DescripcionTipificacion,
                                                ct.IdAsignacion
                                            }).ToList();

            var resultados_telefonos_tipificados_vendedor = (from ta in _context.telefonos_agregados
                                                             join ce in _context.clientes_enriquecidos on ta.IdCliente equals ce.IdCliente
                                                             where ce.IdBase == id_base
                                                             join ct in (
                                                                 from ct_sub in _context.clientes_tipificados
                                                                 join ultimas in (
                                                                     from sub_ct in _context.clientes_tipificados
                                                                     group sub_ct by sub_ct.TelefonoTipificado into g
                                                                     select new { TelefonoTipificado = g.Key, UltimaFecha = g.Max(x => x.FechaTipificacion) }
                                                                 ) on ct_sub.TelefonoTipificado equals ultimas.TelefonoTipificado
                                                                 where ct_sub.FechaTipificacion == ultimas.UltimaFecha
                                                                 select new { ct_sub.TelefonoTipificado, ct_sub.IdTipificacion }
                                                             ) on ta.Telefono equals ct.TelefonoTipificado into ctJoin
                                                             from ct in ctJoin.DefaultIfEmpty()
                                                             join t in _context.tipificaciones on ct.IdTipificacion equals t.IdTipificacion into tJoin
                                                             from t in tJoin.DefaultIfEmpty()
                                                             select new
                                                             {
                                                                 TelefonoTipificado = ta.Telefono,
                                                                 DescripcionTipificacion = t.DescripcionTipificacion,
                                                                 ComentarioTelefono = ta.Comentario
                                                             }).ToList();
            var dni = _context.base_clientes.FirstOrDefault(bc => bc.IdBase == id_base);
            ViewData["numerosCreadosPorElUsuario"] = resultados_telefonos_tipificados_vendedor;
            ViewData["DNIcliente"] = dni != null ? dni.Dni : "El usuario No tiene DNI Registrado";
            ViewData["ID_asignacion"] = ClienteAsignado.IdAsignacion;
            ViewData["Fuente_BD"] = ClienteAsignado.FuenteBase;
            ViewData["ID_cliente"] = ClienteAsignado.IdCliente;
            ViewData["numerosTraidosPorlaDB"] = resultados_telefonos_tipificados_bd;

            return PartialView("_Tipificarcliente", detalleTipificarCliente);
        }

        [HttpGet]
        public IActionResult AddingClient()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["Message"] = "No ha iniciado sesion, por favor inicie sesion.";
                return RedirectToAction("Index", "Home");
            }
            Console.WriteLine("Cargando VISTA");
            return PartialView("_AddingClient");
        }

        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult TipificarMotivo(int IdAsignacion, int? Tipificacion1, int? Tipificacion2,
                                                int? Tipificacion3, int? Tipificacion4, int? Tipificacion5,
                                                string? Telefono1, string? Telefono2, string? Telefono3,
                                                string? Telefono4, string? Telefono5, DateTime? DerivacionDb1,
                                                DateTime? DerivacionDb2, DateTime? DerivacionDb3, DateTime? DerivacionDb4,
                                                DateTime? DerivacionDb5)
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
                return RedirectToAction("Ventas");
            }

            var telefonos = new List<string?> { Telefono1, Telefono2, Telefono3, Telefono4, Telefono5 };
            var tipificaciones = new List<int?> { Tipificacion1, Tipificacion2, Tipificacion3, Tipificacion4, Tipificacion5 };
            var derivaciones = new List<DateTime?> { DerivacionDb1, DerivacionDb2, DerivacionDb3, DerivacionDb4, DerivacionDb5 };

            // Fecha de tipificación
            var fechaTipificacion = DateTime.Now;
            string? descripcionTipificacionMayorPeso = null;
            int? pesoMayor = 0;

            var ClientesEnriquecido = _context.clientes_enriquecidos.FirstOrDefault(ce => ce.IdCliente == ClienteAsignado.IdCliente);
            if (ClientesEnriquecido == null)
            {
                TempData["MessageError"] = "Cliente enriquecido no encontrado.";
                return RedirectToAction("Ventas");
            }
            var agregado = false;

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
                    return RedirectToAction("Ventas");
                }

                if (derivacion == null && tipificacion == 2)
                {
                    TempData["MessageError"] = "Debe ingresar una fecha de derivación para la tipificación CLIENTE ACEPTO OFERTA DERIVACION (Se obviaron las inserciones).";
                    return RedirectToAction("Ventas");
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
                switch (i + 1)
                {
                    case 1:
                        ClientesEnriquecido.UltimaTipificacionTelefono1 = tipificacion_guardada.DescripcionTipificacion;
                        break;
                    case 2:
                        ClientesEnriquecido.UltimaTipificacionTelefono2 = tipificacion_guardada.DescripcionTipificacion;
                        break;
                    case 3:
                        ClientesEnriquecido.UltimaTipificacionTelefono3 = tipificacion_guardada.DescripcionTipificacion;
                        break;
                    case 4:
                        ClientesEnriquecido.UltimaTipificacionTelefono4 = tipificacion_guardada.DescripcionTipificacion;
                        break;
                    case 5:
                        ClientesEnriquecido.UltimaTipificacionTelefono5 = tipificacion_guardada.DescripcionTipificacion;
                        break;
                }

            }

            if (agregado == false)
            {
                TempData["MessageError"] = "No se ha llenado ningun campo o se han llenado incorrectamente.";
                return RedirectToAction("Ventas");
            }

            if (!string.IsNullOrEmpty(descripcionTipificacionMayorPeso) && pesoMayor > (ClienteAsignado.PesoTipificacionMayor ?? 0))
            {
                ClienteAsignado.TipificacionMayorPeso = descripcionTipificacionMayorPeso; // Almacena la descripción
                ClienteAsignado.PesoTipificacionMayor = pesoMayor; // Almacena el peso
                ClienteAsignado.FechaTipificacionMayorPeso = fechaTipificacion; // Almacena la fecha
                _context.clientes_asignados.Update(ClienteAsignado);
            }

            // Guardar cambios en la base de datos
            _context.clientes_enriquecidos.Update(ClientesEnriquecido);
            _context.SaveChanges();
            TempData["Message"] = "Las tipificaciones se han guardado correctamente (Se han Obviado los campos Vacios y los campos que fueron llenados con datos incorrectos)";
            return RedirectToAction("Ventas");
        }

        /*[HttpPost]
        public IActionResult AgregarTelefono(int IdAsignacion, string NuevoTelefono, string NumTelefonoEdicion)
        {
            Console.WriteLine($"ID Asignacion: {IdAsignacion}, Telefono Nuevo {NuevoTelefono}, Edicion de Telefono {NumTelefonoEdicion}");
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            var clienteAsignadoConsulta = _context.clientes_asignados.FirstOrDefault(ca => ca.IdAsignacion == IdAsignacion && ca.IdUsuarioV == HttpContext.Session.GetInt32("UsuarioId"));
            if (clienteAsignadoConsulta == null)
            {
                TempData["MessageError"] = "Usted no tiene acceso a este Usuario.";
                return RedirectToAction("Ventas");
            }
            var ClientesEnriquecido = _context.clientes_enriquecidos.FirstOrDefault(ce => ce.IdCliente == clienteAsignadoConsulta.IdCliente);
            if (ClientesEnriquecido == null)
            {
                TempData["MessageError"] = "No se encuentra un Cliente enriquecido con los datos proporcionados.";
                return RedirectToAction("Ventas");
            }

            if (NumTelefonoEdicion == "NuevoTelefono1")
            {
                ClientesEnriquecido.Telefono1 = NuevoTelefono;
            }

            if (NumTelefonoEdicion == "NuevoTelefono2")
            {
                ClientesEnriquecido.Telefono2 = NuevoTelefono;
            }

            if (NumTelefonoEdicion == "NuevoTelefono3")
            {
                ClientesEnriquecido.Telefono3 = NuevoTelefono;
            }

            if (NumTelefonoEdicion == "NuevoTelefono4")
            {
                ClientesEnriquecido.Telefono4 = NuevoTelefono;
            }

            if (NumTelefonoEdicion == "NuevoTelefono5")
            {
                ClientesEnriquecido.Telefono5 = NuevoTelefono;
            }

            _context.SaveChanges();
            return Json(new { success = true });
        }*/

        /*[HttpPost]
        public IActionResult culminarCliente(int IdAsignacion)
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
                TempData["MessageError"] = "Usted no tiene acceso a este Usuario para su modificacion";
                return RedirectToAction("Index", "Home");
            }
            ClienteAsignado.FinalizarTipificacion = true;
            _context.SaveChanges();

            TempData["Message"] = "Usted a Finalizado con las tipificaciones del Usuario";
            return RedirectToAction("Ventas");
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
                return Json(new { error = false });
                throw;
            }
        }

        [HttpPost]
        public IActionResult GuardarTipificacionesNumPersonales(List<TipificarClienteDTO> tipificaciones, int IdAsignacionCliente)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            
            if (tipificaciones == null || !tipificaciones.Any())
            {
                TempData["MessageError"] = "No se estan enviando datos para guardar (Comunicarse con Servicio Tecnico).";
                return RedirectToAction("Ventas");
            }

            DateTime fechaTipificacion = DateTime.Now;

            var ClienteAsignado = _context.clientes_asignados.FirstOrDefault(ca => ca.IdAsignacion == IdAsignacionCliente);
            if (ClienteAsignado == null)
            {
                TempData["MessageError"] = "El cliente asignado no ha sido encontrado (Probablemente fue eliminado Manualmente).";
                return RedirectToAction("Ventas");
            }

            int? tipificacionMayorPeso = null;
            int? pesoMayor = 0;
            string descripcionTipificacionMayorPeso = null;
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
                    return RedirectToAction("Ventas");
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
                    return RedirectToAction("Ventas");
                }
                else
                {
                    var tipificacionUltima = _context.tipificaciones.FirstOrDefault(t => t.IdTipificacion == tipificacion.TipificacionId);
                    telefonoTipificado.UltimaTipificacion = tipificacionUltima.DescripcionTipificacion;
                    _context.telefonos_agregados.Update(telefonoTipificado);
                }
            }

            if (agregado == false)
            {
                TempData["MessageError"] = "No se ha llenado ningun campo o se han llenado incorrectamente.";
                return RedirectToAction("Ventas");
            }

            else
            {
                if (tipificacionMayorPeso.HasValue && pesoMayor!=0)
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
                return RedirectToAction("Ventas");
            }
        }
    }
}