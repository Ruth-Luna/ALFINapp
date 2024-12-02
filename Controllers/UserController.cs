using System.Security;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
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

                model.Nombres = model.NombresCompletos;
                _context.usuarios.Add(model);
                TempData["SuccessMessage"] = "Usuario agregado con exito";
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }

        // Acción para verificar si el DNI existe
        [HttpPost]
        public async Task<IActionResult> VerificarUsuario(string dni)
        {
            if (!dni.All(char.IsDigit))
            {
                return BadRequest("Ingrese solo números");
            }

            var usuario = await _context.usuarios.FirstOrDefaultAsync(usuario => usuario.Dni == dni);

            if (usuario == null)
            {
                return BadRequest("Usuario no encontrado");
            }

            HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
            return RedirectToAction("Ventas");
        }

        // Acción para mostrar la página de ventas
        [HttpGet]
        public async Task<IActionResult> Ventas()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["ErrorMessage"] = "Ha ocurrido un error en la autenticacion";
                return RedirectToAction("Index", "Home");
            }

            var clientesAsignados = await _context.clientes_asignados
            .Where(ca => ca.IdUsuario == usuarioId)
            .Select(ca => ca.IdCliente)
            .ToListAsync();

            var clientesGralBase = await (
            from ce in _context.clientes_enriquecidos
            where clientesAsignados.Contains(ce.IdCliente)
            select ce.IdBase
            ).ToListAsync();

            var clientes = await (from db in _context.detalle_base
                                  join cg in clientesGralBase
                                  on db.IdBase equals cg
                                  join bc in _context.base_clientes
                                  on db.IdBase equals bc.IdBase
                                  select new DetalleBaseClienteDTO
                                  {
                                      Dni = bc.Dni,
                                      XAppaterno = bc.XAppaterno,
                                      XApmaterno = bc.XApmaterno,
                                      XNombre = bc.XNombre,

                                      OfertaMax = db.OfertaMax,
                                      Campaña = db.Campaña,

                                      IdBase = bc.IdBase
                                  })
                                  .Distinct()
                                  .ToListAsync();

            if (clientes == null)
            {
                return NotFound("El presente usuario no tiene clientes");
            }
            var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);



            ViewData["UsuarioNombre"] = usuario != null ? usuario.NombresCompletos : "Usuario No Encontrado";
            return View("Main", clientes);
        }

        [HttpGet]
        public JsonResult VerificarDNI(string dni)
        {
            try
            {
                Console.WriteLine($"DNI recibido: {dni}");
                var clienteExistente = _context.base_clientes.FirstOrDefault(c => c.Dni == dni);
                return Json(new { existe = clienteExistente != null });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar el DNI: {ex.Message}");
                return Json(new { existe = false, error = true, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarCliente(BaseCliente model)
        {
            if (ModelState.IsValid)
            {
                _context.base_clientes.Add(model);
                await _context.SaveChangesAsync();

                var idBase = model.IdBase;

                ClientesEnriquecido model3 = new ClientesEnriquecido
                {
                    IdBase = idBase,
                };
                _context.clientes_enriquecidos.Add(model3);
                await _context.SaveChangesAsync();

                var idCliente = model3.IdBase;

                ClientesAsignado model2 = new ClientesAsignado
                {
                    IdCliente = idCliente,  // Usar el IdCliente recién insertado
                    IdUsuario = HttpContext.Session.GetInt32("UsuarioId") ?? 0,
                    FechaAsignacion = DateTime.Now,
                    Origen = null
                };

                _context.clientes_asignados.Add(model2);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cliente agregado con exito!";
                return RedirectToAction("Ventas");
            }
            return RedirectToAction("Ventas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TipificarCliente(ClientesEnriquecido model)
        {
            var ClientesAsignado = _context.clientes_asignados.FirstOrDefault(ca => ca.IdCliente == model.IdCliente && ca.IdUsuario == HttpContext.Session.GetInt32("UsuarioId"));
            if (ClientesAsignado == null)
            {
                TempData["ErrorMessage"] = "No tiene permiso para tipificar este cliente";
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
                    TempData["ErrorMessage"] = "Cliente no encontrado";
                    return RedirectToAction("Ventas");
                }
                TempData["SuccessMessage"] = "Cliente actualizado con éxito!";
                return RedirectToAction("Ventas");
            }
            return RedirectToAction("Ventas");
        }

        [HttpGet]
        public IActionResult TipificarClienteView(int id_base)
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["ErrorMessage"] = "No ha iniciado sesion";
                return RedirectToAction("Index", "Home");
            }

            Console.WriteLine($"DNI {id_base}");

            var detalleTipificarCliente = (from baseCliente in _context.base_clientes
                                           join detalleBase in _context.detalle_base
                                           on baseCliente.IdBase equals detalleBase.IdBase
                                           join clienteEnriquecido in _context.clientes_enriquecidos
                                           on baseCliente.IdBase equals clienteEnriquecido.IdBase
                                           where baseCliente.IdBase == id_base
                                           select new DetalleTipificarClienteDTO
                                           {
                                               // Propiedades de BaseCliente
                                               Dni = baseCliente.Dni,
                                               XAppaterno = baseCliente.XAppaterno,
                                               XApmaterno = baseCliente.XApmaterno,
                                               XNombre = baseCliente.XNombre,
                                               Edad = baseCliente.Edad,
                                               Departamento = baseCliente.Departamento,
                                               Provincia = baseCliente.Provincia,
                                               Distrito = baseCliente.Distrito,

                                               // Propiedades de DetalleBase
                                               Campaña = detalleBase.Campaña,
                                               OfertaMax = detalleBase.OfertaMax,
                                               TasaMinima = detalleBase.TasaMinima,
                                               Sucursal = detalleBase.Sucursal,
                                               AgenciaComercial = detalleBase.AgenciaComercial,
                                               Plazo = detalleBase.Plazo,
                                               Cuota = detalleBase.Cuota,
                                               GrupoTasa = detalleBase.GrupoTasa,
                                               GrupoMonto = detalleBase.GrupoMonto,
                                               Propension = detalleBase.Propension,
                                               TipoCliente = detalleBase.TipoCliente,
                                               ClienteNuevo = detalleBase.ClienteNuevo,

                                               // Propiedades de ClientesEnriquecido
                                               Telefono1 = clienteEnriquecido.Telefono1,
                                               Telefono2 = clienteEnriquecido.Telefono2,
                                               Telefono3 = clienteEnriquecido.Telefono3,
                                               Telefono4 = clienteEnriquecido.Telefono4,
                                               Telefono5 = clienteEnriquecido.Telefono5
                                           }).FirstOrDefault();

            if (detalleTipificarCliente == null)
            {
                TempData["ErrorMessage"] = "El Cliente no se le permite ser Tipificado";
                Console.WriteLine("El cliente fue Eliminado manualmente de la Tabla clientes_tipificado");
                return RedirectToAction("Ventas");
            }

            var clienteEnriquecidoConsult = _context.clientes_enriquecidos.FirstOrDefault(ce => ce.IdBase == id_base);
            if (clienteEnriquecidoConsult == null)
            {
                TempData["ErrorMessage"] = "No se encontró un cliente enriquecido con los criterios proporcionados.";
                return RedirectToAction("Ventas");
            }

            var ClienteAsignado = _context.clientes_asignados.FirstOrDefault(ca => ca.IdCliente == clienteEnriquecidoConsult.IdCliente && ca.IdUsuario == HttpContext.Session.GetInt32("UsuarioId"));

            if (ClienteAsignado == null)
            {
                TempData["ErrorMessage"] = "No tiene permiso para tipificar este cliente";
                return RedirectToAction("Ventas");
            }

            ViewData["ID_asignacion"] = ClienteAsignado.IdAsignacion;

            var tipificaciones = _context.tipificaciones.Select(t => new { t.IdTipificacion, t.DescripcionTipificacion }).ToList();
            ViewData["Tipificaciones"] = tipificaciones;

            var tipificaciones_asignadas = (from t in _context.tipificaciones
                                            join ct in _context.clientes_tipificados on t.IdTipificacion equals ct.IdTipificacion
                                            where ct.IdAsignacion == ClienteAsignado.IdAsignacion
                                            select new
                                            {
                                                t.DescripcionTipificacion,
                                                ct.IdAsignacion
                                            }).ToList();
            ViewData["tipificaciones_asignadas"] = tipificaciones_asignadas.ToList();
            var dni = _context.base_clientes.FirstOrDefault(bc => bc.IdBase == id_base);
            ViewData["DNIcliente"] = dni != null ? dni.Dni : "El usuario No tiene DNI Registrado";
            return PartialView("_Tipificarcliente", detalleTipificarCliente);
        }

        [HttpGet]
        public IActionResult AddingClient()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["ErrorMessage"] = "No ha iniciado sesion";
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
        public IActionResult TipificarMotivo(int IdAsignacion, int? Tipificacion1, int? Tipificacion2, int? Tipificacion3, int? Tipificacion4, int? Tipificacion5, int? Telefono1 , int? Telefono2, int? Telefono3, int? Telefono4, int? Telefono5)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["ErrorMessage"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            var ClienteAsignado = _context.clientes_asignados.FirstOrDefault(ca => ca.IdAsignacion == IdAsignacion && ca.IdUsuario == HttpContext.Session.GetInt32("UsuarioId"));

            if (ClienteAsignado == null)
            {
                TempData["ErrorMessage"] = "No tiene permiso para tipificar este cliente";
                Console.WriteLine($"{IdAsignacion}");
                Console.WriteLine($"{Tipificacion1}");
                return RedirectToAction("Ventas");
            }

            var clientesTipificados = new List<ClientesTipificado>();
            DateTime fechaTipificacion = DateTime.Now;
            if (Tipificacion1.HasValue)
            {
                var clienteTipificado = new ClientesTipificado
                {
                    IdAsignacion = IdAsignacion,
                    IdTipificacion = Tipificacion1.Value,
                    FechaTipificacion = fechaTipificacion,
                    Origen = "nuevo",
                    TelefonoTipificado = Telefono1
                };
                clientesTipificados.Add(clienteTipificado);
            }

            // Procesamos la tipificación de Teléfono 2 si se seleccionó una opción
            if (Tipificacion2.HasValue)
            {
                var clienteTipificado = new ClientesTipificado
                {
                    IdAsignacion = IdAsignacion,
                    IdTipificacion = Tipificacion2.Value,
                    FechaTipificacion = fechaTipificacion,
                    Origen = "nuevo",
                    TelefonoTipificado = Telefono2
                };
                clientesTipificados.Add(clienteTipificado);
                Console.WriteLine($"ENTRO A LA LINEA TIPIFICACION 2 {IdAsignacion}");
                Console.WriteLine($"{Tipificacion2}");
            }

            // Procesamos la tipificación de Teléfono 3 si se seleccionó una opción
            if (Tipificacion3.HasValue)
            {
                var clienteTipificado = new ClientesTipificado
                {
                    IdAsignacion = IdAsignacion,
                    IdTipificacion = Tipificacion3.Value,
                    FechaTipificacion = fechaTipificacion,
                    Origen = "nuevo",
                    TelefonoTipificado = Telefono3
                };
                clientesTipificados.Add(clienteTipificado);
                Console.WriteLine($"ENTRO A LA LINEA TIPIFICACION 3 {IdAsignacion}");
                Console.WriteLine($"{Tipificacion3}");
            }
            if (Tipificacion4.HasValue)
            {
                var clienteTipificado = new ClientesTipificado
                {
                    IdAsignacion = IdAsignacion,
                    IdTipificacion = Tipificacion4.Value,
                    FechaTipificacion = fechaTipificacion,
                    Origen = "nuevo",
                    TelefonoTipificado = Telefono4
                };
                clientesTipificados.Add(clienteTipificado);
                Console.WriteLine($"ENTRO A LA LINEA TIPIFICACION 3 {IdAsignacion}");
                Console.WriteLine($"{Tipificacion3}");
            }
            if (Tipificacion5.HasValue)
            {
                var clienteTipificado = new ClientesTipificado
                {
                    IdAsignacion = IdAsignacion,
                    IdTipificacion = Tipificacion5.Value,
                    FechaTipificacion = fechaTipificacion,
                    Origen = "nuevo",
                    TelefonoTipificado = Telefono5
                };
                clientesTipificados.Add(clienteTipificado);
                Console.WriteLine($"ENTRO A LA LINEA TIPIFICACION 3 {IdAsignacion}");
                Console.WriteLine($"{Tipificacion3}");
            }
            _context.clientes_tipificados.AddRange(clientesTipificados);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Las Tipificaciones se han guardado Correctamente";

            return RedirectToAction("Ventas");
        }
    }
}
