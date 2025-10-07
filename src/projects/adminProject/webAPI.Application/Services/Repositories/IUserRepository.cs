using Core.Domain.Entities;
using Core.Persistence.Repositories;

namespace webAPI.Application.Services.Repositories
{
    public interface IUserRepository : IAsyncRepository<User, Guid>, IRepository<User, Guid>
    {
    }
}
