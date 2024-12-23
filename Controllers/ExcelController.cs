using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using OfficeOpenXml;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using System.Globalization; // Necesaria para CultureInfo
using System.IO;           // Necesaria para StreamReader
using System.Linq;         // Necesaria para operaciones como Except
using System.Collections.Generic;
using System.Security.AccessControl; // Necesaria para List<>

namespace ALFINapp.Controllers
{
    public class ExcelController : Controller
    {
        private readonly MDbContext _context;
        public ExcelController(MDbContext context)
        {
            _context = context;
        }

        private bool IsValidDni(float dni)
        {
            // Convertir el DNI a string
            string dniString = dni.ToString();

            // Comprobar que el DNI tenga exactamente 8 caracteres y que todos sean números
            return dniString.Length > 7;
        }


        [HttpGet]
        public IActionResult DescargarClientesAsignados()
        {
            try
            {
                int? idSupervisorActual = HttpContext.Session.GetInt32("UsuarioId");
                if (idSupervisorActual == null)
                {
                    TempData["Message"] = "Error en la autenticación. Intente iniciar sesión nuevamente.";
                    return RedirectToAction("Index", "Home");
                }
                /*
                    Aca debemos descargar los datos en formato xlsx de excel te mostrare las tablas y demas
                    Todos los valores de clientes_tipificados, todos las entradas de base_cliente, todas las
                    entradas de clientes_enriquecidos, etc. 

                    Te mostrare una consulta que maso debe ser asi

                    var supervisorData = from ca in _context.clientes_asignados
                                 join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                 join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                                 join u in _context.usuarios on ca.IdUsuarioV equals u.IdUsuario into usuarioJoin
                                 from u in usuarioJoin.DefaultIfEmpty()
                                 where ca.IdUsuarioS == usuarioId
                                 select new SupervisorDTO
                                 {
                                     IdAsignacion = ca.IdAsignacion,
                                     IdCliente = ca.IdCliente,
                                     idUsuarioV = ca.IdUsuarioV.HasValue ? ca.IdUsuarioV.Value : 0,
                                     FechaAsignacionV = ca.FechaAsignacionVendedor,

                                     Dni = bc.Dni,
                                     XAppaterno = bc.XAppaterno,
                                     XApmaterno = bc.XApmaterno,
                                     XNombre = bc.XNombre,

                                     NombresCompletos = u != null ? u.NombresCompletos : "Vendedor no Asignado",
                                     ApellidoPaterno = u != null ? u.ApellidoPaterno : "No disponible", // Manejo de null para ApellidoPaterno
                                     DniVendedor = u != null ? u.Dni : " ",
                                 };

                    lUEGO POR CADA CLIENTE DEBES DE PODER MOSTRAR SUS TIPIFICACIONES ALGO COMO ESTO

                    var resultados_telefonos_tipificados_vendedor = (from ta in _context.telefonos_agregados
                                                             join ce in _context.clientes_enriquecidos on ta.IdCliente equals ce.IdCliente
                                                             where ce.IdBase == id_base
                                                             join ct in (
                                                                 from ct_sub in _context.clientes_tipificados
                                                                 join ultimas in (
                                                                     from sub_ct in _context.clientes_tipificados
                                                                     group sub_ct by sub_ct.TelefonoTipificado into g
                                                                     select new { TelefonoTipificado = g.Key, UltimaFecha = g.Max(x => x.FechaTipificacion) }
                                                                 ) on ct_sub.TelefonoTipificado equals ultimas.TelefonoTipificado
                                                                 where ct_sub.FechaTipificacion == ultimas.UltimaFecha
                                                                 select new { ct_sub.TelefonoTipificado, ct_sub.IdTipificacion }
                                                             ) on ta.Telefono equals ct.TelefonoTipificado into ctJoin
                                                             from ct in ctJoin.DefaultIfEmpty()
                                                             join t in _context.tipificaciones on ct.IdTipificacion equals t.IdTipificacion into tJoin
                                                             from t in tJoin.DefaultIfEmpty()
                                                             select new
                                                             {
                                                                 TelefonoTipificado = ta.Telefono,
                                                                 DescripcionTipificacion = t.DescripcionTipificacion,
                                                                 ComentarioTelefono = ta.Comentario
                                                             }).ToList();

                    PERO ACA SOLO CONSIGUE LAS TIPIFICACIONES DE 1 USUARIO, ACA DEBE ENCONTRAR TODAS LAS TIPIFICACIONES DE 
                    TODOS LOS USUARIOS ASIGNADOS A AQUEL SUPERVISOR, COMO TAL ESTA PARTE DE SER POSIBLE ASIGNALA EN OTRO
                    csvFile DE EXCEL
                 */

                // Obtén los datos necesarios (simplificado para mostrar la idea)
                var supervisorData = (from ca in _context.clientes_asignados
                                      join ce in _context.clientes_enriquecidos on ca.IdCliente equals ce.IdCliente
                                      join bc in _context.base_clientes on ce.IdBase equals bc.IdBase
                                      join db in _context.detalle_base on bc.IdBase equals db.IdBase
                                      join u in _context.usuarios on ca.IdUsuarioV equals u.IdUsuario into usuarioJoin
                                      from u in usuarioJoin.DefaultIfEmpty()
                                      where ca.IdUsuarioS == idSupervisorActual
                                      group new { db, bc, ca, ce } by db.IdBase into grouped
                                      select new
                                      {
                                          Idbase = grouped.Key,
                                          LatestRecord = grouped.OrderByDescending(x => x.db.FechaCarga)
                                                                 .FirstOrDefault(),
                                      })
                                     .ToList();

                var detallesClientesSupervisor = supervisorData.Select(detallesClientes => new
                {
                    DNI = detallesClientes.LatestRecord.bc.Dni,
                    X_APPATERNO = detallesClientes.LatestRecord.bc.XAppaterno,
                    X_APMATERNO = detallesClientes.LatestRecord.bc.XApmaterno,
                    X_NOMBRE = detallesClientes.LatestRecord.bc.XNombre,
                    EDAD = detallesClientes.LatestRecord.bc.Edad,
                    DEPARTAMENTO = detallesClientes.LatestRecord.bc.Departamento,
                    PROVINCIA = detallesClientes.LatestRecord.bc.Provincia,
                    DISTRITO = detallesClientes.LatestRecord.bc.Distrito,
                    CAMPANA = detallesClientes.LatestRecord.db.Campaña,
                    OFERTA_MAX = detallesClientes.LatestRecord.db.OfertaMax,
                    TASA_MINIMA = detallesClientes.LatestRecord.db.TasaMinima,
                    SUCURSAL_COMERCIAL = detallesClientes.LatestRecord.db.SucursalComercial,
                    AGENCIA_COMERCIAL = detallesClientes.LatestRecord.db.AgenciaComercial,
                    PLAZO = detallesClientes.LatestRecord.db.Plazo,
                    CUOTA = detallesClientes.LatestRecord.db.Cuota,

                    OFERTA12M = detallesClientes.LatestRecord.db.Oferta12m,
                    TASA12M = detallesClientes.LatestRecord.db.Tasa12m,
                    CUOTA12M = detallesClientes.LatestRecord.db.Cuota12m,
                    OFERTA18M = detallesClientes.LatestRecord.db.Oferta18m,
                    TASA18M = detallesClientes.LatestRecord.db.Tasa18m,
                    CUOTA18M = detallesClientes.LatestRecord.db.Cuota18m,
                    OFERTA24M = detallesClientes.LatestRecord.db.Oferta24m,
                    TASA24M = detallesClientes.LatestRecord.db.Tasa24m,
                    CUOTA24M = detallesClientes.LatestRecord.db.Cuota24m,
                    OFERTA36M = detallesClientes.LatestRecord.db.Oferta36m,
                    TASA36M = detallesClientes.LatestRecord.db.Tasa36m,
                    CUOTA36M = detallesClientes.LatestRecord.db.Cuota36m,

                    GRUPO_TASA = detallesClientes.LatestRecord.db.GrupoTasa,
                    GRUPO_MONTO = detallesClientes.LatestRecord.db.GrupoMonto,
                    PROPENSION = detallesClientes.LatestRecord.db.Propension,
                    TIPO_CLIENTE = detallesClientes.LatestRecord.db.TipoCliente,
                    CLIENTE_NUEVO = detallesClientes.LatestRecord.db.ClienteNuevo,
                    COLOR = detallesClientes.LatestRecord.db.Color,
                    COLOR_FINAL = detallesClientes.LatestRecord.db.ColorFinal,
                    TELEFONO1 = detallesClientes.LatestRecord.ce.Telefono1,
                    TELEFONO2 = detallesClientes.LatestRecord.ce.Telefono2,
                    TELEFONO3 = detallesClientes.LatestRecord.ce.Telefono3,
                    TELEFONO4 = detallesClientes.LatestRecord.ce.Telefono4,
                    TELEFONO5 = detallesClientes.LatestRecord.ce.Telefono5,
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
                    worksheet.Cells[1, 35].Value = "TELEFONO1";
                    worksheet.Cells[1, 36].Value = "TELEFONO2";
                    worksheet.Cells[1, 37].Value = "TELEFONO3";
                    worksheet.Cells[1, 38].Value = "TELEFONO4";
                    worksheet.Cells[1, 39].Value = "TELEFONO5";

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
                        worksheet.Cells[row, 35].Value = data.TELEFONO1;
                        worksheet.Cells[row, 36].Value = data.TELEFONO2;
                        worksheet.Cells[row, 37].Value = data.TELEFONO3;
                        worksheet.Cells[row, 38].Value = data.TELEFONO4;
                        worksheet.Cells[row, 39].Value = data.TELEFONO5;

                        row++;
                    }

                    // Estiliza las columnas
                    worksheet.Cells.AutoFitColumns();

                    // Devuelve el csvFile
                    var excelBytes = package.GetAsByteArray();
                    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ClientesAsignados.xlsx");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar : {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult SubircsvFile(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Message"] = "Por favor, seleccione un csvFile valido. ";
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
                        var registroExistente = _context.CORREGIDO_FEED.FirstOrDefault(cf => cf.Dni == registro.DNI);
                        if (registroExistente != null)
                        {
                            TempData["Message"] = $"El registro con DNI {registroExistente.Dni} ya existe. La subida de archivos se canceló.";
                            return RedirectToAction("VistaMainSupervisor", "Supervisor");
                        }

                        // Si todas las validaciones pasan, agregar el registro a la lista
                        registros.Add(registro);
                    }

                    // Procesar y guardar los registros válidos
                    foreach (var registro in registros)
                    {
                        var modelf = new CorregidoFeed
                        {
                            Canal = registro.COD_CANAL,
                            Canal1 = registro.CANAL,
                            Dni = registro.DNI,
                            FechaEnvio = registro.FECHA_ENVIO,
                            FechaGestion = registro.FECHA_GESTION,
                            HoraGestion = registro.HORA_GESTION,
                            Telefono1 = registro.TELEFONO,
                            OrigenTelefono = registro.ORIGEN_TELEFONO,
                            Campaña = registro.COD_CAMPAÑA,
                            CodTipo = registro.COD_TIP,
                            Oferta = registro.OFERTA,
                            DniAsesor = registro.DNI_ASESOR
                        };

                        // Guardar el modelo en la base de datos
                        _context.CORREGIDO_FEED.Add(modelf);
                    }

                    // Guardar los cambios en la base de datos
                    _context.SaveChanges();

                    TempData["Message"] = "csvFile procesado correctamente.";
                }

                return RedirectToAction("VistaMainSupervisor", "Supervisor");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al procesar el csvFile: {ex.Message}");
                TempData["Message"] = "Ocurrió un error al procesar el csvFile. Verifique su formato y vuelva a intentarlo.";
                return RedirectToAction("VistaMainSupervisor", "Supervisor");
            }
        }
    }
}