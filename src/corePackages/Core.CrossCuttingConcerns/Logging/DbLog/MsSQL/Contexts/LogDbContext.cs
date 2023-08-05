using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.CrossCuttingConcerns.Logging.DbLog.MsSQL.Contexts
{
    public class LogDbContext : DbContext
    {
        public LogDbContext()
        {
        }

        public LogDbContext(DbContextOptions<LogDbContext> options)
             : base(options)
        {
        }

        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}