using System.Data;
using ALFINapp.Infrastructure.Repositories;
using ALFINapp.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Operaciones
{
    public class DAO_Derivaciones
    {
        public MDbContext _context;
        public RepositoryUsuarios _repositoryUsuarios;
        public DAO_Derivaciones(MDbContext context,
            RepositoryUsuarios repositoryUsuarios)
        {
            _context = context;
            _repositoryUsuarios = repositoryUsuarios;
        }
        public async Task<ViewOperacionesDerivaciones> GetAllDerivaciones(int idSupervisor)
        {
            var asesores = _context.usuarios
                .AsNoTracking()
                .Where(x => x.IdRol == 3 && x.IDUSUARIOSUP == idSupervisor)
                .ToList();

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
                return new ViewOperacionesDerivaciones();
            }
            var viewOperacionesDerivaciones = new ViewOperacionesDerivaciones();
            return viewOperacionesDerivaciones;
        }
    }
}