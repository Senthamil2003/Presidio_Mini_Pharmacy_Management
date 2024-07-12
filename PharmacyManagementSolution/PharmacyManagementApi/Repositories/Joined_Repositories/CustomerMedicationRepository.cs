using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Repositories.General_Repositories;

namespace PharmacyManagementApi.Repositories.Joined_Repositories
{
    public class CustomerMedicationRepository:CustomerRepository
    {
        private readonly PharmacyContext _context;
        public CustomerMedicationRepository(PharmacyContext context) : base(context)
        {
            _context = context;
        }
        public async override Task<IEnumerable<Customer>> Get()
        {
            try
            {
                return await _context.Customers
                      .Include(c => c.Medications)
                      .ThenInclude(medi => medi.MedicationItems)
                       .ToListAsync();

            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the customers. " + ex);
            }
        }
        public async override Task<Customer> Get(int key)
        {
            try
            {
                return await _context.Customers
                      .Include(c => c.Medications)
                      .ThenInclude(medi => medi.MedicationItems)
                    .SingleOrDefaultAsync(u => u.CustomerId == key)
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
