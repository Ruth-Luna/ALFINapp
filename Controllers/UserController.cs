using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var clientes = await (from bc in _context.base_clientes
                                  join db in _context.detalle_base
                                  on bc.IdBase equals db.IdBase into detalleBaseGroup
                                  from db in detalleBaseGroup.DefaultIfEmpty()
                                  where clientesAsignados.Contains(bc.IdBase)
                                  select new DetalleBaseClienteDTO
                                  {
                                      Dni = bc.Dni,
                                      XAppaterno = bc.XAppaterno,
                                      XApmaterno = bc.XApmaterno,
                                      XNombre = bc.XNombre,
                                      Edad = bc.Edad,

                                      OfertaMax = db.OfertaMax,
                                      Campaña = db.Campaña,
                                      Cuota = db.Cuota,
                                      Oferta12m = db.Oferta12m,
                                      Tasa12m = db.Tasa12m,
                                      Tasa18m = db.Tasa18m,
                                      Cuota18m = db.Cuota18m,
                                      Oferta24m = db.Oferta24m,
                                      Tasa24m = db.Tasa24m,
                                      Cuota24m = db.Cuota24m,
                                      Oferta36m = db.Oferta36m,
                                      Tasa36m = db.Tasa36m,
                                      Cuota36m = db.Cuota36m,

                                      Departamento = bc.Departamento,
                                      Provincia = bc.Provincia,
                                      Distrito = bc.Distrito,

                                      Sucursal = db.Sucursal,
                                      AgenciaComercial = db.AgenciaComercial,
                                      TipoCliente = db.TipoCliente,
                                      ClienteNuevo = db.ClienteNuevo,
                                      GrupoTasa = db.GrupoTasa,
                                      GrupoMonto = db.GrupoMonto,
                                      Propension = db.Propension,
                                      

                                      IdDetalle = db.IdDetalle,
                                      IdBase = bc.IdBase
                                  }).ToListAsync();

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
            var clienteEnriquecido = _context.clientes_enriquecidos.FirstOrDefault(ce => ce.IdBase == id_base);

            if (clienteEnriquecido == null)
            {
                TempData["ErrorMessage"] = "El Cliente no se le permite ser Tipificado";
                Console.WriteLine("El cliente fue Eliminado manualmente de la Tabla clientes_tipificado");
                return RedirectToAction("Ventas");
            }

            var ClientesAsignado = _context.clientes_asignados.FirstOrDefault(ca => ca.IdCliente == clienteEnriquecido.IdCliente && ca.IdUsuario == HttpContext.Session.GetInt32("UsuarioId"));
            if (ClientesAsignado == null)
            {
                TempData["ErrorMessage"] = "No tiene permiso para tipificar este cliente";
                return RedirectToAction("Ventas");
            }

            var dni = _context.base_clientes.FirstOrDefault(bc => bc.IdBase == id_base);

            ViewData["DNIcliente"] = dni != null ? dni.Dni : "El usuario No tiene DNI Registrado";
            return View("Tipificarcliente", clienteEnriquecido);
        }

        public IActionResult AddingClient()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                TempData["ErrorMessage"] = "No ha iniciado sesion";
                return RedirectToAction("Index", "Home");
            }
            return View("AddingClient");
        }

        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
