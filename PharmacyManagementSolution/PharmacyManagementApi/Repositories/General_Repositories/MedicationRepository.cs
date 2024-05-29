using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repositories.General_Repositories
{
    public class MedicationRepository : IRepository<int, Medication>
    {
        private readonly PharmacyContext _context;

        public MedicationRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Medication> Add(Medication item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Medication cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the Medication. " + ex);
            }
        }

        public async Task<Medication> Delete(int key)
        {
            try
            {
                var Medication = await Get(key);
                _context.Remove(Medication);
                await _context.SaveChangesAsync();
                return Medication;
            }

            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the Medication. " + ex);
            }
        }

        public virtual async Task<Medication> Get(int key)
        {
            try
            {
                return await _context.Medications
                    .Include(m=>m.MedicationItems)
                    .SingleOrDefaultAsync(u => u.MedicationId == key)
                    ?? throw new NoMedicationFoundException($"No Medication found with given id {key}");
            }
            catch (NoMedicationFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Medication. " + ex);
            }
        }

        public virtual async Task<IEnumerable<Medication>> Get()
        {
            try
            {
                return await _context.Medications
                    .Include(m => m.MedicationItems)
                    .ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the Medications. " + ex);
            }
        }

        public async Task<Medication> Update(Medication item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Medication cannot be null");

            try
            {
                var Medication = await Get(item.MedicationId);
                _context.Entry(Medication).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return Medication;
            }

            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the Medication. " + ex);
            }
        }
    }
}
