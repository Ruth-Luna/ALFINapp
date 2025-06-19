using ALFINapp.Infrastructure.Persistence.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace ALFINapp.Datos
{
    public class DA_Usuario
    {
        public async Task<(bool IsSuccess, string Message)> CrearUsuario(Usuario usuario, int idUsuarioAccion)
        {
            try
            {
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand command = new SqlCommand("SP_USUARIO_REGISTRAR_NUEVO", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Dni", usuario.Dni);
                        command.Parameters.AddWithValue("@Paterno", usuario.Apellido_Paterno);
                        command.Parameters.AddWithValue("@Materno", usuario.Apellido_Materno);
                        command.Parameters.AddWithValue("@Nombres", usuario.Nombres);
                        command.Parameters.AddWithValue("@Rol", usuario.Rol ?? "ASESOR");
                        command.Parameters.AddWithValue("@Departamento", (object?)usuario.Departamento ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Provincia", (object?)usuario.Provincia ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Distrito", (object?)usuario.Distrito ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Telefono", (object?)usuario.Telefono ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Estado", (object?)usuario.Estado ?? "ACTIVO");
                        command.Parameters.AddWithValue("@IDUSUARIOSUP", (object?)usuario.IDUSUARIOSUP ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RESPONSABLESUP", (object?)usuario.RESPONSABLESUP ?? DBNull.Value);
                        command.Parameters.AddWithValue("@REGION", (object?)usuario.REGION ?? DBNull.Value);
                        command.Parameters.AddWithValue("@NOMBRECAMPAÑA", (object?)usuario.NOMBRECAMPAÑA ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IdRol", usuario.IdRol);
                        command.Parameters.AddWithValue("@id_usuario_accion", idUsuarioAccion);
                        command.Parameters.AddWithValue("@Correo", (object?)usuario.Correo ?? DBNull.Value);

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return (true, "Usuario creado correctamente");
            }
            catch (Exception ex)
            {
                return (false, "Error al crear el usuario: " + ex.Message);
            }
        }
    }
}
