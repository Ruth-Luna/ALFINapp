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


        public List<DetalleBaseClienteDTO>? GetClientesFromVendedor(int IdUsuarioVendedor)
        {
            try
            {
                var clientes = _context
                .inicio_detalles_clientes_from_asesor
                .FromSqlRaw("EXECUTE sp_asesores_get_clientes_data_for_inicio @IdUsuarioVendedor = {0}",
                new SqlParameter("@IdUsuarioVendedor", IdUsuarioVendedor))
                .ToList();
                if (clientes.Count == 0)
                {
                    return null;
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