using Hangfire;

namespace Core.BackgroundJob.Services
{
    public class RetryPolicyManager : IRetryPolicyService
    {
        /// <summary>
        /// Hangfire için yeniden deneme politikasını uygular.
        /// </summary>
        public void ApplyPolicy()
        {
            // Hangfire için yeniden deneme politikasını yapılandırın
            // Örnek olarak, bu yapılandırma belirsiz bir sayıda yeniden denemeyi engeller.
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 5 });
        }
    }
}
