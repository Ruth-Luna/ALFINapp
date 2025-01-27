using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ALFINapp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ALFINapp.Controllers
{
    public class ReferidoController : Controller
    {
        public DBServicesGeneral _dbServicesGeneral;
        public DBServicesReferido _dbServicesReferido;
        public ReferidoController( DBServicesGeneral dbServicesGeneral, DBServicesReferido dbServicesReferido)
        {
            _dbServicesGeneral = dbServicesGeneral;
            _dbServicesReferido = dbServicesReferido;
        }
        public async Task<IActionResult> Referido()
        {
            return View("Referido");
        }

        public async Task<IActionResult> BuscarDNIReferido(string dniBusqueda)
        {
            var getDNIReferido = await _dbServicesReferido.GetDataFromDNI(dniBusqueda);

            if (getDNIReferido.IsSuccess == false)
            {
                return Json(new { success = false, message = getDNIReferido.Message});
            }

            return PartialView("DetalleDNIReferido", getDNIReferido.data);
        }

        public async Task<IActionResult> ReferirCliente(string dniReferir,
                                                                        string fuenteBase,
                                                                        string nombres,
                                                                        string apellidos,
                                                                        string dniUsuario)
        {
            var getReferido = await _dbServicesReferido.GuardarClienteReferido(dniReferir, fuenteBase, nombres, apellidos, dniUsuario);
            if (getReferido.IsSuccess == false)
            {
                return Json(new { success = false, message = getReferido.Message});
            }
            return Json(new { success = true, message = getReferido.Message});
        }
    }
}