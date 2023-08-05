using Core.CrossCuttingConcerns.Logging.DbLog.Dto;
using Core.CrossCuttingConcerns.Logging.DbLog.MsSQL.Contexts;
using Core.Domain.Entities;
using Core.Helpers.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.CrossCuttingConcerns.Logging.DbLog.MsSQL
{
    public class MSSQLLogService : ILogService
    {
        private readonly IConfiguration _configuration;

        public MSSQLLogService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task CreateLog(LogDto logDto)
        {
            Log log = logDto.ToMap<Log>();
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
                throw exception;
            }
        }

        private LogDbContext createDbContext()
        {
            var builder = new DbContextOptionsBuilder<LogDbContext>();
            builder.UseSqlServer(_configuration.GetConnectionString("LogDbConnectionString"));
            return new LogDbContext(builder.Options);
        }
    }
}