using ALFINapp.API.Filters;
using ALFINapp.Application.Interfaces.Asignaciones;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class ExcelController : Controller
    {
        private readonly MDbContext _context;
        private readonly IUseCaseDownloadAsignaciones useCaseDownloadAsignaciones;
        public ExcelController(
            MDbContext context,
            IUseCaseDownloadAsignaciones useCaseDownloadAsignaciones)
        {
            _context = context;
            this.useCaseDownloadAsignaciones = useCaseDownloadAsignaciones;
        }

        private bool IsValidDni(float dni)
        {
            // Convertir el DNI a string
            string dniString = dni.ToString();

            // Comprobar que el DNI tenga exactamente 8 caracteres y que todos sean números
            return dniString.Length > 7;
        }


        [HttpGet]
        public IActionResult DescargarClientesAsignados(DateTime? fechaInicio, DateTime? fechaFin, string? destinoBase)
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

                DateTime fechaInicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime fechaFinMes = fechaInicioMes.AddMonths(1).AddDays(-1).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                var query = from ca in _context.clientes_asignados
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
                                ca.IdAsignacion
                            };

                // Aplicar filtros antes de ejecutar la consulta
                if (fechaInicio.HasValue)
                {
                    query = query.Where(ca => ca.FechaAsignacionSup >= fechaInicio);
                }
                if (fechaFin.HasValue)
                {
                    query = query.Where(ca => ca.FechaAsignacionSup <= fechaFin);
                }
                if (!string.IsNullOrEmpty(destinoBase))
                {
                    query = query.Where(ca => ca.Destino == destinoBase);
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

                // Genera el csvFile Excel
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
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{supervisorUsuario.NombresCompletos} - {destinoBase} - ({detallesClientesSupervisor.Count()}).xlsx");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Redireccionar", "Error");
            }
        }

        [HttpGet]
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

                // Encabezados
                var headers = new[]
                {
                    "Id Asignación", "Fecha Asignación", "Id UsuarioV",
                    "Teléfono 1", "Teléfono 2", "Teléfono 3", "Teléfono 4", "Teléfono 5",
                    "Email 1", "Email 2", "Nombre Cliente", "Oferta Max", "Tipo Base"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    worksheet.Cells[1, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                var row = 2;
                foreach (var asignacion in data.asignaciones_detalladas)
                {
                    worksheet.Cells[row, 1].Value = asignacion.IdAsignacion;
                    worksheet.Cells[row, 2].Value = asignacion.FechaAsignacionSup.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 3].Value = asignacion.IdUsuarioV;
                    worksheet.Cells[row, 4].Value = asignacion.Telefono1;
                    worksheet.Cells[row, 5].Value = asignacion.Telefono2;
                    worksheet.Cells[row, 6].Value = asignacion.Telefono3;
                    worksheet.Cells[row, 7].Value = asignacion.Telefono4;
                    worksheet.Cells[row, 8].Value = asignacion.Telefono5;
                    worksheet.Cells[row, 9].Value = asignacion.Email1;
                    worksheet.Cells[row, 10].Value = asignacion.Email2;
                    worksheet.Cells[row, 11].Value = asignacion.ClienteNombre;
                    worksheet.Cells[row, 12].Value = asignacion.OfertaMax;
                    worksheet.Cells[row, 13].Value = asignacion.TipoBase;

                    for (int col = 1; col <= 13; col++)
                        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Hair);

                    row++;
                }

                var metadata = new Dictionary<(int row, int col), object>
                {
                    [(4, 16)] = "Nombre de la Lista",
                    [(4, 17)] = "DNI del Supervisor",
                    [(4, 18)] = "Nombres del Supervisor",
                    [(4, 19)] = "Fecha de Creación de la Lista",
                    [(4, 20)] = "Total Asignaciones",
                    [(4, 21)] = "Total Asignaciones Gestionadas",
                    [(4, 22)] = "Total Asignaciones Asignadas a Asesores",
                    [(4, 23)] = "Total Asignaciones Sin Gestionar",
                    [(4, 24)] = "Total Asignaciones Sin Asignar",

                    [(5, 16)] = data.nombre_lista,
                    [(5, 17)] = data.dni_supervisor,
                    [(5, 18)] = data.nombres_supervisor,
                    [(5, 19)] = data.fecha_creacion_lista.ToString("yyyy-MM-dd"),
                    [(5, 20)] = data.total_asignaciones,
                    [(5, 21)] = data.total_asignaciones_gestionadas,
                    [(5, 22)] = data.total_asignaciones_asignadas_a_asesores,
                    [(5, 23)] = data.total_asignaciones_sin_gestionar,
                    [(5, 24)] = data.total_asignaciones_sin_asignar
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

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileBytes = package.GetAsByteArray();
                var fileName = $"Asignaciones_{data.dni_supervisor}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                return File(fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Error al descargar las asignaciones"
                });
            }
        }
        /*[HttpGet]
        public IActionResult SubircsvFile(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Message"] = "Por favor, seleccione un csvFile valido. ";
                return RedirectToAction("Index", "Home");
            }
            int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
            if (idSupervisorActual == null)
            {
                TempData["Message"] = "Error en la autenticación. Intente iniciar sesión nuevamente.";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                // Leer el csvFile CSV
                using (var reader = new StreamReader(csvFile.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    // Registrar mapeo personalizado
                    csv.Context.RegisterClassMap<SubidaDeArchivosDTOMapping>();

                    // Obtener los encabezados del csvFile
                    csv.Read();
                    csv.ReadHeader();
                    var encabezadoscsvFile = csv.HeaderRecord;

                    // Encabezados esperados según el DTO
                    var encabezadosEsperados = new[]
                    {
                        "COD_CANAL", "CANAL", "DNI", "FECHA_ENVIO", "FECHA_GESTION",
                        "HORA_GESTION", "TELEFONO", "ORIGEN_TELEFONO", "COD_CAMPAÑA", "COD_TIP",
                        "OFERTA", "DNI_ASESOR"
                    };

                    // Verificar que los encabezados coincidan
                    var faltantes = encabezadosEsperados.Except(encabezadoscsvFile).ToList();
                    if (faltantes.Any())
                    {
                        TempData["Message"] = $"Faltan los siguientes encabezados: {string.Join(", ", faltantes)}";
                        return RedirectToAction("Index", "Home");
                    }

                    // Leer y procesar los datos del csvFile
                    var registros = new List<SubidaDeArchivosDTO>();
                    while (csv.Read())
                    {
                        var registro = csv.GetRecord<SubidaDeArchivosDTO>();

                        // Validación del DNI
                        if (registro.DNI == null)
                        {
                            TempData["Message"] = $"El DNI {registro.DNI} no es valido. Debe ser un numero.";
                            return RedirectToAction("Index", "Home");
                        }

                        // Validación de otros campos del DTO
                        if (registro.FECHA_ENVIO == null || registro.FECHA_GESTION == null || string.IsNullOrEmpty(registro.COD_CANAL))
                        {
                            TempData["Message"] = $"Algunos campos obligatorios están vacíos en el registro con DNI {registro.DNI}.";
                            return RedirectToAction("Index", "Home");
                        }

                        // Verificar si el registro ya existe
                        var registroExistente = _context.SUBIR_FEED
                            .AsNoTracking()
                            .Any(sf => sf.Dni == registro.DNI);
                        if (registroExistente)
                        {
                            TempData["Message"] = $"El registro con DNI {registro.DNI} ya existe. La subida de archivos se canceló.";
                            return RedirectToAction("Inicio", "Supervisor");
                        }

                        // Si todas las validaciones pasan, agregar el registro a la lista
                        registros.Add(registro);
                    }

                    // Procesar y guardar los registros válidos
                    foreach (var registro in registros)
                    {
                        var modelf = new SubirFeed
                        {
                            CodCanal = registro.COD_CANAL,
                            Canal = registro.CANAL,
                            Dni = registro.DNI,
                            FechaEnvio = registro.FECHA_ENVIO,
                            FechaGestion = registro.FECHA_GESTION,
                            HoraGestion = registro.HORA_GESTION,
                            Telefono = registro.TELEFONO,
                            OrigenTelefono = registro.ORIGEN_TELEFONO,
                            CodCampana = registro.COD_CAMPAÑA,
                            CodTip = registro.COD_TIP,
                            Oferta = registro.OFERTA,
                            DniAsesor = registro.DNI_ASESOR
                        };

                        // Guardar el modelo en la base de datos
                        _context.SUBIR_FEED.Add(modelf);

                        var modelforFechas = new CargaManualCsv
                        {
                            IdUsuario = idSupervisorActual,
                            FechaDeCarga = DateTime.Now,
                            DniUsuarioAgregado = registro.DNI
                        };

                        _context.carga_manual_csv.Add(modelforFechas);
                    }

                    // Guardar los cambios en la base de datos
                    _context.SaveChanges();

                    TempData["Message"] = "csvFile procesado correctamente.";
                }

                return RedirectToAction("Inicio", "Supervisor");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al procesar el csvFile: {ex.Message}");
                TempData["Message"] = "Ocurrió un error al procesar el csvFile. Verifique su formato y vuelva a intentarlo.";
                return RedirectToAction("Inicio", "Supervisor");
            }
        }*/
    }
}