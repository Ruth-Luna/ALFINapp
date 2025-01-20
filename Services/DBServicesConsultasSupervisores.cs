using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.EntityFrameworkCore; // Assuming 'Usuario' is defined in the Models namespace

namespace ALFINapp.Services
{

    public class DBServicesConsultasSupervisores
    {
        private readonly MDbContext _context;
        public DBServicesConsultasSupervisores (MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool IsSuccess, string Message, List<Usuario>? Data)> GetAsesorsFromSupervisor(int? IdSupervisor)
        {
            try
            {
                var asesores = await _context.usuarios
                                    .Where(u => u.IDUSUARIOSUP == IdSupervisor)
                                    .ToListAsync();
                if (asesores.Count == 0)
                {
                    return (false, "No hay asesores registrados para este supervisor", null);
                }
                return (true, "Los asesores registrados al supervisor han sido correctamente enviados", asesores);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los asesores: {ex.Message}", null);
            }
        }

        public async Task<(bool IsSuccess, string Message, VendedorConClientesDTO? Data)> GetNumberTipificaciones(Usuario AsesorBusqueda, int IdSupervisor)
        {
            try
            {
                var numeroClientes = _context.numeros_enteros_dto
                                                .FromSqlRaw("EXEC SP_CONSEGUIR_NUM_CLIENTES_POR_ASESOR @AsesorId = {0}, @SupervisorId = {1}", 
                                                AsesorBusqueda.IdUsuario, IdSupervisor)
                                                .AsEnumerable() // Trae los resultados a memoria.
                                                .FirstOrDefault();

                if (numeroClientes == null)
                {
                    return (false, "El id del asesor es incorrecto.", null);
                }

                VendedorConClientesDTO vendedorClientesDTO = new VendedorConClientesDTO
                {
                    NombresCompletos = AsesorBusqueda.NombresCompletos,
                    IdUsuario = AsesorBusqueda.IdUsuario,
                    NumeroClientes = numeroClientes.NumeroEntero // Asumiendo que el campo en el DTO es 'NumeroClientes'
                };
                return (true, $"La Consulta se produjo con exito", vendedorClientesDTO);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los asesores: {ex.Message}", null);
            }
        }

        public async Task<(bool IsSuccess, string Message, List<string>? Data)> GetBasesClientes(int SupervisorId)
        {
            try
            {
                var BasesAsignadas = await (from ca in _context.clientes_asignados
                                                where ca.IdUsuarioS == SupervisorId
                                                    && ca.FechaAsignacionSup.HasValue
                                                    && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                                    && ca.FechaAsignacionSup.Value.Month == DateTime.Now.Month
                                                select new { ca.FuenteBase })
                                                .Distinct()
                                                .ToListAsync();

                if (BasesAsignadas == null)
                {
                    return (false, "Ocurrió un error al obtener la base de los Asesores", null);
                }

                var BasesAsignadasMapeadas = new List<string>();
                foreach (var BaseAsignada in BasesAsignadas)
                {
                    BasesAsignadasMapeadas.Add(BaseAsignada.FuenteBase);
                }
                return (true, $"Ocurrió un error al obtener los asesores", BasesAsignadasMapeadas);
            }
            catch (System.Exception ex)
            {
                return (false, $"Ocurrió un error al obtener los asesores: {ex.Message}", null);
            }
        }
    }
}