using Core.Domain.Entities;
using Core.Persistence.Contexts;
using Core.Persistence.Repositories;
using webAPI.Application.Services.Repositories;

namespace webAPI.Persistence.Repositories
{
    public class UploadedFileRepository : EfRepositoryBase<UploadedFile, Guid, BaseDbContext>, IUploadedFileRepository
    {
        public UploadedFileRepository(BaseDbContext context)
            : base(context)
        {
        }
    }
}
