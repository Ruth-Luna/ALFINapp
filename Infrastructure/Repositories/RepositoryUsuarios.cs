using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Application.DTOs;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Repositories
{
    public class RepositoryUsuarios : ALFINapp.Domain.Interfaces.IRepositoryUsuarios
    {
        private readonly MDbContext _context;
        public RepositoryUsuarios(MDbContext context)
        {
            _context = context;
        }

        public async Task<List<DetallesUsuarioDTO>> GetAllAsesores()
        {
            try
            {
                var asesores = await _context
                    .usuarios
                    .AsNoTracking()
                    .Where(
                        x => x.IdRol == 3 &&
                        x.Dni != "73393133" &&
                        x.Dni !="74049517" &&
                        x.Dni != "98985454" &&
                        x.Dni != "87878744")
                    .ToListAsync();
                var asesoresDTO = new List<DetallesUsuarioDTO>();
                foreach (var item in asesores)
                {
                    asesoresDTO.Add(new DetallesUsuarioDTO(item));
                }
                if (asesores != null)
                {
                    return asesoresDTO;
                }
                else
                {
                    return new List<DetallesUsuarioDTO>();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<DetallesUsuarioDTO>();
            }
        }

        public async Task<List<DetallesUsuarioDTO>> GetAllAsesoresBySupervisor(int idSupervisor)
        {
            try
            {
                var asesores = await _context
                    .usuarios
                    .AsNoTracking()
                    .Where(
                        x => x.IdRol == 3 &&
                        x.IDUSUARIOSUP == idSupervisor &&
                        x.Dni != "73393133" &&
                        x.Dni !="74049517" &&
                        x.Dni != "98985454" &&
                        x.Dni != "87878744"
                        )
                    .ToListAsync();
                var asesoresDTO = new List<DetallesUsuarioDTO>();
                foreach (var item in asesores)
                {
                    asesoresDTO.Add(new DetallesUsuarioDTO(item));
                }
                if (asesores != null)
                {
                    return asesoresDTO;
                }
                else
                {
                    return new List<DetallesUsuarioDTO>();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<DetallesUsuarioDTO>();
            }
        }

        public async Task<List<DetallesUsuarioDTO>> GetAllSupervisores()
        {
            try
            {
                var supervisores = await _context
                    .usuarios
                    .AsNoTracking()
                    .Where(
                        x => x.IdRol == 2 &&
                        x.Dni != "73393133" &&
                        x.Dni !="74049517" &&
                        x.Dni != "98985454" &&
                        x.Dni != "87878744")
                    .ToListAsync();
                var supervisoresDTO = new List<DetallesUsuarioDTO>();
                foreach (var item in supervisores)
                {
                    supervisoresDTO.Add(new DetallesUsuarioDTO(item));
                }
                if (supervisores != null)
                {
                    return supervisoresDTO;
                }
                else
                {
                    return new List<DetallesUsuarioDTO>();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);                
                return new List<DetallesUsuarioDTO>();
            }
        }

        public async Task<List<DetallesUsuarioDTO>> GetAllUsers()
        {
            try
            {
                var usuarios = await _context
                    .usuarios
                    .Where (
                        x => x.Dni != "73393133" &&
                        x.Dni !="74049517" &&
                        x.Dni != "98985454" &&
                        x.Dni != "87878744")
                    .AsNoTracking()
                    .ToListAsync();
                var usuariosDTO = new List<DetallesUsuarioDTO>();
                foreach (var item in usuarios)
                {
                    usuariosDTO.Add(new DetallesUsuarioDTO(item));
                }
                if (usuarios != null)
                {
                    return usuariosDTO;
                }
                else
                {
                    return new List<DetallesUsuarioDTO>();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<DetallesUsuarioDTO>();
            }
        }

        public async Task<Usuario?> GetUser(int idUsuario)
        {
            try
            {
                var usuario = await _context
                    .usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
                if (usuario != null)
                {
                    return usuario;
                }
                else
                {
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> RegisterEmail(string? email, int idUsuario)
        {
            try
            {
                var registerEmail = await _context
                    .Database
                    .ExecuteSqlAsync(
                        $"EXECUTE dbo.sp_usuario_modificacion_existente @IdUsuario={idUsuario}, @Correo={email}");
                if (registerEmail == 0)
                {
                    return false;
                }
                return true;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public Task<bool> RegisterPassword(string password, int idUsuario)
        {
            throw new NotImplementedException();
        }
    }
}