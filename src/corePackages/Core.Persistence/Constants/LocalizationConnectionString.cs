using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Core.Persistence.Constants
{
    public static class LocalizationConnectionString
    {
        private static readonly HttpContextAccessor HttpContextAccessor = new();
        private static HttpContext? _httpContext => HttpContextAccessor.HttpContext;
        private static IWebHostEnvironment? _env => _httpContext?.RequestServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;

        public static IConfiguration AppSetting { get; } = InitializeAppSetting();

        private static IConfiguration InitializeAppSetting()
        {
            if (_httpContext == null || _env == null)
            {
                throw new InvalidOperationException("HttpContext or IWebHostEnvironment is not available.");
            }

            bool result = _env.IsProduction();

            if (result)
            {
                return new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            }
            else
            {
                return new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            }
        }

        public static string? Execute()
        {
            string? connectionString = AppSetting.GetConnectionString("RunflowConnectionString");
            return connectionString;

        }

    }
}