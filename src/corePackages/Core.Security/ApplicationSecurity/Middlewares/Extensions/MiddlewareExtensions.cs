using Microsoft.AspNetCore.Builder;

namespace Core.Security.ApplicationSecurity.Middlewares.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseIpSafe(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IpSafeMiddleware>();
        }

        public static IApplicationBuilder UseCustomHttpContextHashingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomHttpContextHashingMiddleware>();
        }
    }
}