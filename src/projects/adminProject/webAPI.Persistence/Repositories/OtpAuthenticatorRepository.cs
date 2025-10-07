using Core.Domain.Entities;
using Core.Persistence.Contexts;
using Core.Persistence.Repositories;
using webAPI.Application.Services.Repositories;

namespace webAPI.Persistence.Repositories
{
    public class OtpAuthenticatorRepository : EfRepositoryBase<OtpAuthenticator, Guid, BaseDbContext>, IOtpAuthenticatorRepository
    {
        public OtpAuthenticatorRepository(BaseDbContext context)
            : base(context)
        {
        }
    }
}
