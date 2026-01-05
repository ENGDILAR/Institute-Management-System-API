using System.Security.Claims;

namespace Lpgin2.Middleweres
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _next;
        public readonly string _requiredRole;

        public RoleMiddleware(RequestDelegate Next , string RequiredRole)
        {
            _next = Next;
            _requiredRole = RequiredRole;
        }

        public async Task Invoke(HttpContext context)
        {
            var user = context.User;

            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if(role!=_requiredRole)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: You don't have access to this resource");
                return;
            }
            await _next(context);

        }
    }
}
