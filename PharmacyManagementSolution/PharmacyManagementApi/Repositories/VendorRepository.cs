using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repository
{
    public class VendorRepository : IReposiroty<int, Vendor>
    {
        private readonly PharmacyContext _context;

        public VendorRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<Vendor> Add(Vendor item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Vendor cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the Vendor. " + ex);
            }
        }

        public async Task<Vendor> Delete(int key)
        {
            try
            {
                var vendor = await Get(key);
                _context.Remove(vendor);
                await _context.SaveChangesAsync();
                return vendor;
            }
            catch (NoVendorFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the Vendor. " + ex);
            }
        }

        public async Task<Vendor> Get(int key)
        {
            try
            {
                return await _context.Vendors.SingleOrDefaultAsync(u => u.VendorId == key)
                    ?? throw new NoVendorFoundException($"No Vendor found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Vendor. " + ex);
            }
        }

        public async Task<IEnumerable<Vendor>> Get()
        {
            try
            {
                return await _context.Vendors.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the Vendors. " + ex);
            }
        }

        public async Task<Vendor> Update(Vendor item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Vendor cannot be null");

            try
            {
                var vendor = await Get(item.VendorId);
                _context.Entry(vendor).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return vendor;
            }
            catch (NoVendorFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the Vendor. " + ex);
            }
        }
    }
}
