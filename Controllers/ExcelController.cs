using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using OfficeOpenXml;

namespace ALFINapp.Controllers
{
    public class ExcelController : Controller
    {
        private readonly MDbContext _context;
        public ExcelController(MDbContext context)
        {
            _context = context;
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
                    ARCHIVO DE EXCEL
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
                // Genera el archivo Excel
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
                    worksheet.Cells[1, 16].Value = "GRUPO_TASA";
                    worksheet.Cells[1, 17].Value = "GRUPO_MONTO";
                    worksheet.Cells[1, 18].Value = "PROPENSION";
                    worksheet.Cells[1, 19].Value = "TIPO_CLIENTE";
                    worksheet.Cells[1, 20].Value = "CLIENTE_NUEVO";
                    worksheet.Cells[1, 21].Value = "COLOR";
                    worksheet.Cells[1, 22].Value = "COLOR_FINAL";
                    worksheet.Cells[1, 23].Value = "TELEFONO1";
                    worksheet.Cells[1, 24].Value = "TELEFONO2";
                    worksheet.Cells[1, 25].Value = "TELEFONO3";
                    worksheet.Cells[1, 26].Value = "TELEFONO4";
                    worksheet.Cells[1, 27].Value = "TELEFONO5";

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
                        worksheet.Cells[row, 16].Value = data.GRUPO_TASA;
                        worksheet.Cells[row, 17].Value = data.GRUPO_MONTO;
                        worksheet.Cells[row, 18].Value = data.PROPENSION;
                        worksheet.Cells[row, 19].Value = data.TIPO_CLIENTE;
                        worksheet.Cells[row, 20].Value = data.CLIENTE_NUEVO;
                        worksheet.Cells[row, 21].Value = data.COLOR;
                        worksheet.Cells[row, 22].Value = data.COLOR_FINAL;
                        worksheet.Cells[row, 23].Value = data.TELEFONO1;
                        worksheet.Cells[row, 24].Value = data.TELEFONO2;
                        worksheet.Cells[row, 25].Value = data.TELEFONO3;
                        worksheet.Cells[row, 26].Value = data.TELEFONO4;
                        worksheet.Cells[row, 27].Value = data.TELEFONO5;
                        row++;
                    }

                    // Estiliza las columnas
                    worksheet.Cells.AutoFitColumns();

                    // Devuelve el archivo
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
        public IActionResult SubirArchivo(IFormFile archivo)
        {
            try
            {
                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error al verificar : {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }
    }
}