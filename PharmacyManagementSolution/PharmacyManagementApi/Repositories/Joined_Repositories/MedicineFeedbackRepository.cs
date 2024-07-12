using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Repositories.General_Repositories;

namespace PharmacyManagementApi.Repositories.Joined_Repositories
{
    public class MedicineFeedbackRepository : MedicineRepository
    {
        private readonly PharmacyContext _context;
        public MedicineFeedbackRepository(PharmacyContext context) : base(context)
        {
            _context = context;
        }
        public async override Task<IEnumerable<Medicine>> Get()
        {
            try
            {
                return await _context.Medicines
                      .Include(c => c.Feedbacks)
                      .ThenInclude(f => f.Customer)
                       .ToListAsync();

            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the customers. " + ex);
            }
        }
        public async override Task<Medicine> Get(int key)
        {
            try
            {
                return await _context.Medicines
                    .Include(c => c.Feedbacks)
                    .ThenInclude(f=>f.Customer)
                    .SingleOrDefaultAsync(u => u.MedicineId == key)
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
