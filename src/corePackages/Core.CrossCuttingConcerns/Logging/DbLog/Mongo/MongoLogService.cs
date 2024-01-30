using AutoMapper;
using Core.CrossCuttingConcerns.Logging.DbLog.Dto;
using Core.CrossCuttingConcerns.Logging.DbLog.Mongo;
using Core.CrossCuttingConcerns.Logging.DbLog.Mongo.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Core.CrossCuttingConcerns.Logging.DbLog
{
    public class MongoLogService : ILogService
    {
        private readonly ILogger<MongoLogService> _logger;
        private readonly IMongoCollection<MongoLog> _logCollection;
        private readonly IMapper _mapper;

        public MongoLogService(ILogger<MongoLogService> logger, ILogDatabaseSettings logDatabaseSettings, IMapper mapper)
        {
            var client = new MongoClient(logDatabaseSettings.ConnectionString);
            var database = client.GetDatabase(logDatabaseSettings.DatabaseName);
            _logCollection = database.GetCollection<MongoLog>(logDatabaseSettings.LogCollectionName);
            _logger = logger;
            _mapper = mapper;
        }

        public async Task CreateLog(LogDto logDto)
        {
            MongoLog log = _mapper.Map<MongoLog>(logDto);
            await LogToDb(log);
        }

        private async Task LogToDb(MongoLog log)
        {
            try
            {
                await _logCollection.InsertOneAsync(log);
            }
            catch (Exception exception)
            {
                _logger.LogError($"DB Logging Exception: {exception.Message} {Environment.NewLine} " +
                    $"Source: {exception.Source} {Environment.NewLine}" +
                    $"Stack Tree: {exception.StackTrace}");
            }
        }
    }
}