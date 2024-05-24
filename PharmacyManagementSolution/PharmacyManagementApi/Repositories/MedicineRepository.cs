using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repository
{
    public class MedicineRepository : IReposiroty<int, Medicine>
    {
        private readonly PharmacyContext _context;

        public MedicineRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<Medicine> Add(Medicine item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Medicine cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the Medicine. " + ex);
            }
        }

        public async Task<Medicine> Delete(int key)
        {
            try
            {
                var medicine = await Get(key);
                _context.Remove(medicine);
                await _context.SaveChangesAsync();
                return medicine;
            }
            catch (NoMedicineFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the Medicine. " + ex);
            }
        }

        public async Task<Medicine> Get(int key)
        {
            try
            {
                return await _context.Medicines.SingleOrDefaultAsync(u => u.MedicineId == key)
                    ?? throw new NoMedicineFoundException($"No Medicine found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Medicine. " + ex);
            }
        }

        public async Task<IEnumerable<Medicine>> Get()
        {
            try
            {
                return await _context.Medicines.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the Medicines. " + ex);
            }
        }

        public async Task<Medicine> Update(Medicine item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Medicine cannot be null");

            try
            {
                var medicine = await Get(item.MedicineId);
                _context.Entry(medicine).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return medicine;
            }
            catch (NoMedicineFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the Medicine. " + ex);
            }
        }
    }

}
