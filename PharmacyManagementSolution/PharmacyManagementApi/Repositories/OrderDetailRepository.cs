using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repository
{
    public class OrderDetailRepository : IReposiroty<int, OrderDetail>
    {
        private readonly PharmacyContext _context;

        public OrderDetailRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<OrderDetail> Add(OrderDetail item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "OrderDetail cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the OrderDetail. " + ex);
            }
        }

        public async Task<OrderDetail> Delete(int key)
        {
            try
            {
                var orderDetail = await Get(key);
                _context.Remove(orderDetail);
                await _context.SaveChangesAsync();
                return orderDetail;
            }
            catch (NoOrderDetailFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the OrderDetail. " + ex);
            }
        }

        public async Task<OrderDetail> Get(int key)
        {
            try
            {
                return await _context.OrderDetails.SingleOrDefaultAsync(u => u.OrderDetailId == key)
                    ?? throw new NoOrderDetailFoundException($"No OrderDetail found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from OrderDetail. " + ex);
            }
        }

        public async Task<IEnumerable<OrderDetail>> Get()
        {
            try
            {
                return await _context.OrderDetails.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the OrderDetails. " + ex);
            }
        }

        public async Task<OrderDetail> Update(OrderDetail item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "OrderDetail cannot be null");

            try
            {
                var orderDetail = await Get(item.OrderDetailId);
                _context.Entry(orderDetail).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return orderDetail;
            }
            catch (NoOrderDetailFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the OrderDetail. " + ex);
            }
        }
    }

}
