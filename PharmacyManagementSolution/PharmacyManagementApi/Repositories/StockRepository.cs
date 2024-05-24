using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repository
{
    public class StockRepository : IReposiroty<int, Stock>
    {
        private readonly PharmacyContext _context;

        public StockRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
   

        public async Task<Stock> Add(Stock item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Stock cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the Stock. " + ex);
            }
        }

        public async Task<Stock> Delete(int key)
        {
            try
            {
                var stock = await Get(key);
                _context.Remove(stock);
                await _context.SaveChangesAsync();
                return stock;
            }
            catch (NoStockFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the Stock. " + ex);
            }
        }

        public async Task<Stock> Get(int key)
        {
            try
            {
                return await _context.Stocks.SingleOrDefaultAsync(u => u.StockId == key)
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

        public async Task<IEnumerable<Stock>> Get()
        {
            try
            {
                return await _context.Stocks.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the Stocks. " + ex);
            }
        }

        public async Task<Stock> Update(Stock item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Stock cannot be null");

            try
            {
                var stock = await Get(item.StockId);
                _context.Entry(stock).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return stock;
            }
            catch (NoStockFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the Stock. " + ex);
            }
        }
    }

}
