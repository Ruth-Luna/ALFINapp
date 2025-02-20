using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ALFINapp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ALFINapp.Filters
{
    public class PermissionAuthorizationAttribute : TypeFilterAttribute
    {
        public PermissionAuthorizationAttribute(string controller, string action)
            : base(typeof(PermissionAuthorizationFilter))
        {
            Arguments = new object[] { controller, action };
        }
    }

    public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly DBServicesRoles _dBServicesRoles;
        private readonly string _controller;
        private readonly string _action;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public PermissionAuthorizationFilter(
            DBServicesRoles dBServicesRoles,
            ITempDataDictionaryFactory tempDataFactory, // Inyectamos TempData
            string controller,
            string action)
        {
            _dBServicesRoles = dBServicesRoles;
            _tempDataFactory = tempDataFactory;
            _controller = controller;
            _action = action;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var usuarioId = context.HttpContext.Session.GetInt32("UsuarioId");
            var idRol = context.HttpContext.Session.GetInt32("RolUser");

            if (idRol == null || usuarioId == null)
            {
                GuardarMensajeEnTempData(context.HttpContext, "No se ha iniciado sesión");
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            var tienePermiso = await _dBServicesRoles.tienePermiso(idRol.Value, _controller, _action);
            if (!tienePermiso.Data.HasValue || !tienePermiso.Data.Value)
            {
                var vistaPorDefecto = await _dBServicesRoles.getVistaPorDefecto(idRol.Value);
                if (!vistaPorDefecto.IsSuccess || vistaPorDefecto.Data == null)
                {
                    GuardarMensajeEnTempData(context.HttpContext, vistaPorDefecto.Message);
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                    return;
                }

                GuardarMensajeEnTempData(context.HttpContext, tienePermiso.Message);
                context.Result = new RedirectToActionResult(
                    vistaPorDefecto.Data.ruta_vista,
                    vistaPorDefecto.Data.nombre_vista,
                    null);
            }
        }

        private void GuardarMensajeEnTempData(HttpContext httpContext, string mensaje)
        {
            var tempData = _tempDataFactory.GetTempData(httpContext);
            tempData["MessageError"] = mensaje; // Se almacena el mensaje en TempData
        }
    }
}
