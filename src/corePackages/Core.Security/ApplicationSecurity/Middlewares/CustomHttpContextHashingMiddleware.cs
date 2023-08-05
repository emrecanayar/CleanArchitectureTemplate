using Core.Helpers.Helpers;
using Core.Security.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using UAParser;

namespace Core.Security.ApplicationSecurity.Middlewares
{
    public class CustomHttpContextHashingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private Stopwatch _stopwatch;
        private readonly string securitykey = "zKbVE-yEO'Gg9n7)e[8vJOf=dsUf&eP}";

        public CustomHttpContextHashingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<CustomHttpContextHashingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/File")
            {
                await _next.Invoke(context);
                return;
            }

            if (IsSendedFromWeb(context))
            {
                await _next.Invoke(context);
                return;
            }

            if (IsSendedFromMobileBrowser(context))
            {
                await _next.Invoke(context);
                return;
            }

            if (!String.IsNullOrEmpty(context.Request.QueryString.Value)) await QueryStringDecrypt(context);
            else await JsonBodyDecrypt(context);
        }

        public bool IsSendedFromWeb(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"];
            if (userAgent.Count > 0)
            {
                var uaParser = Parser.GetDefault();
                ClientInfo client = uaParser.Parse(userAgent);
                return client.UA.Family == "Chrome" || client.UA.Family == "Edge" || client.UA.Family == "Firefox" || client.UA.Family == "Safari" || client.UA.Family == "Opera" || client.UA.Family == "Brave" || client.String == "PostmanRuntime/7.29.2";
            }
            return false;
        }

        public bool IsSendedFromMobileBrowser(HttpContext context)
        {
            string path = context.Request.Path.ToString();
            bool result = path.StartsWith("/web");
            return result;
        }

        private async Task QueryStringDecrypt(HttpContext context)
        {
            string httpType = context.Request.Method;

            if (httpType != "GET")
            {
                var encryptQueries = context.Request.QueryString.Value.Split("?enc=");
                string encryptQuery = encryptQueries[1];
                string query = HashingHelper.AESDecrypt(encryptQuery, securitykey);
                var queryKeyValues = query.Split("=");

                List<KeyValuePair<string, string>> queryparameters = new List<KeyValuePair<string, string>>();
                KeyValuePair<string, string> newqueryparameter = new KeyValuePair<string, string>(queryKeyValues[0], queryKeyValues[1]);
                queryparameters.Add(newqueryparameter);
                var contentType = context.Request.ContentType;
                var queryString = new QueryBuilder(queryparameters);
                context.Request.QueryString = queryString.ToQueryString();
            }

            await _next(context);
        }

        private async Task JsonBodyDecrypt(HttpContext context)
        {
            context.Request.EnableBuffering();
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            var api = new ApiRequestInputViewModel
            {
                HttpType = context.Request.Method,
                Query = context.Request.QueryString.Value,
                RequestUrl = context.Request.Path,
                RequestName = "",
                RequestIP = context.Request.Host.Value
            };

            var request = context.Request.Body;
            var response = context.Response.Body;

            if (api.RequestUrl == "/File")
            {
                await _next.Invoke(context);
                return;
            }

            try
            {
                using (var newRequest = new MemoryStream())
                {
                    context.Request.Body = newRequest;
                    using (var reader = new StreamReader(request))
                    {
                        api.Body = await reader.ReadToEndAsync();
                        if (String.IsNullOrEmpty(api.Body) != true)
                        {
                            JObject json = JObject.Parse(api.Body);
                            if (json["value"] != null)
                                api.Body = json["value"].ToString();
                            else _logger.LogInformation($" Information: There is no value on the sent request.");
                        }

                        if (string.IsNullOrEmpty(api.Body))
                        {
                            await _next.Invoke(context);
                            return;
                        }

                        api.Body = HashingHelper.AESDecrypt(api.Body, securitykey);
                    }
                    using (var writer = new StreamWriter(newRequest))
                    {
                        await writer.WriteAsync(api.Body);
                        await writer.FlushAsync();
                        newRequest.Position = 0;
                        context.Request.Body = newRequest;

                        await _next(context);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($" Error: " + ex.ToString());
                await _next(context);
            }
            finally
            {
                context.Request.Body = request;
                context.Response.Body = response;
            }

            context.Response.OnCompleted(() =>
            {
                _stopwatch.Stop();
                api.ElapsedTime = _stopwatch.ElapsedMilliseconds;

                _logger.LogDebug($"RequestLog:{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + (new Random()).Next(0, 10000)}-{api.ElapsedTime}ms", $"{JsonConvert.SerializeObject(api)}");
                return Task.CompletedTask;
            });

            _logger.LogInformation($"Finished handling request.{_stopwatch.ElapsedMilliseconds}ms");
        }
    }
}