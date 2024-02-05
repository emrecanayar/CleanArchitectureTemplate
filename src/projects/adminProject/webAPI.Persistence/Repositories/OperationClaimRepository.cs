using Core.Domain.Entities;
using Core.Persistence.Contexts;
using Core.Persistence.Repositories;
using webAPI.Application.Services.Repositories;

namespace webAPI.Persistence.Repositories
{
    public class OperationClaimRepository : EfRepositoryBase<OperationClaim, Guid, BaseDbContext>, IOperationClaimRepository
    {
        public OperationClaimRepository(BaseDbContext context)
            : base(context) { }
    }
}
