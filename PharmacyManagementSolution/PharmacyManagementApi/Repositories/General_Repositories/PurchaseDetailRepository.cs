using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repositories.General_Repositories
{
    public class PurchaseDetailRepository : IReposiroty<int, PurchaseDetail>
    {
        private readonly PharmacyContext _context;

        public PurchaseDetailRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<PurchaseDetail> Add(PurchaseDetail item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "PurchaseDetail cannot be null");
            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the PurchaseDetail. " + ex);
            }
        }

        public async Task<PurchaseDetail> Delete(int key)
        {
            try
            {
                var purchaseDetail = await Get(key);
                _context.Remove(purchaseDetail);
                await _context.SaveChangesAsync();
                return purchaseDetail;
            }
            catch (NoPurchaseDetailFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the PurchaseDetail. " + ex);
            }
        }

        public async Task<PurchaseDetail> Get(int key)
        {
            try
            {
                return await _context.PurchaseDetails.SingleOrDefaultAsync(u => u.PurchaseDetailId == key)
                    ?? throw new NoPurchaseDetailFoundException($"No PurchaseDetail found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from PurchaseDetail. " + ex);
            }
        }

        public async Task<IEnumerable<PurchaseDetail>> Get()
        {
            try
            {
                return await _context.PurchaseDetails.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the PurchaseDetails. " + ex);
            }
        }

        public async Task<PurchaseDetail> Update(PurchaseDetail item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "PurchaseDetail cannot be null");

            try
            {
                var purchaseDetail = await Get(item.PurchaseDetailId);
                _context.Entry(purchaseDetail).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return purchaseDetail;
            }
            catch (NoPurchaseDetailFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the PurchaseDetail. " + ex);
            }
        }
    }

}
