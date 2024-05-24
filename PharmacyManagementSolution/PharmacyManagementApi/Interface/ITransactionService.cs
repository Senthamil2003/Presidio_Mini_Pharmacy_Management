using Microsoft.EntityFrameworkCore.Storage;

namespace PharmacyManagementApi.Interface
{
    public interface ITransactionService
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
