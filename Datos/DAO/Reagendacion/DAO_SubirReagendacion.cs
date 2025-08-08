using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ALFINapp.Datos.DAO.Reagendacion
{
    public class DAO_SubirReagendacion
    {
        private readonly MDbContext _context;
        public DAO_SubirReagendacion(MDbContext context)
        {
            _context = context;
        }
        public async Task<(bool success, string message)> reagendarCliente(
            int idDerivacion, DateTime fechaReagendamiento, List<string>? urls = null)
        {
            try
            {
                urls ??= new List<string>();
                var checkDis = await checkDisReagendacion(idDerivacion, fechaReagendamiento);
                if (!checkDis.success)
                {
                    return (false, checkDis.message);
                }
                var upload = await uploadReagendacion(idDerivacion, fechaReagendamiento, string.Join(",", urls));
                if (!upload.success)
                {
                    return (false, upload.message);
                }
                return (true, "Reagendamiento realizado con éxito.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool success, string message)> checkDisReagendacion(
            int idDerivacion, DateTime fechaReagendamiento)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdDerivacion", idDerivacion),
                    new SqlParameter("@FechaDerivacion", fechaReagendamiento)
                };
                var check = await _context.resultado_verificacion
                    .FromSqlRaw("exec sp_reagendamiento_verificar_disponibilidad_para_reagendamiento_derivacion @IdDerivacion, @FechaDerivacion", parameters)
                    .AsNoTracking()
                    .ToListAsync();
                if (check.Count == 0)
                {
                    return (false, "No se ha encontrado la derivación.");
                }
                var result = check.FirstOrDefault();
                if (result == null)
                {
                    return (false, "No se ha encontrado la derivación.");
                }
                if (result.Resultado == 1)
                {
                    return (false, result.Mensaje);
                }
                else
                {
                    return (true, result.Mensaje);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (false, "Ha ocurrido un error en su red, o en la Base de Datos.");
            }
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<(bool success, string message)> uploadReagendacion(
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            int idDer, DateTime fechaReagendamiento, string urls)
        {
            try
            {
                var parametros = new[]
                {
                    new SqlParameter("@nueva_fecha_visita", fechaReagendamiento) { SqlDbType = SqlDbType.DateTime },
                    new SqlParameter("@id_derivacion", idDer) { SqlDbType = SqlDbType.Int },
                    new SqlParameter("@urls", urls ?? string.Empty)
                };
                var generarReagendacion = _context.Database.ExecuteSqlRaw(
                    "EXEC sp_reagendamiento_upload_nueva_reagendacion_refac @nueva_fecha_visita, @id_derivacion, @urls;",
                    parametros);
                if (generarReagendacion == 0)
                {
                    return (false, "Error al subir la reagendacion");
                }
                return (true, "Reagendacion subida correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (false, ex.Message);
            }
        }
    }
}