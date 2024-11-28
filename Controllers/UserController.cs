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
        public async Task<IActionResult> Ventas()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["SuccessMessage"] = "Algo paso mal en la autenticacion";
                return RedirectToAction("Index", "Home");
            }


            var clientesAsignados = await _context.clientes_asignados
            .Where(ca => ca.IdUsuario == usuarioId)
            .Select(ca => ca.IdCliente)
            .ToListAsync();

            var clientes = await _context.base_clientes
            .Where(bc => clientesAsignados.Contains(bc.IdBase))
            .ToListAsync();

            if (clientes == null)
            {
                return NotFound("El presente usuario no tiene clientes");
            }
            var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);
            ViewData["UsuarioNombre"] = usuario != null ? usuario.NombresCompletos : "Usuario No Encontrado";
            return View("Main", clientes);
        }

        public JsonResult VerificarDNI(string dni)
        {
            var clienteExistente = _context.base_clientes.FirstOrDefault(cliente => cliente.Dni == dni);
            return Json(new { existe = clienteExistente != null });
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

                // 3. Crear y agregar el nuevo cliente enriquecido, usando el IdBase correcto
                ClientesEnriquecido model3 = new ClientesEnriquecido
                {
                    IdBase = idBase,  // Usar el IdBase recién insertado
                    Dni = await _context.usuarios.OrderByDescending(u => u.IdUsuario).Select(u => u.Dni).FirstOrDefaultAsync()
                };
                _context.clientes_enriquecidos.Add(model3);
                await _context.SaveChangesAsync();

                // 4. Obtener el IdCliente del último cliente insertado en la tabla 'clientes_enriquecidos'
                var idCliente = model3.IdBase;

                // 5. Crear y agregar el modelo de 'clientes_asignados'
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

        public IActionResult AddingClient()
        {
            return View("AddingClient");
        }
    }
}
