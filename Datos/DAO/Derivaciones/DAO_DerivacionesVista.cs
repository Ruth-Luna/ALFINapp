using System.Data;
using ALFINapp.API.Models;
using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Derivaciones
{
    public class DAO_DerivacionesVista
    {
        private readonly MDbContext _context;
        private readonly DA_Usuario _da_usuario = new DA_Usuario();
        public DAO_DerivacionesVista(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool success, string message, ViewDerivacionesVistaGeneral data)> getDerivacionesVista(
            int idUsuario,
            int idRol)
        {
            try
            {
                var getdatosUsuario = _da_usuario.getUsuario(idUsuario);
                if (getdatosUsuario == null)
                {
                    return (false, "No se encontró el usuario", new ViewDerivacionesVistaGeneral());
                }
                var viewDerivaciones = new ViewDerivacionesVistaGeneral();
                var asesoresu = new List<Usuario>();
                var supervisores = new List<ViewUsuario>();
                if (idRol == 1 || idRol == 4)
                {
                    var getAllAsesores = _da_usuario.ListarAsesores();
                    var GetAllSupervisores = _da_usuario.ListarSupervisores();
                    foreach (var item in GetAllSupervisores)
                    {
                        supervisores.Add(new ViewUsuario(item));
                    }
                    foreach (var item in getAllAsesores)
                    {
                        var supAsesor = supervisores.FirstOrDefault(x => x.IdUsuario == item.IDUSUARIOSUP);
                        if (supAsesor != null)
                        {
                            supAsesor.Asesores.Add(new ViewUsuario(item));
                        }
                        asesoresu.Add(item);
                    }
                }
                else if (idRol == 2)
                {
                    var getAllAsesores = _da_usuario.ListarAsesores(idUsuario);
                    foreach (var item in getAllAsesores)
                    {
                        asesoresu.Add(item);
                    }

                }
                else if (idRol == 3)
                {
                    asesoresu.Add(getdatosUsuario);
                }
                var getAllDerivaciones = await getDerivaciones(asesoresu);
                getAllDerivaciones = getAllDerivaciones.OrderByDescending(x => x.FechaDerivacion).ToList();
                viewDerivaciones.Asesores = asesoresu.Select(x => new ViewUsuario(x)).ToList();
                viewDerivaciones.Supervisores = supervisores;
                viewDerivaciones.RolUsuario = idRol;
                viewDerivaciones.DniUsuario = getdatosUsuario.Dni ?? string.Empty;

                viewDerivaciones.Derivaciones = getAllDerivaciones;
                return (true, "Se realizo la busqueda de Derivaciones con exito", viewDerivaciones);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewDerivacionesVistaGeneral());
            }
        }

        private async Task<List<ViewDerivaciones>> getDerivaciones(List<Usuario> asesores)
        {
            try
            {
                var dnis = new DataTable();
                dnis.Columns.Add("Dni", typeof(string));
                var getAllDnisClientes = asesores.Select(x => x.Dni).ToHashSet();
                foreach (var dni in getAllDnisClientes)
                {
                    dnis.Rows.Add(dni);
                }
                var parameter = new SqlParameter("@Dni", SqlDbType.Structured)
                {
                    TypeName = "dbo.DniTableType",
                    Value = dnis
                };
                var result = await _context.derivaciones_asesores_for_view_derivacion
                    .FromSqlRaw("EXEC sp_Derivacion_consulta_derivaciones_x_asesor_por_dni_con_reagendacion @Dni = {0}",
                        parameter)
                    .ToListAsync();
                if (result == null || result.Count == 0)
                {
                    return new List<ViewDerivaciones>();
                }
                return result.Select(x => new ViewDerivaciones(x)).ToList();
            }
            catch (System.Exception)
            {
                return new List<ViewDerivaciones>();
            }
            finally
            {
                // Cleanup if necessary
            }
        }
    }
}