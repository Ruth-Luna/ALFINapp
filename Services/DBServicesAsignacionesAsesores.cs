using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.EntityFrameworkCore;


namespace ALFINapp.Services
{
    public class DBServicesAsignacionesAsesores
    {
        private readonly MDbContext _context;

        public DBServicesAsignacionesAsesores(MDbContext context)
        {
            _context = context;
        }

        //ACA ESTAN LAS INSERCIONES A LA DB
        public async Task<Tuple<bool, string>> GuardarReAsignacionCliente(string DNIBusqueda, string BaseTipo, int IdUsuarioVAsignar)
        {
            try
            {
                var getBaseCliente = (from bc in _context.base_clientes
                                            join ce in _context.clientes_enriquecidos on bc.IdBase equals ce.IdBase
                                            join ca in _context.clientes_asignados on ce.IdCliente equals ca.IdCliente
                                            where bc.Dni == DNIBusqueda
                                            select new { bc, ce, ca }).FirstOrDefault();

                var getAsesoresSecundarios = (from asa in _context.asesores_secundarios_asignacion
                                                        where asa.id_usuarioV == IdUsuarioVAsignar
                                                        select new { asa }).FirstOrDefault();

                if (getBaseCliente == null )
                {
                    return new Tuple<bool, string>(false, $"El cliente con DNI {DNIBusqueda} no ha podido ser encontrado, problamente fue eliminado");
                }

                if (getBaseCliente.ca.IdUsuarioV == IdUsuarioVAsignar)
                {
                    return new Tuple<bool, string>(false, $"El cliente con DNI {DNIBusqueda} ya lo tiene como asesor principal asignado a usted");
                }

                if (getAsesoresSecundarios != null)
                {
                    return new Tuple<bool, string>(false, $"El cliente con DNI {DNIBusqueda} ya lo tiene como asesor secundario asignado a usted");
                }

                var reasignarCliente = new AsesoresSecundariosAsignacion
                {
                    id_asignacion = getBaseCliente.ca.IdAsignacion,
                    id_usuarioV = IdUsuarioVAsignar,
                    fecha_asignacion_secundarioV = DateTime.Now,
                    tipo_asesor_secundario = "nuevo",
                    id_cliente = getBaseCliente.ca.IdCliente,
                    fuente_base_asignacion_secundaria = BaseTipo
                };

                _context.asesores_secundarios_asignacion.Add(reasignarCliente);
                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                {
                    return new Tuple<bool, string>(false, "Error al guardar la reasignación del cliente.");
                }

                return new Tuple<bool, string>(true, "La reasignacion del cliente se produjo con exito");
            }
            catch (System.Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
                throw;
            }

        }
    }
}