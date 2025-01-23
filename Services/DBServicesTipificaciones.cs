using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ALFINapp.Models;

namespace ALFINapp.Services
{
    public class DBServicesTipificaciones
    {
        private readonly MDbContext _context;

        public DBServicesTipificaciones(MDbContext context)
        {
            _context = context;
        }


        public async Task<(bool IsSuccess, string Message)> EnviarFomularioDerivacion(DerivacionesAsesores derivarCliente)
        {
            try
            {
                if (derivarCliente == null)
                {
                    return (false, "El objeto derivarCliente no puede ser nulo");
                }

                // Agregar el objeto a la base de datos
                _context.derivaciones_asesores.Add(derivarCliente);
                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
                return (true, "El Formulario de Derivacion se ha enviado correctamente");
            }
            catch (System.Exception ex)
            {
                return (true, ex.Message);
            }
        }
        // Other DB services can be added here
    }
}