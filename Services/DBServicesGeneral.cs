using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ALFINapp.Models;

namespace ALFINapp.Services
{
    public class DBServicesGeneral
    {
        private readonly MDbContext _context;

        public DBServicesGeneral (MDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the DNI (Documento Nacional de Identidad) of a user based on their user ID.
        /// </summary>
        /// <param name="IdUsuario">The ID of the user whose DNI is to be retrieved. It can be null.</param>
        /// <returns>
        /// A string representing the DNI of the user if found; otherwise, null.
        /// </returns>
        /// <exception cref="System.Exception">Thrown when an error occurs while retrieving the DNI.</exception>
        public string ConseguirDNIUsuarios(int? IdUsuario)
        {
            try
            {
                var dniAsesor = (from u in _context.usuarios
                                 where u.IdUsuario == IdUsuario
                                 select u.Dni
                                ).FirstOrDefault();
                return dniAsesor;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public async Task<(bool IsSuccess, string Message, Usuario? Data)> GetUserInformation(int IdUsuario)
        {
            try
            {
                var FoundUser = await _context.usuarios
                                    .Where(u => u.IdUsuario == IdUsuario)
                                    .FirstOrDefaultAsync();
                
                if (FoundUser == null)
                {
                    return (false, "El usuario a buscar no se encuentra registrado", null);
                }

                return (true, "Usuario encontrado correctamente", FoundUser);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task <(bool IsSuccess, string Message)> UpdatePasswordGeneralFunction (int IdUsuario, string password)
        {
            try
            {
                var user = await _context.usuarios.Where(u => u.IdUsuario == IdUsuario)
                                    .FirstOrDefaultAsync();
                
                if (user == null)
                {
                    return (false, "El usuario a modificar no se encuentra registrado");
                }
                
                user.contraseña = password;  // Asegúrate de cifrarla si es necesario
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();
                return (true, "La Modificación se realizo con exito");
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message, BaseCliente? data)> GetBaseClienteFunction(int IdBaseB)
        {
            try
            {
                var cliente = await _context.base_clientes.Where(u => u.IdBase == IdBaseB)
                                    .FirstOrDefaultAsync();
                
                if (cliente == null)
                {
                    return (false, "El usuario no se encuentra registrado", null);
                }
                
                return (true, "El Usuario se ha encontrado", cliente);
            }
            catch (System.Exception ex)
            {
                return (true, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message, ClientesEnriquecido? data)> GetClienteEnriquecidoFunction(int IdBaseB = 0)
        {
            try
            {
                var cliente = await _context.clientes_enriquecidos.Where(u => u.IdBase == IdBaseB)
                                    .FirstOrDefaultAsync();
                
                if (cliente == null)
                {
                    return (false, "El usuario no se encuentra registrado, en la tabla enriquecida", null);
                }
                
                return (true, "El Usuario se ha encontrado en la tabla enriquecida", cliente);
            }
            catch (System.Exception ex)
            {
                return (true, ex.Message, null);
            }
        }
        // Other DB services can be added here
    }
}