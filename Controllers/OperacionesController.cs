using ALFINapp.API.DTOs;
using ALFINapp.API.Models;
using ALFINapp.Datos;
using ALFINapp.Datos.DAO.Operaciones;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Models;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace ALFINapp.Controllers
{
    public class OperacionesController : Controller
    {
        private readonly DAO_Operaciones _dao_Operaciones = new DAO_Operaciones();
        private readonly DA_Usuario _dao_Usuario = new DA_Usuario();

        // [Route("Operaciones")]
        public IActionResult Operaciones()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDerivaciones(int? idAsesor = null, int? idSupervisor = null, string? agencia = null, DateTime? fecha_derivacion = null, DateTime? fecha_visita = null, string? dni = null)
        {
            // Recuperar IDS
            (idAsesor, idSupervisor) = ResolverIdsPorRol(idAsesor, idSupervisor);

            (bool success, List<ViewDerivaciones>? data) = await _dao_Operaciones.GetAllDerivaciones(idAsesor, idSupervisor, agencia, fecha_derivacion, fecha_visita, dni);

            return Json(new { success, data });

        }

        [HttpGet]
        public async Task<IActionResult> GetAllReagendamientos(int? idAsesor = null, int? idSupervisor = null, DateTime? fecha_reagendamiento = null, DateTime? fecha_visita = null, string? agencia = null, string? dni = null)
        {
            // Recuperar IDS
            (idAsesor, idSupervisor) = ResolverIdsPorRol(idAsesor, idSupervisor);

            (bool success, List<ViewReagendamientos>? data) = await _dao_Operaciones.GetAllReagendamientos(idAsesor, idSupervisor, fecha_reagendamiento, fecha_visita, agencia, dni);

            return Json(new { success, data });
        }

        [HttpGet]
        public IActionResult ObtenerAsesoresRelacionados()
        {
            // Recuperar el ID USUSARIO
            int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            int? idRol = HttpContext.Session.GetInt32("RolUser");
            if (idRol == null || idUsuario == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }

            int? idSupervisor = null;
            //Logica segun el rol
            if (idRol == 1 || idRol == 4) // ADMINISTRADOR Y GERENTE ZONAL
            {
                // SIN FILTROS SOBREESCRITOS
            }
            else if (idRol == 2) // SUPERVISOR
            {
                idSupervisor = idUsuario;
            }
            else // ASESOR
            {
                return Json(new { success = false, message = "No tiene permisos para ver esta información." });
            }
            var asesores = _dao_Usuario.ListarAsesores(idSupervisor);
            return Json(new { success = true, data = asesores });
        }

        [HttpGet]
        public IActionResult ObtenerSupervisoresRelacionados()
        {
            // Recuperar el ID USUSARIO
            int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            int? idRol = HttpContext.Session.GetInt32("RolUser");
            if (idRol == null || idUsuario == null)
            {
                return Json(new { success = false, message = "No se ha iniciado sesión." });
            }

            if (idRol == 1 || idRol == 4) // ADMINISTRADOR Y GERENTE ZONAL
            {
                var supervisores = _dao_Usuario.ListarSupervisores();
                return Json(new { success = true, data = supervisores });
            }
            else
            {
                return Json(new { success = false, message = "No tiene permisos para ver esta información." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarAgencias()
        {

            (bool success, List<ViewAgencias>? data) = await _dao_Operaciones.GetAgencias();
            return Json(new { success, data });
        }

        [HttpPost]
        public async Task<IActionResult> Reagendar(
            [FromBody] DtoVReagendar dtovreagendar)
        {
            /*             if (dtovreagendar.FechaReagendamiento == null || dtovreagendar.FechaReagendamiento == DateTime.MinValue)
                        {
                            return Json(new { success = false, message = "La fecha de reagendamiento es obligatoria." });
                        }
                        if (dtovreagendar.IdDerivacion == null || dtovreagendar.IdDerivacion == 0)
                        {
                            return Json(new { success = false, message = "El id de derivación es obligatorio." });
                        }
                        var exec = await _useCaseReagendar.exec(
                            dtovreagendar.IdDerivacion.Value,
                            dtovreagendar.FechaReagendamiento.Value,
                            dtovreagendar.urlEvidencias);
                        if (!exec.IsSuccess)
                        {
                            return Json(new { success = false, message = exec.Message });
                        }
                        return Json(new { success = true, message = exec.Message }); */
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> GetHistoricoReagendamientos([FromBody] List<int> idsDerivacion)
        {
            if (idsDerivacion == null || idsDerivacion.Count == 0)
            {
                return Json(new { success = false, message = "El id de derivación es obligatorio." });
            }

            (bool success, List<ViewReagendamientos>? data) = await _dao_Operaciones.GetHistosricoReagendamientos(idsDerivacion);

            return Json(new { success, data });

        }


        [HttpGet]
        public IActionResult ExportarDerivacionesExcel(string? dni, int? idAsesor, int? idSupervisor, string? agencia = null, DateTime? fecha_derivacion = null, DateTime? fecha_visita = null)
        {
            (idAsesor, idSupervisor) = ResolverIdsPorRol(idAsesor, idSupervisor);

            var derivaciones = _dao_Operaciones.Listar_Derivaciones_Excel(dni, idAsesor, idSupervisor, agencia, fecha_derivacion, fecha_visita);

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Derivaciones");

                string[] headers = {
                    "Estado Derivación",
                    "Estado Formulario",
                    "Estado Correo",
                    "DNI Cliente",
                    "Cliente",
                    "Telefono Cliente",
                    "Asesor",
                    "Oferta Máxima",
                    "Agencia",
                    "Fecha Derivación",
                    "Fecha Visita",
                    "Estado Evidencia",
                    "Fecha Evidencia",
                };

                ws.Row(1).Height = 22;
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = ws.Cells[1, i + 1];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                int row = 2;
                foreach (var it in derivaciones)
                {
                    dynamic d = it;
                    ws.Cells[row, 1].Value = d.EstadoDerivacion;
                    ws.Cells[row, 2].Value = d.EstadoFormulario;
                    ws.Cells[row, 3].Value = d.EstadoCorreo;
                    ws.Cells[row, 4].Value = d.DniCliente;
                    ws.Cells[row, 5].Value = d.NombreCliente;
                    ws.Cells[row, 6].Value = d.TelefonoCliente;
                    ws.Cells[row, 7].Value = d.NombreAsesor;
                    ws.Cells[row, 8].Value = d.OfertaMax;
                    ws.Cells[row, 9].Value = d.NombreAgencia;

                    if (d?.FechaDerivacion != null)
                    {
                        ws.Cells[row, 10].Value = (DateTime?)d.FechaDerivacion;
                        ws.Cells[row, 10].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                    };

                    if (d?.FechaVisita != null)
                    {
                        ws.Cells[row, 11].Value = (DateTime?)d.FechaVisita;
                        ws.Cells[row, 11].Style.Numberformat.Format = "dd/MM/yyyy";
                    };


                    ws.Cells[row, 12].Value = d.estadoEvidencia;
                    if (d?.estadoEvidencia != null)
                    {
                        ws.Cells[row, 13].Value = (DateTime?)d.FechaEvidencia;
                        ws.Cells[row, 13].Style.Numberformat.Format = "dd/MM/yyyy";
                    };

                    row++;
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.View.FreezePanes(2, 1);

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var nombreArchivo = $"Usuarios_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    nombreArchivo);
            }
        }


        [HttpGet]
        public async Task<IActionResult> ExportarReagendamientosExcel(int? idAsesor = null, int? idSupervisor = null, DateTime? fecha_reagendamiento = null, DateTime? fecha_visita = null, string? agencia = null, string? dni = null)
        {
            (idAsesor, idSupervisor) = ResolverIdsPorRol(idAsesor, idSupervisor);

            (bool success, List<ViewReagendamientos>? data) = await _dao_Operaciones.GetAllReagendamientos(idAsesor, idSupervisor, fecha_reagendamiento, fecha_visita, agencia, dni);

            if (!success || data == null || data.Count == 0)
            {
                data = new List<ViewReagendamientos>();
            }

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Reagendamientos");

                string[] headers = {
                    "Puede ser reagendado",
                    "Estado reagendamiento",
                    "Correo enviado",
                    "N° reagendamiento",
                    //"Fue desembolsado ultimo mes",
                    "DNI cliente",
                    "Cliente",
                    "Telefono",
                    "Asesor",
                    "Oferta",
                    "Agencia",
                    "Fecha derivacion",
                    "Fecha visita",
                    "Fecha reagendamiento"
                };

                ws.Row(1).Height = 22;
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = ws.Cells[1, i + 1];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                int row = 2;
                foreach (var reagendamiento in data)
                {
                    ws.Cells[row, 1].Value = reagendamiento.PuedeSerReagendado ? "SI" : "NO";
                    ws.Cells[row, 2].Value = reagendamiento.EstadoReagendamiento;
                    ws.Cells[row, 3].Value = reagendamiento.FueEnviadoEmail ? "SI" : "NO";
                    ws.Cells[row, 4].Value = reagendamiento.NumeroReagendamientoFormateado;
                    //ws.Cells[row, 5].Value = reagendamiento.FueDesembolsadoGeneral ? "SI" : "NO";
                    ws.Cells[row, 5].Value = reagendamiento.DniCliente;
                    ws.Cells[row, 6].Value = reagendamiento.NombreCliente;
                    ws.Cells[row, 7].Value = reagendamiento.Telefono;
                    ws.Cells[row, 8].Value = reagendamiento.NombreAsesor;
                    ws.Cells[row, 9].Value = reagendamiento.Oferta;
                    ws.Cells[row, 10].Value = reagendamiento.Agencia;
                    ws.Cells[row, 11].Value = reagendamiento.FechaDerivacion.HasValue ? reagendamiento.FechaDerivacion : "";
                    ws.Cells[row, 11].Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    ws.Cells[row, 12].Value = reagendamiento.FechaVisita.HasValue ? reagendamiento.FechaVisita : "";
                    ws.Cells[row, 12].Style.Numberformat.Format = "dd/MM/yyyy";
                    ws.Cells[row, 13].Value = reagendamiento.FechaAgendamiento.HasValue ? reagendamiento.FechaAgendamiento : "";
                    ws.Cells[row, 13].Style.Numberformat.Format = "dd/MM/yyyy hh:mm";

                    row++;
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.View.FreezePanes(2, 1);

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var nombreArchivo = $"Usuarios_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    nombreArchivo);
            }

        }

        [HttpPost]
        public async Task<IActionResult> ExportarHistoricosExcel([FromBody] List<int> idsDerivacion)
        {

            (bool success, List<ViewReagendamientos>? historicos) = await _dao_Operaciones.GetHistosricoReagendamientos(idsDerivacion);

            if (!success || historicos == null || historicos.Count == 0)
            {
                historicos = new List<ViewReagendamientos>();
            }

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Historicos");

                string[] headers = {
                    "N° reagendamiento",
                    "Cliente",
                    "Telefono",
                    "Asesor",
                    "Oferta",
                    "Agencia",
                    "Fecha derivacion",
                    "Fecha visita",
                    "Fecha reagendamiento"
                };

                ws.Row(1).Height = 22;
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = ws.Cells[1, i + 1];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                int row = 2;
                foreach (var hstrc in historicos)
                {
                    ws.Cells[row, 1].Value = hstrc.NumeroReagendamientoFormateado;
                    ws.Cells[row, 2].Value = $"{hstrc.DniCliente ?? ""} - {hstrc.NombreCliente ?? ""}";
                    ws.Cells[row, 3].Value = hstrc.Telefono;
                    ws.Cells[row, 4].Value = $"{hstrc.DniAsesor ?? ""} - {hstrc.NombreAsesor ?? ""}";
                    ws.Cells[row, 5].Value = hstrc.Oferta;
                    ws.Cells[row, 5].Style.Numberformat.Format = "#,##0.00";
                    ws.Cells[row, 6].Value = hstrc.Agencia;
                    ws.Cells[row, 7].Value = hstrc.FechaDerivacion.HasValue ? hstrc.FechaDerivacion : "";
                    ws.Cells[row, 7].Style.Numberformat.Format = "dd/MM/yyyy hh:mm";
                    ws.Cells[row, 8].Value = hstrc.FechaVisita.HasValue ? hstrc.FechaVisita : "";
                    ws.Cells[row, 8].Style.Numberformat.Format = "dd/MM/yyyy";
                    ws.Cells[row, 9].Value = hstrc.FechaAgendamiento.HasValue ? hstrc.FechaAgendamiento : "";
                    ws.Cells[row, 9].Style.Numberformat.Format = "dd/MM/yyyy hh:mm";

                    row++;
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.View.FreezePanes(2, 1);

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var nombreArchivo = $"Usuarios_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    nombreArchivo);
            }

        }

        // FUNCION AXILIAR
        private (int? idAsesor, int? idSupervisor) ResolverIdsPorRol(int? idAsesor, int? idSupervisor)
        {
            // Recuperar IDs de la sesión
            int? idUsuario = HttpContext.Session.GetInt32("UsuarioId");
            int? idRol = HttpContext.Session.GetInt32("RolUser");

            //Logica segun el rol
            if (idRol == 1 || idRol == 4) // ADMINISTRADOR Y GERENTE ZONAL
            {
                // SIN FILTROS SOBREESCRITOS
            }
            else if (idRol == 2) // SUPERVISOR
            {
                idSupervisor = idUsuario;
            }
            else if (idRol == 3) // ASESOR
            {
                idSupervisor = null;
                idAsesor = idUsuario;
            }
            return (idAsesor, idSupervisor);
        }

    }
}