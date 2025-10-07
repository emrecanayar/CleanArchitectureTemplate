using Core.Persistence.Contexts;
using Core.Persistence.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BaseDbContext>(
            options =>
            options.UseSqlServer(configuration.GetConnectionString("ConnectionString"), b => b.MigrationsAssembly("Core.Persistence"))
            .LogTo(msg => EntityFrameworkQueryLog.LogConsole(msg), LogLevel.Information));

            return services;
        }
    }
}
