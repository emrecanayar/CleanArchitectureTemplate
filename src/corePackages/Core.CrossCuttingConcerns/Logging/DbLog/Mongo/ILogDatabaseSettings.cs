namespace Core.CrossCuttingConcerns.Logging.DbLog.Mongo
{
    public interface ILogDatabaseSettings
    {
        public string LogCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
