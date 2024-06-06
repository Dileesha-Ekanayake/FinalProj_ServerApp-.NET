using Microsoft.AspNetCore.Authentication;

namespace ServerApp.Security
{
    public class JwtAuthorizationFilter
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAuthorizationFilter> _logger;

        public JwtAuthorizationFilter(RequestDelegate next, ILogger<JwtAuthorizationFilter> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation($"Incoming Request: {context.Request.Method} {context.Request.Path}");

            var authenticationService = context.RequestServices.GetRequiredService<IAuthenticationService>();
            var jwtTokenUtil = context.RequestServices.GetRequiredService<JwtTokenUtil>();
            var userService = context.RequestServices.GetRequiredService<UserService>();

            string authorizationHeader = context.Request.Headers["Authorization"];
            Console.Write(authorizationHeader);

            if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            string token = authorizationHeader.Replace("Bearer ", "");
            context.Items["Authorities"] = JwtTokenUtil.GetAuthoritiesFromToken(token);

            await _next(context);
        }
    }
}
