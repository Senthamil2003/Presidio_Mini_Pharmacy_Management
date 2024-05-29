using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repositories.General_Repositories
{
    public class MedicationItemRepository : IRepository<int, MedicationItem>
    {
        private readonly PharmacyContext _context;

        public MedicationItemRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<MedicationItem> Add(MedicationItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "MedicationItem cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the MedicationItem. " + ex);
            }
        }

        public async Task<MedicationItem> Delete(int key)
        {
            try
            {
                var MedicationItem = await Get(key);
                _context.Remove(MedicationItem);
                await _context.SaveChangesAsync();
                return MedicationItem;
            }

            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the MedicationItem. " + ex);
            }
        }

        public virtual async Task<MedicationItem> Get(int key)
        {
            try
            {
                return await _context.MedicationItems.SingleOrDefaultAsync(u => u.MedicationItemId == key)
                    ?? throw new NoMedicationItemFoundException($"No MedicationItem found with given id {key}");
            }
            catch (NoMedicationItemFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from MedicationItem. " + ex);
            }
        }

        public virtual async Task<IEnumerable<MedicationItem>> Get()
        {
            try
            {
                return await _context.MedicationItems.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the MedicationItems. " + ex);
            }
        }

        public async Task<MedicationItem> Update(MedicationItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "MedicationItem cannot be null");

            try
            {
                var MedicationItem = await Get(item.MedicationItemId);
                _context.Entry(MedicationItem).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return MedicationItem;
            }

            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the MedicationItem. " + ex);
            }
        }
    }
}
