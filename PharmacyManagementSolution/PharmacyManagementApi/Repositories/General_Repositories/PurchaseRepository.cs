using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repositories.General_Repositories
{
    public class PurchaseRepository : IRepository<int, Purchase>
    {
        private readonly PharmacyContext _context;

        public PurchaseRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Purchase> Add(Purchase item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Purchase cannot be null");
            }

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the Purchase. " + ex);
            }
        }
        public async Task<Purchase> Delete(int key)
        {
            try
            {
                var purchase = await Get(key);
                _context.Remove(purchase);
                await _context.SaveChangesAsync();
                return purchase;
            }
      
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the Purchase. " + ex);
            }
        }

        public async Task<Purchase> Get(int key)
        {
            try
            {
                return await _context.Purchases.SingleOrDefaultAsync(u => u.PurchaseId == key)
                    ?? throw new NoPurchaseFoundException($"No Purchase found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Purchase. " + ex);
            }
        }

        public async Task<IEnumerable<Purchase>> Get()
        {
            try
            {
                return await _context.Purchases.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the Purchases. " + ex);
            }
        }

        public async Task<Purchase> Update(Purchase item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Purchase cannot be null");

            try
            {
                var purchase = await Get(item.PurchaseId);
                _context.Entry(purchase).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return purchase;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the Purchase. " + ex);
            }
        }
    }

}
