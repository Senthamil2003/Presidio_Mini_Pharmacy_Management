using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repositories.General_Repositories
{
    public class BrandRepository : IRepository<int, Brand>
    {
        private readonly PharmacyContext _context;


        public BrandRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Brand> Add(Brand item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Brand cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the Brand. " + ex);
            }
        }

        public async Task<Brand> Delete(int key)
        {
            try
            {
                var Brand = await Get(key);
                _context.Remove(Brand);
                await _context.SaveChangesAsync();
                return Brand;
            }
            catch (NoBrandFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the Brand. " + ex);
            }
        }


        public async Task<Brand> Get(int key)
        {
            try
            {
                return await _context.Brands.SingleOrDefaultAsync(u => u.BrandId == key)
                    ?? throw new NoBrandFoundException($"No Brand found with given id {key}");
            }
            catch (NoBrandFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Brand. " + ex);
            }
        }


        public async Task<IEnumerable<Brand>> Get()
        {
            try
            {
                return await _context.Brands.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the Brand. " + ex);
            }
        }


        public async Task<Brand> Update(Brand item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Brand cannot be null");

            try
            {
                var Brand = await Get(item.BrandId);
                _context.Entry(Brand).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return Brand;
            }
            catch (NoBrandFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the Brand. " + ex);
            }
        }
    }
}
