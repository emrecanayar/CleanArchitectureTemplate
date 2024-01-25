using Core.Domain.Entities;
using Core.Persistence.Repositories;

namespace webAPI.Application.Services.Repositories
{
    public interface IUploadedFileRepository : IAsyncRepository<UploadedFile, Guid>, IRepository<UploadedFile, Guid>
    {
    }
}
