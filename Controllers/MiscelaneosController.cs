using ALFINapp.Datos.DAO.Miscelaneos;
using Microsoft.AspNetCore.Mvc;

namespace ALFINapp.Controllers
{
    public class MiscelaneosController : Controller
    {
        private readonly ILogger<MiscelaneosController> _logger;
        private readonly DAO_ConsultasMiscelaneas _dao_consultasMiscelaneas;

        public MiscelaneosController(
            ILogger<MiscelaneosController> logger,
            DAO_ConsultasMiscelaneas dao_consultasMiscelaneas)
        {
            _logger = logger;
            _dao_consultasMiscelaneas = dao_consultasMiscelaneas;
        }

        public async Task<IActionResult> getAgencias()
        {
            try
            {
                var agencias = await _dao_consultasMiscelaneas.GetAgencias();
                if (agencias.IsSuccess)
                {
                    return Json(new { success = true, data = agencias.Agencias });
                }
                else
                {
                    return Json(new { success = false, message = agencias.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching agencies");
                return Json(new { success = false, message = "Error fetching agencies" });
            }
        }

        public async Task<IActionResult> GetTipificaciones()
        {
            try
            {
                var tipificaciones = await _dao_consultasMiscelaneas.GetTipificaciones();
                if (tipificaciones.IsSuccess)
                {
                    return Json(new { success = true, data = tipificaciones.Data.Select(t => new { t.idtip, t.nombretip }), message = tipificaciones.Message });
                }
                else
                {
                    return Json(new { success = false, message = tipificaciones.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tipificaciones");
                return Json(new { success = false, message = "Error fetching tipificaciones" });
            }
        }
    }
}