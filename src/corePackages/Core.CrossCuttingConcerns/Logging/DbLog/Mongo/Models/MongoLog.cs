using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.CrossCuttingConcerns.Logging.DbLog.Mongo.Models
{
    public class MongoLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string EventId { get; set; } = string.Empty;

        public string LogDomain { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.DateTime)]
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

        public string CreatedBy { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string ModifiedBy { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;

        public string GetLog { get; set; } = string.Empty;

        public string GetErrorLog { get; set; } = string.Empty;
    }
}
