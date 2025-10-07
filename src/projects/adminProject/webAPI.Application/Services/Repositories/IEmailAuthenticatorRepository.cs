using Core.Domain.Entities;
using Core.Persistence.Repositories;

namespace webAPI.Application.Services.Repositories
{
    public interface IEmailAuthenticatorRepository : IAsyncRepository<EmailAuthenticator, Guid>, IRepository<EmailAuthenticator, Guid>
    {
    }
}
