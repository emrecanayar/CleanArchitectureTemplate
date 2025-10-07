using AutoMapper;
using Core.CrossCuttingConcerns.Logging.DbLog.Dto;
using Core.CrossCuttingConcerns.Logging.DbLog.MsSQL.Contexts;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.CrossCuttingConcerns.Logging.DbLog.MsSQL
{
    public class MsSqlLogService : ILogService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public MsSqlLogService(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task CreateLog(LogDto logDto)
        {
            Log log = _mapper.Map<Log>(logDto);
            await logToDb(log);
        }

        private async Task logToDb(Log log)
        {
            try
            {
                using var db = createDbContext();
                await db.Logs.AddAsync(log);
                await db.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException($"DB Logging Exception: {exception.Message} {Environment.NewLine} " +
                                                    $"Source: {exception.Source} {Environment.NewLine}" +
                                                    $"Stack Tree: {exception.StackTrace}");
            }
        }

        private LogDbContext createDbContext()
        {
            var builder = new DbContextOptionsBuilder<LogDbContext>();
            builder.UseSqlServer(_configuration.GetConnectionString("ConnectionString"));
            return new LogDbContext(builder.Options);
        }
    }
}
