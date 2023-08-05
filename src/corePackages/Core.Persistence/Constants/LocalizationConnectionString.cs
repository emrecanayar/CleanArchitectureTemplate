using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Core.Persistence.Constants
{
    public static class LocalizationConnectionString
    {
        public static IConfiguration AppSetting { get; }
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext;
        private static IWebHostEnvironment _env => (IWebHostEnvironment)_httpContext.RequestServices.GetService(typeof(IWebHostEnvironment));

        static LocalizationConnectionString()
        {
            bool result = _env.IsProduction();

            if (result)
                AppSetting = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            else
                AppSetting = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();

        }

        public static string Execute()
        {
            string connectionString = AppSetting.GetConnectionString("RunflowConnectionString");
            return connectionString;

        }

    }
}