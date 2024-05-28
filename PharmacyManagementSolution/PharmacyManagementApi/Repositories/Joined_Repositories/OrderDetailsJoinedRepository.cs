using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Repositories.General_Repositories;

namespace PharmacyManagementApi.Repositories.Joined_Repositories
{
    public class OrderDetailsJoinedRepository:OrderDetailRepository
    {
        private readonly PharmacyContext _context;
        public OrderDetailsJoinedRepository(PharmacyContext context):base(context) {

            _context=context;

        }
        public async override Task<IEnumerable<OrderDetail>> Get()
        {
            try
            {
                return await _context.OrderDetails
                    .Include(od => od.Medicine)
                    .ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the OrderDetails. " + ex);
            }
        }
        public async override Task<OrderDetail> Get(int key)
        {
            try
            {
                return await _context.OrderDetails
                    .Include(od=>od.DeliveryDetails)      
                    .SingleOrDefaultAsync(u => u.OrderDetailId == key)
                    ?? throw new NoOrderDetailFoundException($"No OrderDetail found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from OrderDetail. " + ex);
            }
        }
    }
}
