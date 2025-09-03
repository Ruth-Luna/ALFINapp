using System.Data;
using ALFINapp.API.Models;
using ALFINapp.Domain.Entities;
using ALFINapp.Infrastructure.Persistence.Procedures;
using ALFINapp.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Models;

namespace ALFINapp.Datos.DAO.Operaciones
{
    public class DAO_Operaciones
    {
        public async Task<(bool issuccess, List<ViewReagendamientos>? data)> GetAllReagendamientos(int? idAsesor = null, int? idSupervisor = null, DateTime? fecha_reagendamiento = null, DateTime? fecha_visita = null, string? agencia = null, string? dni = null)
        {
            try
            {
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand command = new SqlCommand("[SP_REAGENDAMIENTOS_GET_REAGENDAMIENTOS_VIEW_2]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@month", DateTime.Now.Month);
                        command.Parameters.AddWithValue("@year", DateTime.Now.Year);
                        command.Parameters.AddWithValue("@id_asesor", idAsesor.HasValue ? idAsesor.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@id_supervisor", idSupervisor.HasValue ? idSupervisor.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@fecha_reagendamiento", fecha_reagendamiento.HasValue ? fecha_reagendamiento.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@fecha_visita", fecha_visita.HasValue ? fecha_visita.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@agencia", agencia ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@dni", dni ?? (object)DBNull.Value);

                        connection.Open();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var reagendamientos = new List<ViewReagendamientos>();
                            while (await reader.ReadAsync())
                            {
                                var reagendamiento = new ViewReagendamientos
                                {
                                    IdDerivacion = reader["id_derivacion"] != DBNull.Value ? Convert.ToInt32(reader["id_derivacion"]) : 0,
                                    IdAgendamientosRe = reader["id_agendamientos_re"] != DBNull.Value ? Convert.ToInt32(reader["id_agendamientos_re"]) : 0,
                                    Oferta = reader["oferta"] != DBNull.Value ? Convert.ToDecimal(reader["oferta"]) : 0,
                                    FechaVisita = reader["fecha_visita"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_visita"]) : null,
                                    Telefono = reader["telefono"] != DBNull.Value ? Convert.ToString(reader["telefono"]) : string.Empty,
                                    Agencia = reader["agencia"] != DBNull.Value ? Convert.ToString(reader["agencia"]) : string.Empty,
                                    FechaAgendamiento = reader["fecha_agendamiento"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_agendamiento"]) : null,
                                    FechaDerivacion = reader["fecha_derivacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_derivacion"]) : null,
                                    DniAsesor = reader["dni_asesor"] != DBNull.Value ? Convert.ToString(reader["dni_asesor"]) : string.Empty,
                                    DniCliente = reader["dni_cliente"] != DBNull.Value ? Convert.ToString(reader["dni_cliente"]) : string.Empty,
                                    NombreCliente = reader["nombre_cliente"] != DBNull.Value ? Convert.ToString(reader["nombre_cliente"]) : string.Empty,
                                    PuedeSerReagendado = reader["puede_ser_reagendado"] != DBNull.Value ? Convert.ToBoolean(reader["puede_ser_reagendado"]) : false,
                                    NombreAsesor = reader["nombre_asesor"] != DBNull.Value ? Convert.ToString(reader["nombre_asesor"]) : string.Empty,
                                    EstadoReagendamiento = reader["estado_derivacion"] != DBNull.Value ? Convert.ToString(reader["estado_derivacion"]) : string.Empty,
                                    FechaDerivacionOriginal = reader["fecha_derivacion_original"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_derivacion_original"]) : null,
                                    DocSupervisor = reader["doc_supervisor"] != DBNull.Value ? Convert.ToString(reader["doc_supervisor"]) : string.Empty,
                                    NumeroReagendamiento = reader["numero_reagendamiento"] != DBNull.Value ? Convert.ToInt32(reader["numero_reagendamiento"]) : 0,
                                    NumeroReagendamientoFormateado = reader["numero_reagendamiento_formateado"] != DBNull.Value ? Convert.ToString(reader["numero_reagendamiento_formateado"]) : string.Empty,
                                    FueDesembolsadoGeneral = reader["fue_desembolsado_general"] != DBNull.Value ? Convert.ToBoolean(reader["fue_desembolsado_general"]) : false,
                                    FueEnviadoEmail = reader["fue_enviado_email"] != DBNull.Value ? Convert.ToBoolean(reader["fue_enviado_email"]) : false,
                                    //TotalReagendamientos = reader["total_reagendamientos"] != DBNull.Value ? Convert.ToInt32(reader["total_reagendamientos"]) : 0
                                };
                                reagendamientos.Add(reagendamiento);
                            }
                            return (true, reagendamientos);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, null);
            }
        }


        public async Task<(bool success, List<ViewDerivaciones>? data)> GetAllDerivaciones(int? idAsesor, int? idSupervisor, string? agencia = null, DateTime? fecha_derivacion = null, DateTime? fecha_visita = null, string? dni = null)
        {
            try
            {
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand command = new SqlCommand("[sp_Derivacion_consulta_derivaciones_x_asesor_por_dni_con_reagendacion_2]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@month", DateTime.Now.Month);
                        command.Parameters.AddWithValue("@year", DateTime.Now.Year);
                        command.Parameters.AddWithValue("@id_asesor", idAsesor.HasValue ? idAsesor.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@id_supervisor", idSupervisor.HasValue ? idSupervisor.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@agencia", agencia ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@fecha_derivacion", fecha_derivacion ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@fecha_visita", fecha_visita ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@dni", dni ?? (object)DBNull.Value);

                        connection.Open();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var derivaciones = new List<ViewDerivaciones>();
                            while (await reader.ReadAsync())
                            {
                                var derivacion = new ViewDerivaciones
                                {
                                    IdDerivacion = reader["id_derivacion"] != DBNull.Value ? Convert.ToInt32(reader["id_derivacion"]) : 0,
                                    FechaDerivacion = Convert.ToDateTime(reader["fecha_derivacion"]),
                                    DniAsesor = reader["dni_asesor"]?.ToString() ?? string.Empty,
                                    NombreAsesor = reader["nombre_asesor"]?.ToString() ?? string.Empty,
                                    DniCliente = reader["dni_cliente"]?.ToString() ?? string.Empty,
                                    IdCliente = reader["id_cliente"] != DBNull.Value ? Convert.ToInt32(reader["id_cliente"]) : 0,
                                    NombreCliente = reader["nombre_cliente"]?.ToString() ?? string.Empty,
                                    TelefonoCliente = reader["telefono_cliente"]?.ToString() ?? string.Empty,
                                    NombreAgencia = reader["nombre_agencia"]?.ToString() ?? string.Empty,
                                    NumAgencia = reader["num_agencia"]?.ToString() ?? string.Empty,
                                    FueProcesado = reader["fue_procesado"] != DBNull.Value ? Convert.ToBoolean(reader["fue_procesado"]) : false,
                                    FechaVisita = Convert.ToDateTime(reader["fecha_visita"]),
                                    EstadoDerivacion = reader["estado_derivacion"]?.ToString() ?? string.Empty,
                                    IdAsignacion = reader["id_asignacion"] != DBNull.Value ? Convert.ToInt32(reader["id_asignacion"]) : 0,
                                    ObservacionDerivacion = reader["observacion_derivacion"]?.ToString() ?? string.Empty,
                                    FueEnviadoEmail = reader["fue_enviado_email"] != DBNull.Value ? Convert.ToBoolean(reader["fue_enviado_email"]) : false,
                                    IdDesembolso = reader["ID_DESEMBOLSO"] != DBNull.Value ? Convert.ToInt32(reader["ID_DESEMBOLSO"]) : 0,
                                    DocSupervisor = reader["doc_supervisor"]?.ToString() ?? string.Empty,
                                    OfertaMax = reader["oferta_max"] != DBNull.Value ? Convert.ToDecimal(reader["oferta_max"]) : 0,
                                    Supervisor = reader["supervisor"]?.ToString() ?? string.Empty,
                                    MontoDesembolso = reader["monto_desembolso"] != DBNull.Value ? Convert.ToDecimal(reader["monto_desembolso"]) : 0,
                                    RealError = reader["real_error"]?.ToString() ?? string.Empty,
                                    //FueReagendado = reader["fue_reagendado"] != DBNull.Value ? Convert.ToBoolean(reader["fue_reagendado"]) : false,
                                    //FueReprocesado = reader["fue_reprocesado"] != DBNull.Value ? Convert.ToBoolean(reader["fue_reprocesado"]) : false,
                                    PuedeSerReagendado = reader["PuedeSerReagendado"] != DBNull.Value ? Convert.ToBoolean(reader["PuedeSerReagendado"]) : false,
                                    FechaEvidencia = reader["fecha_evidencia"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_evidencia"]) : (DateTime?)null,
                                    estadoEvidencia = reader["estado_evidencia"]?.ToString() ?? string.Empty,
                                    HayEvidencia = reader["hay_evidencias"] != DBNull.Value ? Convert.ToBoolean(reader["hay_evidencias"]) : false,
                                    FueDesembolsado = reader["fue_desembolsado"] != DBNull.Value ? Convert.ToBoolean(reader["fue_desembolsado"]) : false,
                                    FechaDesembolsos = reader["fecha_desembolsos"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_desembolsos"]) : (DateTime?)null,
                                    DocAsesorDesembolso = reader["doc_asesor_desembolso"]?.ToString() ?? string.Empty,
                                    DocSupervisorDesembolso = reader["doc_supervisor_desembolso"]?.ToString() ?? string.Empty,
                                    MontoDesembolsoFinanciado = reader["monto_desembolso_financiado"] != DBNull.Value ? Convert.ToDecimal(reader["monto_desembolso_financiado"]) : (decimal?)null

                                };
                                derivaciones.Add(derivacion);
                            }
                            return (true, derivaciones);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, null);
            }
        }

        public List<ViewDerivaciones> Listar_Derivaciones_Excel(string? dni , int? idAsesor, int? idSupervisor, string? agencia = null, DateTime? fecha_derivacion = null, DateTime? fecha_visita = null)
        {
            try
            {
                var cn = new Conexion();
                using (var connection = new SqlConnection(cn.getCadenaSQL()))
                using (var command = new SqlCommand("SP_DERIVACION_Listar_Derivaciones_Excel", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@dni", string.IsNullOrEmpty(dni) ? (object)DBNull.Value : dni);
                    command.Parameters.AddWithValue("@id_asesor", idAsesor.HasValue ? (object)idAsesor.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@id_supervisor", idSupervisor.HasValue ? (object)idSupervisor.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@fecha_derivacion", fecha_derivacion ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@fecha_visita", fecha_visita ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@agencia", agencia ?? (object)DBNull.Value);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        var derivaciones = new List<ViewDerivaciones>();
                        while (reader.Read())
                        {
                            derivaciones.Add(new ViewDerivaciones
                            {
                                EstadoDerivacion = reader["estado_derivacion"]?.ToString() ?? string.Empty,
                                EstadoFormulario = reader["estado_formulario"]?.ToString() ?? string.Empty,
                                EstadoCorreo = reader["estado_email"]?.ToString() ?? string.Empty,
                                DniCliente = reader["dni_cliente"]?.ToString() ?? string.Empty,
                                NombreCliente = reader["nombre_cliente"]?.ToString() ?? string.Empty,
                                TelefonoCliente = reader["telefono_cliente"]?.ToString() ?? string.Empty,
                                NombreAsesor = reader["nombre_asesor"]?.ToString() ?? string.Empty,
                                OfertaMax = reader["oferta_max"] != DBNull.Value ? Convert.ToDecimal(reader["oferta_max"]) : 0,
                                NombreAgencia = reader["nombre_agencia"]?.ToString() ?? string.Empty,
                                FechaDerivacion = Convert.ToDateTime(reader["fecha_derivacion"]),
                                FechaVisita = Convert.ToDateTime(reader["fecha_visita"]),
                                estadoEvidencia = reader["estado_evidencia"]?.ToString() ?? string.Empty,
                                FechaEvidencia = reader["fecha_evidencia"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_evidencia"]) : (DateTime?)null,
                            });
                        }
                        return derivaciones;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ViewDerivaciones>();
            }
        }
        public async Task<(bool success, List<ViewAgencias>? data)> GetAgencias()
        {
            try
            {
                var cn = new Conexion();
                using (var connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (var command = new SqlCommand("[SP_OPERACIONES_LISTAR_AGENCIAS]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var agencias = new List<ViewAgencias>();
                            while (await reader.ReadAsync())
                            {
                                var agencia = new ViewAgencias
                                {
                                    CECO = reader["CECO"]?.ToString() ?? null,
                                    agencia = reader["AGENCIA"]?.ToString() ?? null
                                };
                                agencias.Add(agencia);
                            }
                            return (true, agencias);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, null);
            }
        }

        public async Task<(bool success, List<ViewReagendamientos>? data)> GetHistosricoReagendamientos(List<int> idsDerivacion)
        {
            try
            {
                var cn = new Conexion();

                using (SqlConnection connection = new SqlConnection(cn.getCadenaSQL()))
                {
                    using (SqlCommand command = new SqlCommand("[SP_REAGENDAMIENTOS_GET_REAGENDAMIENTOS_HISTORICO]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        // Transformar lista a XML usando LINQ
                        var xml = new System.Xml.Linq.XElement("Ids",
                            idsDerivacion.Select(id => new System.Xml.Linq.XElement("Id", id))
                        ).ToString();

                        command.Parameters.Add(new SqlParameter("@ids_derivacionXML", SqlDbType.Xml) { Value = xml });
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var derivaciones = new List<ViewReagendamientos>();
                            while (await reader.ReadAsync())
                            {
                                var derivacion = new ViewReagendamientos
                                {
                                    IdDerivacion = reader["id_derivacion"] != DBNull.Value ? Convert.ToInt32(reader["id_derivacion"]) : 0,
                                    IdAgendamientosRe = reader["id_agendamientos_re"] != DBNull.Value ? Convert.ToInt32(reader["id_agendamientos_re"]) : 0,
                                    Oferta = reader["oferta"] != DBNull.Value ? Convert.ToDecimal(reader["oferta"]) : 0,
                                    FechaVisita = reader["fecha_visita"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_visita"]) : null,
                                    Telefono = reader["telefono"] != DBNull.Value ? Convert.ToString(reader["telefono"]) : string.Empty,
                                    Agencia = reader["agencia"] != DBNull.Value ? Convert.ToString(reader["agencia"]) : string.Empty,
                                    FechaAgendamiento = reader["fecha_agendamiento"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_agendamiento"]) : null,
                                    FechaDerivacion = reader["fecha_derivacion"] != DBNull.Value ? Convert.ToDateTime(reader["fecha_derivacion"]) : null,
                                    DniAsesor = reader["dni_asesor"] != DBNull.Value ? Convert.ToString(reader["dni_asesor"]) : string.Empty,
                                    DniCliente = reader["dni_cliente"] != DBNull.Value ? Convert.ToString(reader["dni_cliente"]) : string.Empty,
                                    //NumeroReagendamiento = reader["numero_reagendamiento"] != DBNull.Value ? Convert.ToInt32(reader["numero_reagendamiento"]) : 0,
                                    NumeroReagendamientoFormateado = reader["numero_reagendamiento_formateado"] != DBNull.Value ? Convert.ToString(reader["numero_reagendamiento_formateado"]) : string.Empty,
                                    EstadoReagendamiento = reader["estado_derivacion"] != DBNull.Value ? Convert.ToString(reader["estado_derivacion"]) : string.Empty,
                                    NombreCliente = reader["nombre_cliente"] != DBNull.Value ? Convert.ToString(reader["nombre_cliente"]) : string.Empty,
                                    NombreAsesor = reader["nombre_asesor"] != DBNull.Value ? Convert.ToString(reader["nombre_asesor"]) : string.Empty,

                                };
                                derivaciones.Add(derivacion);
                            }
                            return (true, derivaciones);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, null);
            }
        }


    }
}