using Core.CrossCuttingConcerns.Exceptions.Types;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Pipelines.CheckId
{
    public class CheckIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IRequestWithId
    {
        private readonly ILogger<CheckIdBehavior<TRequest, TResponse>> _logger;

        public CheckIdBehavior(ILogger<CheckIdBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is IRequestWithId requestWithId)
            {
                if (requestWithId.Id == 0)
                {
                    throw new BusinessException("Id param is missing");
                }
                else if (!int.TryParse(requestWithId.Id.ToString(), out _))
                {
                    throw new BusinessException("Id param must be an integer");
                }
            }

            _logger.LogInformation("CheckIdBehavior - Handling {RequestType}", typeof(TRequest).Name);

            TResponse response = await next();

            _logger.LogInformation("CheckIdBehavior - Handled {RequestType}", typeof(TRequest).Name);

            return response;
        }
    }
}
