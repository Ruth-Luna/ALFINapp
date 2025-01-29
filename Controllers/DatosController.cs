using System.Security;
using System.Text.RegularExpressions;
using ALFINapp.Models;
using ALFINapp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ALFINapp.Controllers
{
    [RequireSession]
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
                    DEPARTAMENTO = formData.DEPARTAMENTO ?? string.Empty,
                    PROVINCIA = formData.PROVINCIA ?? string.Empty,
                    DISTRITO = formData.DISTRITO ?? string.Empty,
                    X_NOMBRE = formData.X_NOMBRE ?? string.Empty,
                    X_APPATERNO = formData.X_APPATERNO ?? string.Empty,
                    X_APMATERNO = formData.X_APMATERNO ?? string.Empty,

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
                if (base_clientes == null)
                {
                    return Json(new { success = false, message = "El id de Base Clientes fue modificado durante el llamado a la Funcion." });
                }
                
                var base_clientes_banco = _context.base_clientes_banco.FirstOrDefault(bcb => bcb.IdBaseBanco == base_clientes.IdBaseBanco);
                if (base_clientes_banco == null)
                {
                    return Json(new { success = false, message = "El id de Base Clientes Banco fue modificado durante el llamado a la Funcion." });
                }

                base_clientes.XNombre = formData.X_NOMBRE;
                base_clientes.XAppaterno = formData.X_APPATERNO;
                base_clientes.XApmaterno = formData.X_APMATERNO;
                base_clientes.Edad = formData.EDAD;
                base_clientes.Departamento = formData.DEPARTAMENTO;
                base_clientes.Provincia = formData.PROVINCIA;
                base_clientes.Distrito = formData.DISTRITO;
                //base_clientes_banco.IdCampanaGrupoBanco = formData.CAMPAÑA;
                
                _context.SaveChanges();

                return Json(new { success = true, message = "Los campos recientemente llenados han sido agregados con éxito a la base de datos." });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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