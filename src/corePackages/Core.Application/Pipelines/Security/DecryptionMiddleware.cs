using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Core.Application.Pipelines.Security
{
    public class DecryptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDecryptService _decryptService;

        public DecryptionMiddleware(RequestDelegate next, IDecryptService decryptService)
        {
            _next = next;
            _decryptService = decryptService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            string requestBody;
            using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            if (!string.IsNullOrEmpty(requestBody))
            {
                JObject json = JObject.Parse(requestBody);
                if (json["value"] != null)
                {
                    string encryptedValue = json["value"]!.ToString();
                    string decryptedValue = _decryptService.Decrypt(encryptedValue);
                    context.Items["DecryptedRequestBody"] = decryptedValue;
                }
            }

            await _next(context);
        }
    }
}