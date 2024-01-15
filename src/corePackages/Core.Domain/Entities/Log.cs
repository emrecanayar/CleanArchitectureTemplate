using Core.Domain.Entities.Base;

namespace Core.Domain.Entities
{
    public class Log : Entity<Guid>
    {

        public string EventId { get; set; } = string.Empty;
        public string LogDomain { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime LogDate { get; set; }
        public string Host { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Scheme { get; set; } = string.Empty;
        public string QueryString { get; set; } = string.Empty;
        public string RemoteIp { get; set; } = string.Empty;
        public string Headers { get; set; } = string.Empty;
        public string ResponseHeaders { get; set; } = string.Empty;
        public string RequestMethod { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string RequestBody { get; set; } = string.Empty;
        public string ResponseBody { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
        public string ExceptionMessage { get; set; } = string.Empty;
        public string InnerException { get; set; } = string.Empty;
        public string InnerExceptionMessage { get; set; } = string.Empty;
        public int? StatusCode { get; set; }
        public long? ResponseTime { get; set; }
        public string GetLog { get; set; } = string.Empty;
        public string GetErrorLog { get; set; } = string.Empty;


        public Log()
        {

        }

        public Log(Guid id, string eventId, string logDomain, string userId, DateTime logDate, string host, string path, string scheme, string queryString, string remoteIp, string headers, string responseHeaders, string requestMethod, string userAgent, string requestBody, string responseBody, string exception, string exceptionMessage, string innerException, string innerExceptionMessage, int? statusCode, long? responseTime, string getLog, string getErrorLog) : this()
        {
            Id = id;
            EventId = eventId;
            LogDomain = logDomain;
            UserId = userId;
            LogDate = logDate;
            Host = host;
            Path = path;
            Scheme = scheme;
            QueryString = queryString;
            RemoteIp = remoteIp;
            Headers = headers;
            ResponseHeaders = responseHeaders;
            RequestMethod = requestMethod;
            UserAgent = userAgent;
            RequestBody = requestBody;
            ResponseBody = responseBody;
            Exception = exception;
            ExceptionMessage = exceptionMessage;
            InnerException = innerException;
            InnerExceptionMessage = innerExceptionMessage;
            StatusCode = statusCode;
            ResponseTime = responseTime;
            GetLog = getLog;
            GetErrorLog = getErrorLog;
        }
    }
}