using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Repositories.General_Repositories;

namespace PharmacyManagementApi.Repositories.Joined_Repositories
{
    public class StockJoinedRepository:StockRepository
    {
        private readonly PharmacyContext _context;
        public StockJoinedRepository(PharmacyContext context):base(context){
            _context=context;
        }
        public async override Task<IEnumerable<Stock>> Get()
        {
            try
            {
                var result = await _context.Stocks
                    .Include(x => x.Medicine)
                    .ThenInclude(M => M.Category).ToListAsync();
                 return result;


            }
            catch
            {
                throw;
            }
        }
        public async override Task<Stock> Get(int key)
        {
            try
            {
                return await _context.Stocks
                      .Include(x => x.Medicine)
                      .ThenInclude(M => M.Category)
                      .SingleOrDefaultAsync(u => u.StockId == key)
                    ?? throw new NoStockFoundException($"No Stock found with given id {key}");
            }
            catch (NoStockFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Stock. " + ex);
            }
        }
    }
}
