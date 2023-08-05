using Core.BackgroundJob.Filters;
using Core.BackgroundJob.Logging;
using Core.BackgroundJob.Services;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Core.BackgroundJob.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddHangfireServices(this IServiceCollection services)
        {
            services.AddTransient<IJobService, JobManager>();
            services.AddSingleton<IJobLogger, JobLogger>();
            services.AddSingleton<IRetryPolicyService, RetryPolicyManager>();
            GlobalJobFilters.Filters.Add(new CustomJobFilterAttribute(new JobLogger()));
        }
    }
}
