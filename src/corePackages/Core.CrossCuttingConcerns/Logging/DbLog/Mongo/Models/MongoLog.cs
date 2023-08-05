using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.CrossCuttingConcerns.Logging.DbLog.Mongo.Models
{
    public class MongoLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string EventId { get; set; }

        public string LogDomain { get; set; }

        public string UserId { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
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

        public string CreatedBy { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string ModifiedBy { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? ModifiedDate { get; set; } = DateTime.Now;

        public string GetLog { get; set; }

        public string GetErrorLog { get; set; }
    }
}
