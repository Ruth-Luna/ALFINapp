using System.Data;
using ALFINapp.Datos.DAO.Miscelaneos;
using ALFINapp.Datos.DAO.Usuarios;
using ALFINapp.Infrastructure.Persistence.Models;
using ALFINapp.Infrastructure.Repositories;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Operaciones
{
    public class DAO_Derivaciones
    {
        public MDbContext _context;
        private readonly DAO_ConsultasMiscelaneas _dao_consultasMiscelaneas;
        private readonly DAO_Usuarios _dao_usuarios;
        public DAO_Derivaciones(
            MDbContext context,
            DAO_ConsultasMiscelaneas dao_consultasMiscelaneas,
            DAO_Usuarios dao_usuarios
            )
        {
            _context = context;
            _dao_consultasMiscelaneas = dao_consultasMiscelaneas;
            _dao_usuarios = dao_usuarios;
        }
        public async Task<(bool success, string message, ViewOperacionesDerivaciones data)> GetAllDerivaciones(
            int idUsuario,
            int idRol)
        {
            try
            {
                var getdatosUsuario = await _dao_consultasMiscelaneas.getuser(idUsuario);
                if (getdatosUsuario == null)
                {
                    return (false, "No se encontró el usuario", new ViewOperacionesDerivaciones());
                }
                var viewDerivaciones = new ViewOperacionesDerivaciones();
                var asesores = new List<Usuario>();
                var supervisores = new List<Usuario>();
                if (idRol == 1 || idRol == 4)
                {
                    var getAllAsesores = await _dao_usuarios.ListarAsesores();
                    var GetAllSupervisores = await _dao_usuarios.ListarSupervisores();
                    foreach (var item in GetAllSupervisores)
                    {
                        supervisores.Add(item);
                    }
                    foreach (var item in getAllAsesores)
                    {
                        var supAsesor = supervisores.FirstOrDefault(x => x.IdUsuario == item.IDUSUARIOSUP);
                        asesores.Add(item);
                    }
                }
                else if (idRol == 2)
                {
                    var getAllAsesores = await _dao_usuarios.ListarAsesores(idUsuario);
                    foreach (var item in getAllAsesores)
                    {
                        asesores.Add(item);
                    }
                }
                else if (idRol == 3)
                {
                    asesores.Add(getdatosUsuario);
                }
                var getADerivaciones = await getDerivaciones(asesores);
                var orderDerivaciones = getADerivaciones.data.OrderByDescending(x => x.FechaDerivacion).ToList();
                viewDerivaciones.Asesores = asesores;
                viewDerivaciones.Supervisores = supervisores;
                viewDerivaciones.RolUsuario = idRol;
                viewDerivaciones.DniUsuario = getdatosUsuario.Dni ?? string.Empty;
                viewDerivaciones.Derivaciones = orderDerivaciones;
                
                return (true, "Se realizo la busqueda de Derivaciones con exito", viewDerivaciones);
            }
            catch (System.Exception ex)
            {
                return (false, ex.Message, new ViewOperacionesDerivaciones());
            }
        }
        public async Task<(bool success, string message, List<ViewDerivaciones> data)> getDerivaciones(List<Usuario> asesores)
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
                    return (false, "No se encontraron derivaciones", new List<ViewDerivaciones>());
                }
                var derivaciones = new List<ViewDerivaciones>();
                foreach (var item in result)
                {
                    derivaciones.Add(new ViewDerivaciones(item));
                }
                return (true, "Se obtuvieron las derivaciones correctamente.", derivaciones);
            }
            catch (Exception ex)
            {
                return (false, $"Error al obtener las derivaciones: {ex.Message}", new List<ViewDerivaciones>());
            }
        }
    }
}