using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Pipelines.Security
{
    public class DecryptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IHttpRequestRelated
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DecryptionBehavior(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;

            if (context!.Items.ContainsKey("DecryptedRequestBody"))
            {
                var modifiedBody = context.Items["DecryptedRequestBody"] as string;

                if (request is IHttpRequestRelated httpRequestRelatedRequest)
                {
                    httpRequestRelatedRequest.HttpRequestBody = modifiedBody;
                }
            }

            return await next();
        }
    }
}