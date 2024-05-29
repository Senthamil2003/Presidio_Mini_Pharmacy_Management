using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repositories.General_Repositories
{
    public class UserCredentialRepository : IRepository<string, UserCredential>
    {
        private readonly PharmacyContext _context;

        public UserCredentialRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<UserCredential> Add(UserCredential item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "UserCredential cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the UserCredential. " + ex);
            }
        }

        public async Task<UserCredential> Delete(string key)
        {
            try
            {
                var userCredential = await Get(key);
                _context.Remove(userCredential);
                await _context.SaveChangesAsync();
                return userCredential;
            }
       
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the UserCredential. " + ex);
            }
        }

        public async Task<UserCredential> Get(string key)
        {
            try
            {
                return await _context.UserCredentials.SingleOrDefaultAsync(u => u.Email == key)
                    ?? throw new NoUserCredentialFoundException($"No user found with given id {key}");
            }
            catch (NoUserCredentialFoundException)
            {
                throw;
            }
       

            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from UserCredential. " + ex);
            }
        }

        public async Task<IEnumerable<UserCredential>> Get()
        {
            try
            {
                return await _context.UserCredentials.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the UserCredentials. " + ex);
            }
        }

        public async Task<UserCredential> Update(UserCredential item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "UserCredential cannot be null");

            try
            {
                var userCredential = await Get(item.Email);
                _context.Entry(userCredential).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return userCredential;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error occurred while updating the UserCredential. " + ex);
            }
        }
    }

}
