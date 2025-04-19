using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Domain.Interfaces;
using ALFINapp.Infrastructure.Persistence.Procedures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryVendedor : IRepositoryVendedor
    {
        private readonly MDbContext _context;
        public RepositoryVendedor(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message, int Total, int Tipificados, int Pendientes)> GetCantidadClientesGeneralTotalFromVendedor(int IdUsuarioVendedor)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuarioVendedor", IdUsuarioVendedor)
                };
                var getCantidad = await _context
                    .leads_get_clientes_asignados_cantidades
                    .FromSqlRaw("EXECUTE sp_leads_get_clientes_asignados_cantidades @IdUsuarioVendedor",
                    parameters)
                    .ToListAsync();
                var cantidades = getCantidad.FirstOrDefault();
                if (cantidades == null)
                {
                    return (true, "No se encontraron clientes", 0, 0, 0);
                }
                return (true, "Non Implemented", cantidades.Total, cantidades.Gestionados, cantidades.Pendientes);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, "Error al obtener la cantidad de clientes", 0, 0, 0);
            }
        }

        public List<DetalleBaseClienteDTO>? GetClientesAlfinFromVendedor(int IdUsuarioVendedor)
        {
            try
            {
                var clientes = _context
                .inicio_detalles_clientes_from_asesor
                .FromSqlRaw("EXECUTE sp_asesores_get_clientes_alfin_data_for_inicio @IdUsuarioVendedor = {0}",
                new SqlParameter("@IdUsuarioVendedor", IdUsuarioVendedor))
                .ToList(); // Usamos ToList() en lugar de ToListAsync()

                if (clientes.Count == 0)
                {
                    return new List<DetalleBaseClienteDTO>(); // Retorna lista vacía en vez de null
                }

                var dtoEnvio = new List<DetalleBaseClienteDTO>();
                foreach (var cliente in clientes)
                {
                    dtoEnvio.Add(new DetalleBaseClienteDTO(cliente));
                }
                return dtoEnvio;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<List<DetalleBaseClienteDTO>> GetClientesFiltradoPaginadoFromVendedor(
            int IdUsuarioVendedor,
            string? filter,
            string? searchfield,
            int IntervaloInicio,
            int IntervaloFin)
        {
            try
            {
                var filtrosValidos = new HashSet<string> { "nombres", "campana",
                    "oferta", "comentario", "tipificacion", "dni" };

                if (string.IsNullOrWhiteSpace(filter) || !filtrosValidos.Contains(filter))
                {
                    throw new ArgumentException("Filtro no válido");
                }

                var parameters = new[]
                {
                    new SqlParameter("@IdUsuarioVendedor", IdUsuarioVendedor),
                    new SqlParameter("@Search", searchfield),
                    new SqlParameter("@IntervaloInicio", IntervaloInicio),
                    new SqlParameter("@IntervaloFin", IntervaloFin)
                };
#pragma warning disable EF1002 // Risk of vulnerability to SQL injection.
                var getAllBase = await _context
                    .leads_get_clientes_asignados_gestion_leads
                    .FromSqlRaw($"EXECUTE sp_leads_get_clientes_asignados_for_gestion_de_leads_filtro_por_{filter} @IdUsuarioVendedor, @Search, @IntervaloInicio, @IntervaloFin",
                    parameters)
                    .ToListAsync();
#pragma warning restore EF1002 // Risk of vulnerability to SQL injection.
                if (getAllBase.Count == 0)
                {
                    return new List<DetalleBaseClienteDTO>();
                }
                var dtoEnvio = new List<DetalleBaseClienteDTO>();
                foreach (var cliente in getAllBase)
                {
                    dtoEnvio.Add(new DetalleBaseClienteDTO(cliente));
                }
                return dtoEnvio;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<DetalleBaseClienteDTO>();
            }
        }

        public async Task<List<DetalleBaseClienteDTO>> GetClientesGeneralPaginadoFromVendedor(
            int IdUsuarioVendedor,
            int IntervaloInicio,
            int IntervaloFin,
            string? filter,
            string? searchfield,
            string? order,
            bool orderAsc)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdUsuarioVendedor", IdUsuarioVendedor),
                    new SqlParameter("@Search", searchfield),
                    new SqlParameter("@Filter", filter),
                    new SqlParameter("@IntervaloInicio", IntervaloInicio),
                    new SqlParameter("@IntervaloFinal", IntervaloFin),
                    new SqlParameter("@Ordenamiento", order),
                    new SqlParameter("@OrdenamientoAsc", orderAsc)
                };

                var getAllBase = await _context
                    .leads_get_clientes_asignados_gestion_leads
                    .FromSqlRaw("EXECUTE sp_leads_get_clientes_asignados_for_gestion_de_leads_filtro_y_ordenamiento_general @IdUsuarioVendedor, @Search, @Filter, @IntervaloInicio, @IntervaloFinal, @Ordenamiento, @OrdenamientoAsc",
                    parameters)
                    .ToListAsync();
                if (getAllBase.Count == 0)
                {
                    return new List<DetalleBaseClienteDTO>();
                }
                var dtoEnvio = new List<DetalleBaseClienteDTO>();
                foreach (var cliente in getAllBase)
                {
                    dtoEnvio.Add(new DetalleBaseClienteDTO(cliente));
                }
                return dtoEnvio;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<DetalleBaseClienteDTO>();
            }
        }
        public async Task<Persistence.Models.Usuario?> GetVendedor(int IdUsuario)
        {
            try
            {
                var usuario = await _context.usuarios.FirstOrDefaultAsync(u => u.IdUsuario == IdUsuario);
                return usuario;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}