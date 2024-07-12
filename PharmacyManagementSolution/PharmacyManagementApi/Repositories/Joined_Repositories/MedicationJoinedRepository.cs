using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Repositories.General_Repositories;

namespace PharmacyManagementApi.Repositories.Joined_Repositories
{
    public class MedicationJoinedRepository:MedicationRepository
    {
        private readonly PharmacyContext _context;
        public MedicationJoinedRepository(PharmacyContext context) : base(context)
        {
            _context = context;
        }
        public async override Task<IEnumerable<Medication>> Get()
        {
            try
            {
                return await _context.Medications
                      .Include(m => m.MedicationItems)
                      .ThenInclude(medi => medi.Medicine)
                      .ThenInclude(medicine=>medicine.Brand)
                       .ToListAsync();

            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the customers. " + ex);
            }
        }
        public async override Task<Medication> Get(int key)
        {
            try
            {
                return await  _context.Medications
                      .Include(m => m.MedicationItems)
                      .ThenInclude(medi => medi.Medicine)
                      .ThenInclude(medicine => medicine.Brand)
                    .SingleOrDefaultAsync(u => u.MedicationId == key)
                    ?? throw new NoCustomerFoundException($"No Customer found with given id {key}");
            }
            catch (NoCustomerFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Customer. " + ex);
            }
        }
    }
}
