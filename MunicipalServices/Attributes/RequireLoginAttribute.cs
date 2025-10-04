using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MunicipalServices.Attributes
{
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                // User is not logged in, redirect to login
                var controller = context.RouteData.Values["controller"]?.ToString();
                var action = context.RouteData.Values["action"]?.ToString();

                // Store the return URL
                var returnUrl = $"/{controller}/{action}";
                if (context.HttpContext.Request.QueryString.HasValue)
                {
                    returnUrl += context.HttpContext.Request.QueryString.Value;
                }

                context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl = returnUrl });
            }
            else
            {
                base.OnActionExecuting(context);
            }
        }
    }
}