using Core.Domain.Entities.Base;
using Core.Persistence.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Core.Persistence.Contexts
{
    public class BaseDbContext : DbContext
    {
        protected IConfiguration Configuration { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BaseDbContext(DbContextOptions dbContextOptions, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(dbContextOptions)
        {
            Configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.RegisterAllEntities<Entity>(Assembly.GetExecutingAssembly());
            modelBuilder.RegisterAllConfigurations(Assembly.GetExecutingAssembly());

        }
        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is Entity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;
                var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "anonymous";

                if (entity.State == EntityState.Added)
                {
                    ((Entity)entity.Entity).CreatedBy = user;
                    ((Entity)entity.Entity).CreatedDate = now;
                }

                ((Entity)entity.Entity).ModifiedBy = user;
                ((Entity)entity.Entity).ModifiedDate = now;
            }
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            var entities = ChangeTracker.Entries().Where(x => x.Entity is Entity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;
                var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "anonymous";

                if (entity.State == EntityState.Added)
                {
                    ((Entity)entity.Entity).CreatedBy = user;
                    ((Entity)entity.Entity).CreatedDate = now;
                }

                ((Entity)entity.Entity).ModifiedBy = user;
                ((Entity)entity.Entity).ModifiedDate = now;
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}