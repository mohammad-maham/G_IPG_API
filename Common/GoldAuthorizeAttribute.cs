
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace G_IPG_API.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

public class GoldAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        StringValues user = context.HttpContext.Request.Headers[HeaderNames.Authorization];

        AuthenticationHeaderValue.TryParse(user, out AuthenticationHeaderValue? headerValue);

        if (headerValue == null || headerValue.Parameter == null)
        {
            context.Result = new JsonResult(new { message = "Unauthorized!" })
            { StatusCode = StatusCodes.Status401Unauthorized };
        }
       
    }
}
