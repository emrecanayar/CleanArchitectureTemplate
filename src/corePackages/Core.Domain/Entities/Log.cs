using Core.Domain.Entities.Base;

namespace Core.Domain.Entities
{
    public class Log : Entity<Guid>
    {

        public string EventId { get; set; }
        public string LogDomain { get; set; }
        public string UserId { get; set; }
        public DateTime LogDate { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string Scheme { get; set; }
        public string QueryString { get; set; }
        public string RemoteIp { get; set; }
        public string Headers { get; set; }
        public string ResponseHeaders { get; set; }
        public string RequestMethod { get; set; }
        public string UserAgent { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string Exception { get; set; }
        public string ExceptionMessage { get; set; }
        public string InnerException { get; set; }
        public string InnerExceptionMessage { get; set; }
        public int? StatusCode { get; set; }
        public long? ResponseTime { get; set; }
        public string GetLog { get; set; }
        public string GetErrorLog { get; set; }


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