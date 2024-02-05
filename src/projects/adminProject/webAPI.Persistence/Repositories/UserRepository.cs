using Core.Domain.Entities;
using Core.Persistence.Contexts;
using Core.Persistence.Repositories;
using webAPI.Application.Services.Repositories;

namespace webAPI.Persistence.Repositories
{
    public class UserRepository : EfRepositoryBase<User, Guid, BaseDbContext>, IUserRepository
    {
        public UserRepository(BaseDbContext context)
            : base(context) { }
    }
}
