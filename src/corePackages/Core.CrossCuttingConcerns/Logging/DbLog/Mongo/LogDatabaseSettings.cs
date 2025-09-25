namespace Core.CrossCuttingConcerns.Logging.DbLog.Mongo
{
    public class LogDatabaseSettings : ILogDatabaseSettings
    {
        public string LogCollectionName { get; set; } = string.Empty;

        public string ConnectionString { get; set; } = string.Empty;

        public string DatabaseName { get; set; } = string.Empty;
    }
}