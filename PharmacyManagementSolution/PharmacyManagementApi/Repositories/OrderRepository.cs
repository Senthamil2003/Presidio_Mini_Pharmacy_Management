using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repository
{
    public class OrderRepository : IReposiroty<int, Order>
    {
        private readonly PharmacyContext _context;

        public OrderRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<Order> Add(Order item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Order cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the Order. " + ex);
            }
        }

        public async Task<Order> Delete(int key)
        {
            try
            {
                var order = await Get(key);
                _context.Remove(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (NoOrderFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the Order. " + ex);
            }
        }

        public async Task<Order> Get(int key)
        {
            try
            {
                return await _context.Orders.SingleOrDefaultAsync(u => u.OrderId == key)
                    ?? throw new NoOrderFoundException($"No Order found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Order. " + ex);
            }
        }

        public async Task<IEnumerable<Order>> Get()
        {
            try
            {
                return await _context.Orders.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the Orders. " + ex);
            }
        }

        public async Task<Order> Update(Order item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Order cannot be null");

            try
            {
                var order = await Get(item.OrderId);
                _context.Entry(order).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (NoOrderFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the Order. " + ex);
            }
        }
    }

}
