using Core.Domain.Entities;
using Core.Persistence.Repositories;

namespace webAPI.Application.Services.Repositories
{
    public interface IOperationClaimRepository : IAsyncRepository<OperationClaim, Guid>, IRepository<OperationClaim, Guid>
    {
    }
}
