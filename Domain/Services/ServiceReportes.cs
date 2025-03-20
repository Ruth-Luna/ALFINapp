using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Domain.Services
{
    public class ServiceReportes
    {
        public async Task<bool> ProcessDerivationsGral ()
        {
            try
            {
                return true;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error al procesar las derivaciones generales: " + ex.Message);
                return false;
            }
        }
    }
}