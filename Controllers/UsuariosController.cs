using ALFINapp.API.Filters;
using ALFINapp.API.Models;
using ALFINapp.Datos;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Threading.Tasks;

namespace ALFINapp.API.Controllers
{
    [RequireSession]
    public class UsuariosController : Controller
    {
        DA_Usuario _daUsuario = new DA_Usuario();

        public UsuariosController(){}

        [HttpGet]
        [PermissionAuthorization("Usuarios", "Administracion")]
        public IActionResult Administracion()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ListarUsuarioAdministrador(int? idUsuario)
        {
            var rolUser = HttpContext.Session.GetInt32("RolUser");
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            
            int? idSupervisor = null;

            if (rolUser == 2)
            {
                // Si es supervisor, usar su ID como filtro
                idSupervisor = usuarioId;
            }

            var listarUsuario = _daUsuario.ListarUsuarios(idUsuario, idSupervisor);
            return Json(listarUsuario);
        }
        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] ViewUsuario usuario)
        {
            try
            {
                var UsuarioIdJefe = HttpContext.Session.GetInt32("UsuarioId");
                if (UsuarioIdJefe == null)
                {
                    return Json(new { success = false, message = "No se ha podido crear el usuario" });
                }
                var result = await _daUsuario.CrearUsuario(usuario, UsuarioIdJefe.Value);
                if (result.IsSuccess)
                {
                    return Json(new { success = true, message = "Usuario creado correctamente" });
                }
                return Json(new { success = false, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> ActualizarUsuario([FromBody] ViewUsuario usuario)
        {
            try
            {
                (bool success, string message) = await _daUsuario.ActualizarUsuario(usuario);
                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    mensaje = "Ocurrió un error interno al intentar actualizar el usuario.",
                    detalle = ex.Message
                });
            }
        }

        public IActionResult ActualizarEstadoUsuario([FromBody] ViewUsuario usuario)
        {
            try
            {
                string estado = usuario.Estado == "1" ? "ACTIVO" : "INACTIVO";

                bool actualizado = _daUsuario.ActualizarEstado(usuario.IdUsuario, estado);

                if (actualizado)
                    return Ok(new { success = true, mensaje = "Se ha cambiado el estado del Usuario" });
                else
                    return StatusCode(500, new { success = false, mensaje = "No se pudo actualizar el estado del usuario." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    mensaje = "Ocurrió un error interno al intentar actualizar el estado.",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet]
        public JsonResult ListarSupervisores()
        {
            var listarSupervisores = _daUsuario.ListarSupervisores();
            return Json(listarSupervisores);
        }

        [HttpGet]
        public JsonResult ListarRoles()
        {
            var listarRol = _daUsuario.ListarRoles();
            return Json(listarRol);
        }

        [HttpGet]
        public IActionResult ExportarUsuariosExcel(string? dni, string? usuario, int? idRol, string? estado)
        {
            var usuarios = _daUsuario.ExportarUsuariosExcel(dni, usuario, idRol, estado);

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add("Usuarios");

                string[] headers = {
                    "Dni",
                    "Tipo de Documento",
                    "Apellido Paterno",
                    "Apellido Materno",
                    "Nombres",
                    "Rol",
                    "Departamento",
                    "Provincia",
                    "Distrito",
                    "Telefono",
                    "Fecha de Registro",
                    "Fecha de Inicio",
                    "Fecha de Cese",
                    "Estado",
                    "Supervisor",
                    "Región",
                    "Campaña"
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
                foreach (var it in usuarios)
                {
                    dynamic d = it;
                    ws.Cells[row, 1].Value = d.Dni;
                    ws.Cells[row, 2].Value = d.TipoDocumento; 
                    ws.Cells[row, 3].Value = d.ApellidoPaterno;
                    ws.Cells[row, 4].Value = d.ApellidoMaterno;
                    ws.Cells[row, 5].Value = d.Nombres;
                    ws.Cells[row, 6].Value = d.Rol;
                    ws.Cells[row, 7].Value = d.Departamento;
                    ws.Cells[row, 8].Value = d.Provincia;
                    ws.Cells[row, 9].Value = d.Distrito;
                    ws.Cells[row, 10].Value = d.Telefono;

                    if (d?.FechaRegistro != null)
                    {
                        ws.Cells[row, 11].Value = (DateTime?)d.FechaRegistro;
                        ws.Cells[row, 11].Style.Numberformat.Format = "dd/MM/yyyy";
                    }
                    if (d?.FechaInicio != null)
                    {
                        ws.Cells[row, 12].Value = (DateTime?)d.FechaInicio;
                        ws.Cells[row, 12].Style.Numberformat.Format = "dd/MM/yyyy";
                    }
                    if (d?.FechaCese != null)
                    {
                        ws.Cells[row, 13].Value = (DateTime?)d.FechaCese;
                        ws.Cells[row, 13].Style.Numberformat.Format = "dd/MM/yyyy";
                    }
                    ws.Cells[row, 14].Value = d.Estado;
                    ws.Cells[row, 15].Value = d.Supervisor;
                    ws.Cells[row, 16].Value = d.Region;  
                    ws.Cells[row, 17].Value = d.NombreCampania;

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
    }
}