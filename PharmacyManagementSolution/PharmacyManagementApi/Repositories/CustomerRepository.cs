using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PharmacyManagementApi.Repository
{
    public class CustomerRepository : IReposiroty<int, Customer>
    {
        private readonly PharmacyContext _context;

        public CustomerRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
   
        public async Task<Customer> Add(Customer item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Customer cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the customer. " + ex);
            }
        }

        public async Task<Customer> Delete(int key)
        {
            try
            {
                var customer = await Get(key);
                _context.Remove(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (NoCustomerFoundException)
            {
                throw; 
            }
            catch (Exception ex)
            {
                
                throw new RepositoryException("Error occurred while deleting the customer. "+  ex);
            }
        }

        public async Task<Customer> Get(int key)
        {
            try
            {
                return await _context.Customers.SingleOrDefaultAsync(u => u.CustomerId == key)
                    ?? throw new NoCustomerFoundException($"No Customer found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Customer. "+ ex);            }
        }

        public async Task<IEnumerable<Customer>> Get()
        {
            try
            {
                return await _context.Customers.ToListAsync();
            }
            catch (Exception ex)
            {
       
                throw new RepositoryException("Error occurred while fetching the customers. "+ ex);
            }
        }

        public async Task<Customer> Update(Customer item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Customer cannot be null");

            try
            {
                var customer = await Get(item.CustomerId);
                _context.Entry(customer).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (NoCustomerFoundException)
            {
                throw; 
            }
            catch (Exception ex)
            {
              
                throw new RepositoryException("Error occurred while updating the customer. "+ ex);
            }
        }
    }
}
