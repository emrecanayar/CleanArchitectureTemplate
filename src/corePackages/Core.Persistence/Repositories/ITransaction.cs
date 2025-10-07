using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Persistence.Repositories;

public interface ITransaction
{
    IDbContextTransaction BeginTransaction();

    Task<IDbContextTransaction> BeginTransactionAsync();

    void CommitTransaction();

    Task CommitTransactionAsync();

    void RollbackTransaction();

    Task RollbackTransactionAsync();
}
