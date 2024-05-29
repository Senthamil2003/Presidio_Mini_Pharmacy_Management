using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PharmacyManagementApi.Repositories.General_Repositories
{
  
    public class DeliveryDetailRepository : IRepository<int, DeliveryDetail>
    {
        private readonly PharmacyContext _context;

      
        public DeliveryDetailRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

      
        public async Task<DeliveryDetail> Add(DeliveryDetail item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "DeliveryDetail cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the DeliveryDetail. " + ex);
            }
        }

      
        public async Task<DeliveryDetail> Delete(int key)
        {
            try
            {
                var deliveryDetail = await Get(key);
                _context.Remove(deliveryDetail);
                await _context.SaveChangesAsync();
                return deliveryDetail;
            }
            catch (NoDeliveryDetailFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the DeliveryDetail. " + ex);
            }
        }

        public async Task<DeliveryDetail> Get(int key)
        {
            try
            {
                return await _context.DeliveryDetails.SingleOrDefaultAsync(u => u.DeliveryId == key)
                    ?? throw new NoDeliveryDetailFoundException($"No DeliveryDetail found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from DeliveryDetail. " + ex);
            }
        }

     
        public async Task<IEnumerable<DeliveryDetail>> Get()
        {
            try
            {
                return await _context.DeliveryDetails.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the DeliveryDetails. " + ex);
            }
        }

        public async Task<DeliveryDetail> Update(DeliveryDetail item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "DeliveryDetail cannot be null");

            try
            {
                var deliveryDetail = await Get(item.DeliveryId);
                _context.Entry(deliveryDetail).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return deliveryDetail;
            }
            catch (NoDeliveryDetailFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the DeliveryDetail. " + ex);
            }
        }
    }
}
