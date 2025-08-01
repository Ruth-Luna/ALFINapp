using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Asignaciones;
using ALFINapp.Datos.DAO.Derivaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    [Route("Excel")]
    public class ExcelController : Controller
    {
        private readonly MDbContext _context;
        private readonly IUseCaseDownloadAsignaciones useCaseDownloadAsignaciones;
        private readonly DAO_DescargarDerivaciones dAO_DescargarDerivaciones;
        private readonly ILogger<ExcelController> _logger;
        public ExcelController(
            MDbContext context,
            IUseCaseDownloadAsignaciones useCaseDownloadAsignaciones,
            ILogger<ExcelController> logger,
            DAO_DescargarDerivaciones dAO_DescargarDerivaciones)
        {
            _context = context;
            this.useCaseDownloadAsignaciones = useCaseDownloadAsignaciones;
            _logger = logger;
            this.dAO_DescargarDerivaciones = dAO_DescargarDerivaciones;
        }

        private bool IsValidDni(float dni)
        {
            // Convertir el DNI a string
            string dniString = dni.ToString();

            // Comprobar que el DNI tenga exactamente 8 caracteres y que todos sean números
            return dniString.Length > 7;
        }


        [HttpGet("DescargarClientesAsignados")]
        public IActionResult DescargarClientesAsignados(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string? filtroDescarga,
            string? typeFilter)
        {
            try
            {
                int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
                if (idSupervisorActual == null)
                {
                    TempData["Message"] = "Error en la autenticación. Intente iniciar sesión nuevamente.";
                    return RedirectToAction("Index", "Home");
                }
                if (fechaFin.HasValue)
                {
                    fechaFin = fechaFin.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                if (fechaInicio.HasValue)
                {
                    fechaInicio = fechaInicio.Value.Date.AddMinutes(-1); // Reducir un minuto
                }

                var query = (from ca in _context.clientes_asignados
                             join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                             join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                             join db in _context.detalle_base on bc.IdBase equals db.IdBase
                             where ca.IdUsuarioS == idSupervisorActual
                                   && ca.ClienteDesembolso != true
                                   && ca.ClienteRetirado != true
                                   && ca.FechaAsignacionSup.HasValue
                                   && ca.FechaAsignacionSup.Value.Year == DateTime.Now.Year
                                   && db.TipoBase == ca.FuenteBase
                             select new
                             {
                                 db.IdBase,
                                 db.FechaCarga,
                                 db.Campaña,
                                 db.OfertaMax,
                                 db.TasaMinima,
                                 db.SucursalComercial,
                                 db.AgenciaComercial,
                                 db.Plazo,
                                 db.Cuota,
                                 db.Oferta12m,
                                 db.Tasa12m,
                                 db.Cuota12m,
                                 db.Oferta18m,
                                 db.Tasa18m,
                                 db.Cuota18m,
                                 db.Oferta24m,
                                 db.Tasa24m,
                                 db.Cuota24m,
                                 db.Oferta36m,
                                 db.Tasa36m,
                                 db.Cuota36m,
                                 db.GrupoTasa,
                                 db.GrupoMonto,
                                 db.Propension,
                                 db.TipoCliente,
                                 db.ClienteNuevo,
                                 db.Color,
                                 db.ColorFinal,
                                 db.Usuario,
                                 db.UserV3,
                                 db.FlagDeudaVOferta,
                                 db.PerfilRo,
                                 db.Propensionv2,
                                 db.PrioridadSistema,
                                 bc.Dni,
                                 bc.XAppaterno,
                                 bc.XApmaterno,
                                 bc.XNombre,
                                 bc.Edad,
                                 bc.Departamento,
                                 bc.Provincia,
                                 bc.Distrito,
                                 ce.Telefono1,
                                 ce.Telefono2,
                                 ce.Telefono3,
                                 ce.Telefono4,
                                 ce.Telefono5,

                                 ca.FechaAsignacionSup,
                                 ca.IdUsuarioS,
                                 ca.Destino,
                                 ca.IdAsignacion,
                                 ca.IdLista
                             }).AsNoTracking();

                // Aplicar filtros antes de ejecutar la consulta
                if (fechaInicio.HasValue)
                {
                    query = query.Where(ca => ca.FechaAsignacionSup >= fechaInicio).AsNoTracking();
                }
                if (fechaFin.HasValue)
                {
                    query = query.Where(ca => ca.FechaAsignacionSup <= fechaFin).AsNoTracking();
                }
                if (!string.IsNullOrEmpty(filtroDescarga) && typeFilter == "destino")
                {
                    query = query.Where(ca => ca.Destino == filtroDescarga).AsNoTracking();
                }
                if (!string.IsNullOrEmpty(filtroDescarga) && typeFilter == "lista")
                {
                    var idLista = _context.listas_asignacion
                        .Where(l => l.NombreLista == filtroDescarga)
                        .Select(l => l.IdLista)
                        .FirstOrDefault();
                    query = query.Where(ca => ca.IdLista == idLista)
                        .AsNoTracking();
                }

                var result = query.GroupBy(ca => ca.IdAsignacion)
                    .Select(group => group.OrderByDescending(ca => ca.FechaAsignacionSup).FirstOrDefault());

                // Ejecutar la consulta
                var supervisorData = result.ToList();
                var supervisorUsuario = _context.usuarios
                    .Where(u => u.IdUsuario == idSupervisorActual)
                    .FirstOrDefault();

                if (supervisorUsuario == null)
                {
                    TempData["Message"] = "Error al verificar el usuario. Intente iniciar sesión nuevamente.";
                    return RedirectToAction("Redireccionar", "Error");
                }
                /*var supervisorData = (from ca in _context.clientes_asignados
                                      join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                      join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                                      join db in _context.detalle_base on bc.IdBase equals db.IdBase
                                      join u in _context.usuarios on ca.IdUsuarioV equals u.IdUsuario into usuarioJoin
                                      from u in usuarioJoin.DefaultIfEmpty()
                                      where ca.IdUsuarioS == idSupervisorActual 
                                                && db.FechaCarga >= fechaInicio 
                                                && db.FechaCarga <= fechaFin
                                      group new { db, bc, ca, ce } by db.IdBase into grouped
                                      select new
                                      {
                                          Idbase = grouped.Key,
                                          LatestRecord = grouped.OrderByDescending(x => x.db.FechaCarga)
                                                                 .FirstOrDefault(),
                                      })
                                     .ToList();*/

                var detallesClientesSupervisor = supervisorData.Select(detallesClientes => new
                {
                    DNI = detallesClientes.Dni,
                    X_APPATERNO = detallesClientes.XAppaterno,
                    X_APMATERNO = detallesClientes.XApmaterno,
                    X_NOMBRE = detallesClientes.XNombre,
                    EDAD = detallesClientes.Edad,
                    DEPARTAMENTO = detallesClientes.Departamento,
                    PROVINCIA = detallesClientes.Provincia,
                    DISTRITO = detallesClientes.Distrito,
                    CAMPANA = detallesClientes.Campaña,
                    OFERTA_MAX = detallesClientes.OfertaMax,
                    TASA_MINIMA = detallesClientes.TasaMinima,
                    SUCURSAL_COMERCIAL = detallesClientes.SucursalComercial,
                    AGENCIA_COMERCIAL = detallesClientes.AgenciaComercial,
                    PLAZO = detallesClientes.Plazo,
                    CUOTA = detallesClientes.Cuota,
                    USUARIO = detallesClientes.Usuario,
                    USERV3 = detallesClientes.UserV3,
                    FLAG_DEUDA_V_OFERTA = detallesClientes.FlagDeudaVOferta,
                    PERFILRO = detallesClientes.PerfilRo,
                    PROPENSIONV2 = detallesClientes.Propensionv2,

                    OFERTA12M = detallesClientes.Oferta12m,
                    TASA12M = detallesClientes.Tasa12m,
                    CUOTA12M = detallesClientes.Cuota12m,
                    OFERTA18M = detallesClientes.Oferta18m,
                    TASA18M = detallesClientes.Tasa18m,
                    CUOTA18M = detallesClientes.Cuota18m,
                    OFERTA24M = detallesClientes.Oferta24m,
                    TASA24M = detallesClientes.Tasa24m,
                    CUOTA24M = detallesClientes.Cuota24m,
                    OFERTA36M = detallesClientes.Oferta36m,
                    TASA36M = detallesClientes.Tasa36m,
                    CUOTA36M = detallesClientes.Cuota36m,

                    GRUPO_TASA = detallesClientes.GrupoTasa,
                    GRUPO_MONTO = detallesClientes.GrupoMonto,
                    PROPENSION = detallesClientes.Propension,
                    TIPO_CLIENTE = detallesClientes.TipoCliente,
                    CLIENTE_NUEVO = detallesClientes.ClienteNuevo,
                    COLOR = detallesClientes.Color,
                    COLOR_FINAL = detallesClientes.ColorFinal,
                    PRIORIDAD_SISTEMA = detallesClientes.PrioridadSistema,
                    TELEFONO1 = detallesClientes.Telefono1,
                    TELEFONO2 = detallesClientes.Telefono2,
                    TELEFONO3 = detallesClientes.Telefono3,
                    TELEFONO4 = detallesClientes.Telefono4,
                    TELEFONO5 = detallesClientes.Telefono5,
                }).ToList();

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Clientes Asignados");

                    // Encabezados
                    worksheet.Cells[1, 1].Value = "DNI";
                    worksheet.Cells[1, 2].Value = "X_APPATERNO";
                    worksheet.Cells[1, 3].Value = "X_APMATERNO";
                    worksheet.Cells[1, 4].Value = "X_NOMBRE";
                    worksheet.Cells[1, 5].Value = "EDAD";
                    worksheet.Cells[1, 6].Value = "DEPARTAMENTO";
                    worksheet.Cells[1, 7].Value = "PROVINCIA";
                    worksheet.Cells[1, 8].Value = "DISTRITO";
                    worksheet.Cells[1, 9].Value = "CAMPANA";
                    worksheet.Cells[1, 10].Value = "OFERTA_MAX";
                    worksheet.Cells[1, 11].Value = "TASA_MINIMA";
                    worksheet.Cells[1, 12].Value = "SUCURSAL_COMERCIAL";
                    worksheet.Cells[1, 13].Value = "AGENCIA_COMERCIAL";
                    worksheet.Cells[1, 14].Value = "PLAZO";
                    worksheet.Cells[1, 15].Value = "CUOTA";

                    worksheet.Cells[1, 16].Value = "OFERTA12M";
                    worksheet.Cells[1, 17].Value = "TASA12M";
                    worksheet.Cells[1, 18].Value = "CUOTA12M";
                    worksheet.Cells[1, 19].Value = "OFERTA18M";
                    worksheet.Cells[1, 20].Value = "TASA18M";
                    worksheet.Cells[1, 21].Value = "CUOTA18M";
                    worksheet.Cells[1, 22].Value = "OFERTA24M";
                    worksheet.Cells[1, 23].Value = "TASA24M";
                    worksheet.Cells[1, 24].Value = "CUOTA24M";
                    worksheet.Cells[1, 25].Value = "OFERTA36M";
                    worksheet.Cells[1, 26].Value = "TASA36M";
                    worksheet.Cells[1, 27].Value = "CUOTA36M";

                    worksheet.Cells[1, 28].Value = "GRUPO_TASA";
                    worksheet.Cells[1, 29].Value = "GRUPO_MONTO";
                    worksheet.Cells[1, 30].Value = "PROPENSION";
                    worksheet.Cells[1, 31].Value = "TIPO_CLIENTE";
                    worksheet.Cells[1, 32].Value = "CLIENTE_NUEVO";
                    worksheet.Cells[1, 33].Value = "COLOR";
                    worksheet.Cells[1, 34].Value = "COLOR_FINAL";

                    worksheet.Cells[1, 35].Value = "USUARIO";
                    worksheet.Cells[1, 36].Value = "USUARIO V3";
                    worksheet.Cells[1, 37].Value = "FLAG DEUDA V OFERTA";
                    worksheet.Cells[1, 38].Value = "PERFIL_RO";
                    worksheet.Cells[1, 39].Value = "PRIORIDAD";

                    worksheet.Cells[1, 40].Value = "TELEFONO1";
                    worksheet.Cells[1, 41].Value = "TELEFONO2";
                    worksheet.Cells[1, 42].Value = "TELEFONO3";
                    worksheet.Cells[1, 43].Value = "TELEFONO4";
                    worksheet.Cells[1, 44].Value = "TELEFONO5";
                    // Llena los datos
                    int row = 2;
                    foreach (var data in detallesClientesSupervisor)
                    {
                        worksheet.Cells[row, 1].Value = data.DNI;
                        worksheet.Cells[row, 2].Value = data.X_APPATERNO;
                        worksheet.Cells[row, 3].Value = data.X_APMATERNO;
                        worksheet.Cells[row, 4].Value = data.X_NOMBRE;
                        worksheet.Cells[row, 5].Value = data.EDAD;
                        worksheet.Cells[row, 6].Value = data.DEPARTAMENTO;
                        worksheet.Cells[row, 7].Value = data.PROVINCIA;
                        worksheet.Cells[row, 8].Value = data.DISTRITO;
                        worksheet.Cells[row, 9].Value = data.CAMPANA;
                        worksheet.Cells[row, 10].Value = data.OFERTA_MAX;
                        worksheet.Cells[row, 11].Value = data.TASA_MINIMA;
                        worksheet.Cells[row, 12].Value = data.SUCURSAL_COMERCIAL;
                        worksheet.Cells[row, 13].Value = data.AGENCIA_COMERCIAL;
                        worksheet.Cells[row, 14].Value = data.PLAZO;
                        worksheet.Cells[row, 15].Value = data.CUOTA;

                        worksheet.Cells[row, 16].Value = data.OFERTA12M;
                        worksheet.Cells[row, 17].Value = data.TASA12M;
                        worksheet.Cells[row, 18].Value = data.CUOTA12M;
                        worksheet.Cells[row, 19].Value = data.OFERTA18M;
                        worksheet.Cells[row, 20].Value = data.TASA18M;
                        worksheet.Cells[row, 21].Value = data.CUOTA18M;
                        worksheet.Cells[row, 22].Value = data.OFERTA24M;
                        worksheet.Cells[row, 23].Value = data.TASA24M;
                        worksheet.Cells[row, 24].Value = data.CUOTA24M;
                        worksheet.Cells[row, 25].Value = data.OFERTA36M;
                        worksheet.Cells[row, 26].Value = data.TASA36M;
                        worksheet.Cells[row, 27].Value = data.CUOTA36M;

                        worksheet.Cells[row, 28].Value = data.GRUPO_TASA;
                        worksheet.Cells[row, 29].Value = data.GRUPO_MONTO;
                        worksheet.Cells[row, 30].Value = data.PROPENSION;
                        worksheet.Cells[row, 31].Value = data.TIPO_CLIENTE;
                        worksheet.Cells[row, 32].Value = data.CLIENTE_NUEVO;
                        worksheet.Cells[row, 33].Value = data.COLOR;
                        worksheet.Cells[row, 34].Value = data.COLOR_FINAL;

                        worksheet.Cells[row, 35].Value = data.USUARIO;
                        worksheet.Cells[row, 36].Value = data.USERV3;
                        worksheet.Cells[row, 37].Value = data.FLAG_DEUDA_V_OFERTA;
                        worksheet.Cells[row, 38].Value = data.PERFILRO;
                        worksheet.Cells[row, 39].Value = data.PRIORIDAD_SISTEMA;

                        worksheet.Cells[row, 40].Value = data.TELEFONO1;
                        worksheet.Cells[row, 41].Value = data.TELEFONO2;
                        worksheet.Cells[row, 42].Value = data.TELEFONO3;
                        worksheet.Cells[row, 43].Value = data.TELEFONO4;
                        worksheet.Cells[row, 44].Value = data.TELEFONO5;

                        row++;
                    }

                    // Estiliza las columnas
                    worksheet.Cells.AutoFitColumns();

                    // Devuelve el csvFile
                    var excelBytes = package.GetAsByteArray();
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{supervisorUsuario.NombresCompletos} - {filtroDescarga} - ({detallesClientesSupervisor.Count()}).xlsx");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Redireccionar", "Error");
            }
        }

        [HttpGet("DownloadAsignaciones")]
        public async Task<IActionResult> DownloadAsignaciones(string nombre_lista, int page = -1)
        {
            try
            {
                var result = await useCaseDownloadAsignaciones.exec(nombre_lista, page);
                if (!result.success)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = result.message
                    });
                }
                var data = result.data;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Asignaciones");

                // 1. Metadatos en fila 1 y 2, columnas desde la columna 1
                var metadata = new Dictionary<(int row, int col), object>
                {
                    [(1, 1)] = "Nombre de la Lista",
                    [(1, 2)] = "DNI del Supervisor",
                    [(1, 3)] = "Nombres del Supervisor",
                    [(1, 4)] = "Fecha de Creación de la Lista",
                    [(1, 5)] = "Total Asignaciones",
                    [(1, 6)] = "Total Asignaciones Gestionadas",
                    [(1, 7)] = "Total Asignaciones Asignadas a Asesores",
                    [(1, 8)] = "Total Asignaciones Sin Gestionar",
                    [(1, 9)] = "Total Asignaciones Sin Asignar",

                    [(2, 1)] = data.nombre_lista,
                    [(2, 2)] = data.dni_supervisor,
                    [(2, 3)] = data.nombres_supervisor,
                    [(2, 4)] = data.fecha_creacion_lista.ToString("yyyy-MM-dd"),
                    [(2, 5)] = data.total_asignaciones,
                    [(2, 6)] = data.total_asignaciones_gestionadas,
                    [(2, 7)] = data.total_asignaciones_asignadas_a_asesores,
                    [(2, 8)] = data.total_asignaciones_sin_gestionar,
                    [(2, 9)] = data.total_asignaciones_sin_asignar
                };

                foreach (var entry in metadata)
                {
                    var (r, c) = entry.Key;
                    worksheet.Cells[r, c].Value = entry.Value;
                    worksheet.Cells[r, c].Style.Font.Bold = true;
                    worksheet.Cells[r, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[r, c].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    worksheet.Cells[r, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // 2. Cabecera (fila 4)
                var headers = new[]
                {
                    "Id Asignación", "Fecha Asignación", "Id UsuarioV",
                    "Teléfono 1", "Teléfono 2", "Teléfono 3", "Teléfono 4", "Teléfono 5",
                    "Email 1", "Email 2", "Nombre Cliente", "Oferta Max", "Tipo Base",
                    "DNI", "Apellido Paterno", "Apellido Materno", "Nombres", "Edad",
                    "Departamento", "Provincia", "Distrito", "Campaña", "Tasa Mínima",
                    "Sucursal Comercial", "Agencia Comercial", "Plazo", "Cuota",
                    "Oferta 12m", "Tasa 12m", "Cuota 12m",
                    "Oferta 18m", "Tasa 18m", "Cuota 18m",
                    "Oferta 24m", "Tasa 24m", "Cuota 24m",
                    "Oferta 36m", "Tasa 36m", "Cuota 36m",
                    "Grupo Tasa", "Grupo Monto", "Propensión",
                    "Tipo Cliente", "Cliente Nuevo", "Color", "Color Final",
                    "Usuario", "UserV3", "Flag Deuda v Oferta", "Perfil RO", "Prioridad"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[4, i + 1].Value = headers[i];
                    worksheet.Cells[4, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[4, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[4, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet.Cells[4, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // 3. Datos (desde fila 5)
                var row = 5;
                foreach (var a in data.asignaciones_detalladas)
                {
                    var values = new object?[]
                    {
                        a.IdAsignacion,
                        a.FechaAsignacionSup?.ToString("yyyy-MM-dd"),
                        a.IdUsuarioV,
                        a.Telefono1, a.Telefono2, a.Telefono3, a.Telefono4, a.Telefono5,
                        a.Email1, a.Email2, a.ClienteNombreCompleto, a.OfertaMax, a.TipoBase,
                        a.Dni, a.XAppaterno, a.XApmaterno, a.XNombre, a.Edad,
                        a.Departamento, a.Provincia, a.Distrito, a.Campaña, a.TasaMinima,
                        a.SucursalComercial, a.AgenciaComercial, a.Plazo, a.Cuota,
                        a.Oferta12m, a.Tasa12m, a.Cuota12m,
                        a.Oferta18m, a.Tasa18m, a.Cuota18m,
                        a.Oferta24m, a.Tasa24m, a.Cuota24m,
                        a.Oferta36m, a.Tasa36m, a.Cuota36m,
                        a.GrupoTasa, a.GrupoMonto, a.Propension,
                        a.TipoCliente, a.ClienteNuevo, a.Color, a.ColorFinal,
                        a.Usuario, a.UserV3, a.FlagDeudaVOferta, a.PerfilRo, a.Prioridad
                    };

                    for (int col = 0; col < values.Length; col++)
                    {
                        worksheet.Cells[row, col + 1].Value = values[col];
                        worksheet.Cells[row, col + 1].Style.Border.BorderAround(ExcelBorderStyle.Hair);
                    }

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                var fileName = $"Asignaciones_{data.dni_supervisor}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar las asignaciones");
                return Json(new
                {
                    isSuccess = false,
                    message = "Error al descargar las asignaciones"
                });
            }
        }
        [HttpGet("DownloadDerivacionesAsync")]
        public async Task<IActionResult> DownloadDerivacionesAsync(
            string? filtro,
            string? campo,
            DateTime? fecha_inicio,
            DateTime? fecha_final)
        {
            try
            {
                var asignaciones = await dAO_DescargarDerivaciones.GetDerivacionesForDownload(filtro, campo, fecha_inicio, fecha_final);
                var data = asignaciones.Data;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Asignaciones");

                // 1. Metadatos en fila 1 y 2, columnas desde la columna 1
                var metadata = new Dictionary<(int row, int col), object>
                {
                    [(1, 1)] = "Nombre de la Lista",
                    [(1, 2)] = "DNI del Supervisor",
                    [(1, 3)] = "Nombres del Supervisor",
                    [(1, 4)] = "Fecha de Creación de la Lista",
                    [(1, 5)] = "Total Asignaciones",
                    [(1, 6)] = "Total Asignaciones Gestionadas",
                    [(1, 7)] = "Total Asignaciones Asignadas a Asesores",
                    [(1, 8)] = "Total Asignaciones Sin Gestionar",
                    [(1, 9)] = "Total Asignaciones Sin Asignar",
                    [(1, 10)] = "Total Asignaciones Derivadas",

                    [(2, 1)] = data.nombre_lista,
                    [(2, 2)] = data.dni_supervisor,
                    [(2, 3)] = data.nombres_supervisor,
                    [(2, 4)] = data.fecha_creacion_lista.ToString("yyyy-MM-dd"),
                    [(2, 5)] = data.total_asignaciones,
                    [(2, 6)] = data.total_asignaciones_gestionadas,
                    [(2, 7)] = data.total_asignaciones_asignadas_a_asesores,
                    [(2, 8)] = data.total_asignaciones_sin_gestionar,
                    [(2, 9)] = data.total_asignaciones_sin_asignar,
                    [(2, 10)] = data.total_asignaciones_derivadas
                };

                foreach (var entry in metadata)
                {
                    var (r, c) = entry.Key;
                    worksheet.Cells[r, c].Style.WrapText = true;
                    worksheet.Cells[r, c].Value = entry.Value;
                    worksheet.Cells[r, c].Style.Font.Bold = true;
                    worksheet.Cells[r, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[r, c].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                    worksheet.Cells[r, c].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // 2. Cabecera (fila 4)
                var headers = new[]
                {
                    "Id Asignación", "Fecha Asignación", "Id UsuarioV",
                    "Teléfono 1", "Teléfono 2", "Teléfono 3", "Teléfono 4", "Teléfono 5",
                    "Email 1", "Email 2", "Nombre Cliente", "Oferta Max", "Tipo Base",
                    "DNI", "Apellido Paterno", "Apellido Materno", "Nombres", "Edad",
                    "Departamento", "Provincia", "Distrito", "Campaña", "Tasa Mínima",
                    "Sucursal Comercial", "Agencia Comercial", "Plazo", "Cuota",
                    "Oferta 12m", "Tasa 12m", "Cuota 12m",
                    "Oferta 18m", "Tasa 18m", "Cuota 18m",
                    "Oferta 24m", "Tasa 24m", "Cuota 24m",
                    "Oferta 36m", "Tasa 36m", "Cuota 36m",
                    "Grupo Tasa", "Grupo Monto", "Propensión",
                    "Tipo Cliente", "Cliente Nuevo", "Color", "Color Final",
                    "Usuario", "UserV3", "Flag Deuda v Oferta", "Perfil RO", "Prioridad",
                    "Teléfono Derivado", "Fecha Derivación", "Nombre Agencia Derivada", "Fecha Visita", "Oferta Max Derivada",
                    "Documento del Asesor", "Nombre del Asesor"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[4, i + 1].Value = headers[i];
                    worksheet.Cells[4, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[4, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[4, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet.Cells[4, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // 3. Datos (desde fila 5)
                var row = 5;
                foreach (var a in data.asignaciones_detalladas)
                {
                    var values = new object?[]
                    {
                        a.IdAsignacion,
                        a.FechaAsignacionSup?.ToString("yyyy-MM-dd"),
                        a.IdUsuarioV,
                        a.Telefono1, a.Telefono2, a.Telefono3, a.Telefono4, a.Telefono5,
                        a.Email1, a.Email2, a.ClienteNombreCompleto, a.OfertaMax, a.TipoBase,
                        a.Dni, a.XAppaterno, a.XApmaterno, a.XNombre, a.Edad,
                        a.Departamento, a.Provincia, a.Distrito, a.Campaña, a.TasaMinima,
                        a.SucursalComercial, a.AgenciaComercial, a.Plazo, a.Cuota,
                        a.Oferta12m, a.Tasa12m, a.Cuota12m,
                        a.Oferta18m, a.Tasa18m, a.Cuota18m,
                        a.Oferta24m, a.Tasa24m, a.Cuota24m,
                        a.Oferta36m, a.Tasa36m, a.Cuota36m,
                        a.GrupoTasa, a.GrupoMonto, a.Propension,
                        a.TipoCliente, a.ClienteNuevo, a.Color, a.ColorFinal,
                        a.Usuario, a.UserV3, a.FlagDeudaVOferta, a.PerfilRo, a.Prioridad,
                        a.TelefonoDerivado, a.FechaDerivacion?.ToString("yyyy-MM-dd"),
                        a.NombreAgenciaDerivada, a.FechaVisita?.ToString("yyyy-MM-dd"),
                        a.OfertaMaxDerivada,
                        a.DocAsesor,
                        a.NombreAsesor
                    };

                    for (int col = 0; col < values.Length; col++)
                    {
                        worksheet.Cells[row, col + 1].Value = values[col];
                        worksheet.Cells[row, col + 1].Style.Border.BorderAround(ExcelBorderStyle.Hair);
                    }

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                var fileName = $"Derivaciones_{data.dni_supervisor}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al descargar las derivaciones");
                return Json(new
                {
                    isSuccess = false,
                    message = "Error al descargar las derivaciones"
                });
            }
        }
    }
}