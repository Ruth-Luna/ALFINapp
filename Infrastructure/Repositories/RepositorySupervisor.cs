using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositorySupervisor : IRepositorySupervisor
    {
        private readonly MDbContext _context;
        public RepositorySupervisor(MDbContext context)
        {
            _context = context;
        }
        public async Task<DetallesInicioSupervisorDTO> GetInicioSupervisor(int idUsuario)
        {
            try
            {
                var hoy = DateTime.Now;
                var añoActual = hoy.Year;
                var mesActual = hoy.Month;

                var supervisorData = await _context.supervisor_get_inicio_data.FromSqlRaw(
                    "EXEC dbo.sp_supervisor_get_inicio_data @IdUsuario = {0}", new SqlParameter("@IdUsuario", idUsuario))
                    .ToListAsync();

                if (!supervisorData.Any())
                {
                    Console.WriteLine("No hay clientes asignados al supervisor.");
                    return new DetallesInicioSupervisorDTO 
                    {
                        DetallesClientes = new List<DetallesClienteDTO>()
                    };
                }

                var detallesClientes = supervisorData.Select(cliente => new DetallesClienteDTO(cliente)).ToList();
                var detallesClientesl = new DetallesInicioSupervisorDTO
                {
                    DetallesClientes = detallesClientes
                };
                return detallesClientesl;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new DetallesInicioSupervisorDTO
                {
                    DetallesClientes = new List<DetallesClienteDTO>()
                };
            }
        }
    }
}