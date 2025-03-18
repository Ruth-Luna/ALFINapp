using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Persistence.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Infrastructure.Services
{
    public class DBServicesConsultasAdministrador
    {
        private readonly MDbContext _context;
        public DBServicesConsultasAdministrador(MDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsSuccess, string Message, List<AsignacionFiltrarBasesDTO>? Data)> AsignacionFiltrarBases(
                                    string? base_busqueda,
                                    string? rango_edad,
                                    string? rango_tasas,
                                    decimal? oferta,
                                    string? tipo_cliente,
                                    string? cliente_estado,
                                    string? grupo_tasa,
                                    string? grupo_monto,
                                    int? deudas,
                                    List<string>? campaña,
                                    List<string>? usuarios,
                                    List<string>? propension,
                                    List<string>? color,
                                    List<string>? color_final,
                                    List<string>? frescura)
        {
            try
            {
                var asignacionFiltrarBases = await _context.asignacion_filtrar_bases_dto
                    .FromSqlRaw("EXEC sp_Asignacion_FiltrarBases @base = {0}, @campaña = {1}, @oferta = {2}, @usuario = {3}, @propension = {4}, @color = {5}, @color_final = {6}, @rango_edad = {7}, @rango_sueldo = {8}, @rango_tasa = {9}, @rango_oferta = {10}, @tipo_cliente = {11}, @cliente_estado = {12}, @grupo_tasa = {13}, @grupo_monto = {14}, @deudas = {15}, @frescura = {16}",
                        new SqlParameter("@base", base_busqueda ?? (object)DBNull.Value),
                        new SqlParameter("@campaña", campaña != null ? string.Join(",", campaña) : (object)DBNull.Value),
                        new SqlParameter("@oferta", oferta ?? (object)DBNull.Value),
                        new SqlParameter("@usuario", usuarios != null ? string.Join(",", usuarios) : (object)DBNull.Value),
                        new SqlParameter("@propension", propension != null ? string.Join(",", propension) : (object)DBNull.Value),
                        new SqlParameter("@color", color != null ? string.Join(",", color) : (object)DBNull.Value),
                        new SqlParameter("@color_final", color_final != null ? string.Join(",", color_final) : (object)DBNull.Value),
                        new SqlParameter("@rango_edad", rango_edad ?? (object)DBNull.Value),
                        new SqlParameter("@rango_sueldo", (object)DBNull.Value),
                        new SqlParameter("@rango_tasa", rango_tasas ?? (object)DBNull.Value),
                        new SqlParameter("@rango_oferta", (object)DBNull.Value),
                        new SqlParameter("@tipo_cliente", tipo_cliente ?? (object)DBNull.Value),
                        new SqlParameter("@cliente_estado", cliente_estado ?? (object)DBNull.Value),
                        new SqlParameter("@grupo_tasa", grupo_tasa ?? (object)DBNull.Value),
                        new SqlParameter("@grupo_monto", grupo_monto ?? (object)DBNull.Value),
                        new SqlParameter("@deudas", deudas ?? (object)DBNull.Value),
                        new SqlParameter("@frescura", frescura != null ? string.Join(",", frescura) : (object)DBNull.Value)
                    )
                    .ToListAsync();

                if (asignacionFiltrarBases == null)
                {
                    return (false, "La Asignacion al Filtrar Bases no se ha encontrado", null);
                }

                return (true, "Asignacion Filtrar Bases ha devuelto una tabla", asignacionFiltrarBases);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<Usuario>? Data)> ConseguirTodosLosSupervisores()
        {
            try
            {
                var TodosLosSupervisores = await (from u in _context.usuarios
                                                  where u.IdRol == 2
                                                  select u
                                ).ToListAsync();

                if (TodosLosSupervisores == null)
                {
                    return (false, "No se han encontrado supervisores este error fue inesperado", null);
                }

                return (true, "Se han encontrado los siguientes supervisores", TodosLosSupervisores);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool IsSuccess, string Message, List<Usuario>? Data)> ConseguirTodosLosUsuarios()
        {
            try
            {
                var TodosLosUsuarios = await _context.usuarios.FromSqlRaw("EXEC sp_usuario_conseguir_todos").ToListAsync();
                if (TodosLosUsuarios == null)
                {
                    return (false, "No se han encontrado usuarios este error fue inesperado", null);
                }
                return (true, "Se han encontrado los siguientes usuarios", TodosLosUsuarios);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, EnriquecerConsultasSupervisorDTO? Data)> EnriquecerConsultasSupervisor(Usuario supervisor)
        {
            try
            {
                var getAsesoresInfo = await (from u in _context.usuarios
                                                where u.IDUSUARIOSUP == supervisor.IdUsuario
                                                    && u.IdRol == 3
                                                    && u.Estado == "ACTIVO"
                                                select u
                                            ).ToListAsync();

                var getAsesoresTipInfo = new List<EnriquecerConsultasSupervisorDTO>();
                if (getAsesoresInfo != null)
                {
                    foreach (var item in getAsesoresInfo)
                    {
                        var getDerivacionesInfo = await (from d in _context.derivaciones_asesores
                                                            where d.DniAsesor == item.Dni
                                                                && d.FechaDerivacion.Year == DateTime.Now.Year
                                                                && d.FechaDerivacion.Month == DateTime.Now.Month
                                                            select new DerivacionesInfo
                                                            {
                                                                IdDerivacion = d.IdDerivacion,
                                                                DniCliente = d.DniCliente,
                                                                FechaDerivacion = d.FechaDerivacion
                                                            }).ToListAsync();
                        var getDerivacionesTotales = await (from d in _context.derivaciones_asesores
                                                            where d.DniAsesor == item.Dni
                                                            select d
                                                            ).ToListAsync();
                        var getClientesAsignadosInfo = await (from ca in _context.clientes_asignados
                                                            where ca.IdUsuarioV == item.IdUsuario
                                                                && ca.FechaAsignacionVendedor.HasValue
                                                                && ca.FechaAsignacionVendedor.Value.Year == DateTime.Now.Year
                                                                && ca.FechaAsignacionVendedor.Value.Month == DateTime.Now.Month
                                                            select ca
                                                            ).ToListAsync();
                        var enriquecerClientesAsignadosInfoXTipificacion = new List<AsignacionesInfo> ();
                        foreach (var item2 in getClientesAsignadosInfo)
                        {
                            var getTipificacionesInfo = await (from ct in _context.clientes_tipificados
                                                            join t in _context.tipificaciones on ct.IdTipificacion equals t.IdTipificacion
                                                            where ct.IdAsignacion == item2.IdAsignacion
                                                            select new { ct, t }
                                                            ).ToListAsync();
                            var getAsignacionesInfo = new AsignacionesInfo
                            {
                                IdAsignacion = item2.IdAsignacion,
                                Tipificaciones = getTipificacionesInfo.Select(x => x.t.DescripcionTipificacion).ToList(),
                                idTipificaciones = getTipificacionesInfo.Select(x => x.t.IdTipificacion).ToList()
                            };
                            enriquecerClientesAsignadosInfoXTipificacion.Add(getAsignacionesInfo);                            
                        }
                        var getAsesoresTipInfoDerivacionesAsignaciones = new EnriquecerConsultasSupervisorDTO
                        {
                            IdUsuario = item.IdUsuario,
                            Dni = item.Dni,
                            NombresCompletos = item.NombresCompletos,
                            Rol = item.Rol,
                            derivaciones = getDerivacionesInfo,
                            AsignacionesInfo = enriquecerClientesAsignadosInfoXTipificacion
                        };
                    }
                }
                return (true, "Se han encontrado los siguientes asesores", null);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}