using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Pipelines.Retry
{
    public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;
        private readonly int _retryCount;

        public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger, int retryCount)
        {
            _logger = logger;
            _retryCount = retryCount;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            for (int i = 0; i < _retryCount; i++)
            {
                try
                {
                    return await next();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Retry Pipeline: Retry attempt {i + 1} failed.");
                }
            }

            throw new Exception($"Retry Pipeline: Maximum retry attempts of {_retryCount} exceeded.");
        }
    }

}
