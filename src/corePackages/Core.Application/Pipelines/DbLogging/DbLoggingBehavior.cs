using AutoMapper;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.CrossCuttingConcerns.Logging.DbLog.Dto;
using Core.CrossCuttingConcerns.Logging.DbLog.MsSQL;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Core.Application.Pipelines.DbLogging
{
    public class DbLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly CrossCuttingConcerns.Logging.DbLog.Logging _logging;
        private readonly IMapper _mapper;

        public DbLoggingBehavior(IHttpContextAccessor httpContextAccessor, CrossCuttingConcerns.Logging.DbLog.Logging logging, IConfiguration configuration, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _logging = logging;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Request
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string requestBody = await readRequestBody(httpContext.Request);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            Domain.Entities.Log logEntry = createLogEntry(httpContext, requestBody);

            // Response
            try
            {
                var response = await next();
                stopwatch.Stop();
                logEntry.ResponseBody = JsonConvert.SerializeObject(response, settings);
                logEntry.StatusCode = (response as ObjectResult)?.StatusCode;
                logEntry.LogDate = DateTime.Now;
                logEntry.ResponseTime = stopwatch.ElapsedMilliseconds;
                return response;

            }
            catch (Exception ex)
            {
                logEntry.Exception = ex.ToString();
                logEntry.ExceptionMessage = ex.Message;
                logEntry.InnerException = ex.InnerException?.ToString();
                logEntry.InnerExceptionMessage = ex.InnerException?.Message;
                logEntry.StatusCode = httpContext.Response.StatusCode;

                await handleExceptionAsync(ex);
                return default;
            }
            finally
            {
                // Veritabanına log kaydını eklenir.
                await addLogToDatabase(logEntry);
            }


        }
        private async Task<string> readRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            using (var reader = new StreamReader(request.Body, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
                return body;
            }
        }

        private Domain.Entities.Log createLogEntry(HttpContext context, string requestBody)
        {
            string headers = string.Join(";", context.Request.Headers.Select(h => $"{h.Key}:{h.Value}"));
            string responseHeaders = string.Join(";", context.Response.Headers.Select(h => $"{h.Key}:{h.Value}"));
            var logEntry = new Domain.Entities.Log
            {
                LogDate = DateTime.Now,
                EventId = Guid.NewGuid().ToString(),
                UserId = context.User?.Identity?.Name,
                LogDomain = context.Request.Host.Value,
                Host = context.Request.Host.Value,
                Path = context.Request.Path,
                Scheme = context.Request.Scheme,
                QueryString = context.Request.QueryString.ToString(),
                RemoteIp = context.Connection.RemoteIpAddress?.ToString(),
                Headers = headers,
                RequestBody = requestBody,
                RequestMethod = context.Request.Method,
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                ResponseHeaders = responseHeaders,
            };

            return logEntry;
        }

        private async Task addLogToDatabase(Domain.Entities.Log logEntry)
        {
            LogDto data = _mapper.Map<LogDto>(logEntry);
            await _logging.CreateLog(new MsSqlLogService(_configuration, _mapper), data);
        }

        private Task handleExceptionAsync(Exception exception) => exception switch
        {
            BusinessException businessException => throw new BusinessException(businessException.Message),
            AuthorizationException authorizationException => throw new AuthorizationException(authorizationException.Message),
            NotFoundException notFoundException => throw new NotFoundException(notFoundException.Message),
            _ => throw new Exception(exception.Message, exception.InnerException)
        };
    }
}
