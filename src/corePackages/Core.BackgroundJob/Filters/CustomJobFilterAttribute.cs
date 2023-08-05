using Core.BackgroundJob.Logging;
using Hangfire.Common;
using Hangfire.States;

namespace Core.BackgroundJob.Filters
{
    public class CustomJobFilterAttribute : JobFilterAttribute, IElectStateFilter
    {
        private readonly IJobLogger _logger;

        public CustomJobFilterAttribute(IJobLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Durum seçim olayını ele alır ve hata durumunda bir işleme geri bildirim sağlar.
        /// </summary>
        /// <param name="context">Durum seçim bağlamı</param>
        public void OnStateElection(ElectStateContext context)
        {
            var failedState = context.CandidateState as FailedState;
            if (failedState != null)
            {
                _logger.Log($"Job {context.BackgroundJob.Id} failed with exception {failedState.Exception}");
            }
        }
    }

}
