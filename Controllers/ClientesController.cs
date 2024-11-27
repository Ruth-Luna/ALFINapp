using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Controllers
{
    public class ClientesController : Controller
    {
        private readonly MDbContext _context;

        public ClientesController(MDbContext context)
        {
            _context = context;
        }

        // Acción para verificar si el DNI existe
        [HttpPost]
        public async Task<IActionResult> VerificarUsuario(string dni)
        {
            if (!dni.All(char.IsDigit))
            {
                return BadRequest("Ingrese solo números >:v");
            }

            var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.dni == dni);

            if (usuario.id_usuario == null)
            {
                return BadRequest("Usuario no encontrado");
            }

            return RedirectToAction("Ventas", new { usuarioId = usuario.id_usuario });
        }

        // Acción para mostrar la página de ventas
        public async Task<IActionResult> Ventas(int usuarioId)
        {
            var clientesAsignados = await _context.clientes_asignados
            .Where(ca => ca.id_usuario == usuarioId)
            .Select(ca => ca.id_cliente)
            .ToListAsync();

            var clientes = await _context.base_clientes
            .Where(bc => clientesAsignados.Contains(bc.id_base))
            .ToListAsync();

            if (clientes == null)
            {
                return NotFound("El presente usuario no tiene clientes");
            }

            return View("Main");
        }
    }
}
