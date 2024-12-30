using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
    public class DatosController : Controller
    {
        private readonly MDbContext _context;

        public DatosController(MDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult ActualizarDatosProspecto(ActualizarDatosProspectoDTO formData, int idBase)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["Message"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            try
            {

                var prospectoData = new
                {
                    //BaseClientes
                    PROVINCIA = formData.PROVINCIA ?? string.Empty,
                    DISTRITO = formData.DISTRITO ?? string.Empty,

                    // DetalleBase
                    CAMPAÑA = formData.CAMPAÑA ?? string.Empty,
                    OFERTAMAX = formData.OFERTA_MAX ?? string.Empty,
                    TASAMIN = formData.TASA_MINIMA ?? string.Empty,
                    SUCURSALCOMERCIAL = formData.SUCURSAL_COMERCIAL ?? string.Empty,
                    PLAZO = formData.PLAZO ?? string.Empty,
                    CUOTA = formData.CUOTA ?? string.Empty,
                    GRUPOTASA = formData.GRUPO_TASA ?? string.Empty,
                    GRUPOMONTO = formData.GRUPO_MONTO ?? string.Empty,
                    OFERTA12M = formData.OFERTA12M ?? string.Empty,
                    OFERTA18M = formData.OFERTA18M ?? string.Empty,
                    OFERTA24M = formData.OFERTA24M ?? string.Empty,
                    OFERTA36M = formData.OFERTA36M ?? string.Empty,
                    CUOTA12M = formData.CUOTA12M ?? string.Empty,
                    CUOTA18M = formData.CUOTA18M ?? string.Empty,
                    CUOTA24M = formData.CUOTA24M ?? string.Empty,
                    CUOTA36M = formData.CUOTA36M ?? string.Empty,
                    TASA12M = formData.TASA12M ?? string.Empty,
                    TASA18M = formData.TASA18M ?? string.Empty,
                    TASA24M = formData.TASA24M ?? string.Empty,
                    TASA36M = formData.TASA36M ?? string.Empty,
                    PROPENSION = formData.PROPENSION ?? string.Empty,
                    TIPOCLIENTE = formData.TIPO_CLIENTE ?? string.Empty,
                };

                //SECCION DE COMPROBACION DE DATOS
                foreach (var property in prospectoData.GetType().GetProperties())
                {
                    if (property.GetValue(prospectoData) is not string)
                    {
                        return Json(new { success = false, message = $"El campo {property.Name} no es valido." });
                    }
                }

                // Validar que los campos de oferta, cuota y tasa sean números y estén llenos
                var camposNumericos = new List<string> { "OFERTAMAX", "TASAMIN", "OFERTA12M", "OFERTA18M", "OFERTA24M", "OFERTA36M", "CUOTA12M", "CUOTA18M", "CUOTA24M", "CUOTA36M", "TASA12M", "TASA18M", "TASA24M", "TASA36M" };
                var datosDeOfertas = new Dictionary<string, decimal>();

                foreach (var campo in camposNumericos)
                {
                    var property = prospectoData.GetType().GetProperty(campo);
                    if (property != null && property.GetValue(prospectoData) is string value && !string.IsNullOrEmpty(value))
                    {
                        if (decimal.TryParse(value, out decimal valor))
                        {
                            datosDeOfertas[campo] = valor;
                        }
                        else
                        {
                            return Json(new { success = false, message = $"El campo {campo} debe ser un número válido." });
                        }
                    }
                }
                var datosEnteros = new Dictionary<string, int>();

                // Validar que el campo PLAZO no esté vacío y contenga solo números
                if (!string.IsNullOrEmpty(prospectoData.PLAZO))
                {
                    if (prospectoData.PLAZO.All(char.IsDigit))
                    {
                        int PlazoVar = int.Parse(prospectoData.PLAZO);
                        datosEnteros["PLAZO"] = PlazoVar;
                    }
                    else
                    {
                        return Json(new { success = false, message = "El campo PLAZO debe contener solo números." });
                    }
                }

                // Validar que el campo PROPENSION no esté vacío y contenga solo números
                if (!string.IsNullOrEmpty(prospectoData.PROPENSION))
                {
                    if (prospectoData.PROPENSION.All(char.IsDigit))
                    {
                        datosEnteros["PROPENSION"] = int.Parse(prospectoData.PROPENSION);
                    }
                    else
                    {
                        return Json(new { success = false, message = "El campo PROPENSION debe contener solo números." });
                    }
                }
                //SECCION DE ACTUALIZACION DE DATOS Y CONSULTA DE DATOS

                var base_clientes = _context.base_clientes.FirstOrDefault(bc => bc.IdBase == idBase);
                var detalle_base = _context.detalle_base.FirstOrDefault(db => db.IdBase == idBase);
                if (base_clientes == null || detalle_base == null || base_clientes.IdBase != detalle_base.IdBase)
                {
                    return Json(new { success = false, message = "El id de Base Clientes fue modificado durante el llamado a la Funcion." });
                }

                base_clientes.Provincia = formData.PROVINCIA;
                base_clientes.Distrito = formData.DISTRITO;
                detalle_base.Campaña = formData.CAMPAÑA;
                detalle_base.OfertaMax = datosDeOfertas.ContainsKey("OFERTAMAX") ? datosDeOfertas["OFERTAMAX"] : 0;
                detalle_base.TasaMinima = datosDeOfertas.ContainsKey("TASAMIN") ? datosDeOfertas["TASAMIN"] : 0;
                detalle_base.SucursalComercial = formData.SUCURSAL_COMERCIAL;
                detalle_base.Plazo = datosEnteros.ContainsKey("PLAZO") ? datosEnteros["PLAZO"] : 0;
                detalle_base.Cuota = datosDeOfertas.ContainsKey("CUOTA") ? datosDeOfertas["CUOTA"] : 0;
                detalle_base.GrupoTasa = formData.GRUPO_TASA;
                detalle_base.GrupoMonto = formData.GRUPO_MONTO;

                detalle_base.Oferta12m = datosDeOfertas.ContainsKey("OFERTA12M") ? datosDeOfertas["OFERTA12M"] : 0;
                detalle_base.Oferta18m = datosDeOfertas.ContainsKey("OFERTA18M") ? datosDeOfertas["OFERTA18M"] : 0;
                detalle_base.Oferta24m = datosDeOfertas.ContainsKey("OFERTA24M") ? datosDeOfertas["OFERTA24M"] : 0;
                detalle_base.Oferta36m = datosDeOfertas.ContainsKey("OFERTA36M") ? datosDeOfertas["OFERTA36M"] : 0;
                detalle_base.Cuota12m = datosDeOfertas.ContainsKey("CUOTA12M") ? datosDeOfertas["CUOTA12M"] : 0;
                detalle_base.Cuota18m = datosDeOfertas.ContainsKey("CUOTA18M") ? datosDeOfertas["CUOTA18M"] : 0;
                detalle_base.Cuota24m = datosDeOfertas.ContainsKey("CUOTA24M") ? datosDeOfertas["CUOTA24M"] : 0;
                detalle_base.Cuota36m = datosDeOfertas.ContainsKey("CUOTA36M") ? datosDeOfertas["CUOTA36M"] : 0;

                detalle_base.Tasa12m = datosDeOfertas.ContainsKey("TASA12M") ? datosDeOfertas["TASA12M"] : 0;
                detalle_base.Tasa18m = datosDeOfertas.ContainsKey("TASA18M") ? datosDeOfertas["TASA18M"] : 0;
                detalle_base.Tasa24m = datosDeOfertas.ContainsKey("TASA24M") ? datosDeOfertas["TASA24M"] : 0;
                detalle_base.Tasa36m = datosDeOfertas.ContainsKey("TASA36M") ? datosDeOfertas["TASA36M"] : 0;
                detalle_base.Propension = datosEnteros.ContainsKey("PROPENSION") ? datosEnteros["PROPENSION"] : 0;

                detalle_base.TipoCliente = formData.TIPO_CLIENTE;

                _context.SaveChanges();

                return Json(new { success = true, message = "Los campos recientemente llenados han sido agregados con éxito a la base de datos." });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult VerificarDNI(string dni)
        {
            try
            {
                Console.WriteLine($"DNI recibido: {dni}");
                var clienteExistente = _context.base_clientes.FirstOrDefault(c => c.Dni == dni);
                return Json(new { existe = clienteExistente != null });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar el DNI: {ex.Message}");
                return Json(new { existe = false, error = true, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult EnviarComentario(string Telefono, int IdCliente, string Comentario)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Lógica para agregar o actualizar el comentario en la base de datos
                var registro = _context.telefonos_agregados.FirstOrDefault(ta => ta.Telefono == Telefono && ta.IdCliente == IdCliente);
                if (string.IsNullOrEmpty(Telefono) || string.IsNullOrEmpty(Comentario))
                {
                    return Json(new { success = false, message = "Los Campos enviados estan Vacios." });
                }

                if (registro != null)
                {
                    registro.Comentario = Comentario;
                    _context.SaveChanges();
                }

                else
                {
                    return Json(new { success = false, message = "Número no encontrado." });
                }

                return Json(new { success = true, message = "Comentario guardado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult EnviarComentarioTelefonoDB(string Telefono, int IdCliente, string Comentario, int numeroTelefono)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (string.IsNullOrEmpty(Telefono) || string.IsNullOrEmpty(Comentario))
                {
                    return Json(new { success = false, message = "Datos inválidos." });
                }

                var clienteEnriquecido = _context.clientes_enriquecidos.FirstOrDefault(ce => ce.IdCliente == IdCliente);
                
                if (clienteEnriquecido == null)
                {
                    return Json(new { success = false, message = "Cliente no encontrado." });
                }

                switch (numeroTelefono)
                {
                    case 1:
                        if (clienteEnriquecido.Telefono1 == Telefono)
                        {
                            clienteEnriquecido.ComentarioTelefono1 = Comentario;
                        }
                        break;
                    case 2:
                        if (clienteEnriquecido.Telefono2 == Telefono)
                        {
                            clienteEnriquecido.ComentarioTelefono2 = Comentario;
                        }
                        break;
                    case 3:
                        if (clienteEnriquecido.Telefono3 == Telefono)
                        {
                            clienteEnriquecido.ComentarioTelefono3 = Comentario;
                        }
                        break;
                    case 4:
                        if (clienteEnriquecido.Telefono4 == Telefono)
                        {
                            clienteEnriquecido.ComentarioTelefono4 = Comentario;
                        }
                        break;
                    case 5:
                        if (clienteEnriquecido.Telefono5 == Telefono)
                        {
                            clienteEnriquecido.ComentarioTelefono5 = Comentario;
                        }
                        break;
                    default:
                        return Json(new { success = false, message = "Número de teléfono inválido." });
                }

                _context.SaveChanges();
                return Json(new { success = true, message = "Comentario guardado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult EnviarComentarioGeneral(int idAsignacion, string comentarioGeneral)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
            {
                TempData["MessageError"] = "Ha ocurrido un error en la autenticación";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var modificarComentarioGeneral = _context.clientes_asignados.FirstOrDefault(ca => ca.IdAsignacion == idAsignacion);
                if (modificarComentarioGeneral != null)
                {
                    modificarComentarioGeneral.ComentarioGeneral = comentarioGeneral;
                    _context.SaveChanges();
                }
                else
                {
                    return Json(new { success = false, message = "Número no encontrado." });
                }

                return Json(new { success = true, message = "Comentario guardado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}