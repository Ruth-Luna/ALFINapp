using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ALFINapp.Filters
{
    public class RequireSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            if (session.GetInt32("UsuarioId") == null)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                context.HttpContext.Items["MessageError"] = "No ha iniciado sesión, por favor inicie sesión.";
            }
            base.OnActionExecuting(context);
        }
    }
}