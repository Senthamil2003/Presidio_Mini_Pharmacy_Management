using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repository
{
    public class CartRepository : IReposiroty<int, Cart>
    {
        private readonly PharmacyContext _context;

        public CartRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Cart> Add(Cart item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Cart cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the cart. " + ex);
            }
        }

        public async Task<Cart> Delete(int key)
        {
            try
            {
                var cart = await Get(key);
                _context.Remove(cart);
                await _context.SaveChangesAsync();
                return cart;
            }
            catch (NoCartFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the cart. " + ex);
            }
        }

        public async Task<Cart> Get(int key)
        {
            try
            {
                return await _context.Carts.SingleOrDefaultAsync(u => u.CartId == key)
                    ?? throw new NoCartFoundException($"No Cart found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Cart. " + ex);
            }
        }

        public async Task<IEnumerable<Cart>> Get()
        {
            try
            {
                return await _context.Carts.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the cart. " + ex);
            }
        }

        public async Task<Cart> Update(Cart item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Cart cannot be null");

            try
            {
                var cart = await Get(item.CartId);
                _context.Entry(cart).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return cart;
            }
            catch (NoCartFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the cart. " + ex);
            }
        }
    }

}
