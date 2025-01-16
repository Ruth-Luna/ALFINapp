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

        public string ConseguirDNIUsuarios(int? IdUsuario)
        {
            try
            {
                var dniAsesor = (from u in _context.usuarios
                                 where u.IdUsuario == IdUsuario
                                 select u.Dni // Cambiado para seleccionar directamente el string
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