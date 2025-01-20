using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}