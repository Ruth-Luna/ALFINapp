using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ALFINapp.Services
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

                foreach (var item in asignacionFiltrarBases)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(item));
                }

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
                                                  where u.Rol == "SUPERVISOR"
                                                  select new Usuario
                                                  {
                                                      IdUsuario = u.IdUsuario,
                                                      Dni = u.Dni,
                                                      NombresCompletos = u.NombresCompletos,
                                                  }
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
                var TodosLosUsuarios = await (from u in _context.usuarios
                                              select u ).ToListAsync();

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
    }
}