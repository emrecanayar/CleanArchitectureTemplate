using Core.Domain.Entities;
using Core.Persistence.Contexts;
using Core.Persistence.Repositories;
using webAPI.Application.Services.Repositories;

namespace webAPI.Persistence.Repositories
{
    public class UserOperationClaimRepository : EfRepositoryBase<UserOperationClaim, Guid, BaseDbContext>, IUserOperationClaimRepository
    {
        public UserOperationClaimRepository(BaseDbContext context)
            : base(context)
        {
        }
    }
}
