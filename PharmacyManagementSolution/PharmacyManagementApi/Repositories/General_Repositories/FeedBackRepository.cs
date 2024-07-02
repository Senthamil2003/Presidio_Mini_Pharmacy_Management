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
    /// <summary>
    /// Repository for performing CRUD operations on the Feedback entity.
    /// </summary>
    public class FeedbackRepository : IRepository<int, Feedback>
    {
        private readonly PharmacyContext _context;

      
        public FeedbackRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

      
        public async Task<Feedback> Add(Feedback item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Feedback cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while adding the Feedback. " + ex);
            }
        }

      
        public async Task<Feedback> Delete(int key)
        {
            try
            {
                var feedback = await Get(key);
                _context.Remove(feedback);
                await _context.SaveChangesAsync();
                return feedback;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while deleting the Feedback. " + ex);
            }
        }

        public virtual async Task<Feedback> Get(int key)
        {
            try
            {
                return await _context.Feedbacks
                    .Include(f=>f.Customer)
                    .SingleOrDefaultAsync(u => u.FeedbackId == key)
                    ?? throw new NoFeedbackFoundException($"No Feedback found with given id {key}");
            }
            catch (NoFeedbackFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Feedback. " + ex);
            }
        }

    
        public virtual async Task<IEnumerable<Feedback>> Get()
        {
            try
            {
                return await _context.Feedbacks
                    .Include(f=>f.Customer)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while fetching the Feedbacks. " + ex);
            }
        }

        public async Task<Feedback> Update(Feedback item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Feedback cannot be null");

            try
            {
                var feedback = await Get(item.FeedbackId);
                _context.Entry(feedback).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return feedback;
            }

            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the Feedback. " + ex);
            }
        }
    }
}
