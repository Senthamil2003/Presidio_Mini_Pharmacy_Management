using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Context;

namespace PharmacyManagementApi.Repositories
{
    public class TransactionRepository:ITransactionService
    {
        private readonly PharmacyContext _context;
        public TransactionRepository(PharmacyContext context) {
            _context=context;
        }  
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

    }
}
