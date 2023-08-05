using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Core.Application.Pipelines.Security
{
    public class DecryptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDecryptService _decryptService;

        public DecryptionBehavior(IHttpContextAccessor httpContextAccessor, IDecryptService decryptService)
        {
            _httpContextAccessor = httpContextAccessor;
            _decryptService = decryptService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                context.Request.EnableBuffering();

                var requestStream = context.Request.Body;
                string requestBody;

                using (var reader = new StreamReader(requestStream))
                {
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                if (!string.IsNullOrEmpty(requestBody))
                {
                    JObject json = JObject.Parse(requestBody);
                    if (json["value"] != null)
                    {
                        string encryptedValue = json["value"].ToString();
                        string decryptedValue = _decryptService.Decrypt(encryptedValue);
                        json["value"] = decryptedValue;

                        using (var newRequestStream = new MemoryStream())
                        using (var writer = new StreamWriter(newRequestStream))
                        {
                            await writer.WriteAsync(json.ToString());
                            await writer.FlushAsync();
                            newRequestStream.Position = 0;
                            context.Request.Body = newRequestStream;

                            return await next();
                        }
                    }
                }
            }

            return await next();
        }
    }
}
