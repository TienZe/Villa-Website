using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VillaWeb.Infrastructures;

public class AuthRedirectionExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is UnauthorizedException) {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            context.ExceptionHandled = true;
        }
    }
}