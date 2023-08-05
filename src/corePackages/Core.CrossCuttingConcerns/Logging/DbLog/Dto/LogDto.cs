using System.Text.Json.Serialization;

namespace Core.CrossCuttingConcerns.Logging.DbLog.Dto
{
    public class LogDto
    {
        public Guid Id { get; set; }
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

        [JsonIgnore]
        public string GetLog => $"Http Request Log Information:{Environment.NewLine}" +
                                   $"{Environment.NewLine}EventId: {EventId} " +
                                   $"{Environment.NewLine}LogDomain: {LogDomain} " +
                                   $"{Environment.NewLine}UserId: {UserId} " +
                                   $"{Environment.NewLine}LogDate: {LogDate} " +
                                   $"{Environment.NewLine}Schema: {Scheme} " +
                                   $"{Environment.NewLine}Host: {Host} " +
                                   $"{Environment.NewLine}Path: {Path} " +
                                   $"{Environment.NewLine}QueryString: {QueryString} " +
                                   $"{Environment.NewLine}ResponseHeaders: {ResponseHeaders}" +
                                   $"{Environment.NewLine}RequestMethod: {RequestMethod}" +
                                   $"{Environment.NewLine}UserAgent: {UserAgent}" +
                                   $"{Environment.NewLine}Remote Ip: {RemoteIp}" +
                                   $"{Environment.NewLine}Headers: {Headers}" +
                                   $"{Environment.NewLine}Request Body: {RequestBody}" +
                                   $"{Environment.NewLine}Response Body: {ResponseBody}" +
                                   $"{Environment.NewLine}StatusCode: {StatusCode}" +
                                   $"{Environment.NewLine}ResponseTime: {ResponseTime}";

        [JsonIgnore]
        public string GetErrorLog => $"Http Request ErrorLog Information:{Environment.NewLine}" +
                                   $"{Environment.NewLine}Schema: {Scheme} " +
                                   $"{Environment.NewLine}Host: {Host} " +
                                   $"{Environment.NewLine}Path: {Path} " +
                                   $"{Environment.NewLine}QueryString: {QueryString} " +
                                   $"{Environment.NewLine}UserId: {UserId} " +
                                   $"{Environment.NewLine}LogDate: {LogDate} " +
                                   $"{Environment.NewLine}Remote Ip: {RemoteIp} " +
                                   $"{Environment.NewLine}Headers: {Headers} " +
                                   $"{Environment.NewLine}Request Body: {RequestBody} " +
                                   $"{Environment.NewLine}UserAgent: {UserAgent}" +
                                   $"{Environment.NewLine}Error : {Exception} " +
                                   $"{Environment.NewLine}InnerError:{InnerException} " +
                                   $"{Environment.NewLine}ErrorMessage : {ExceptionMessage} " +
                                   $"{Environment.NewLine}InnerExceptionMessage : {InnerExceptionMessage} " +
                                   $"{Environment.NewLine}StatusCode: {StatusCode}" +
                                   $"{Environment.NewLine}ResponseTime: {ResponseTime}";

    }
}