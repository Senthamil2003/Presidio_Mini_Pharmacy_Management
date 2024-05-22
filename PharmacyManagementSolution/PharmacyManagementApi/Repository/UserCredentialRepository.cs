using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repository
{
    public class UserCredentialRepository : IReposiroty<int, UserCredential>
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

        public async Task<UserCredential> Delete(int key)
        {
            try
            {
                var UserCredential = await Get(key);
                _context.Remove(UserCredential);
                await _context.SaveChangesAsync();
                return UserCredential;
            }
            catch (NoUserCredentialFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the UserCredential. " + ex);
            }
        }

        public async Task<UserCredential> Get(int key)
        {
            try
            {
                return await _context.UserCredentials.SingleOrDefaultAsync(u => u.UserCredentialId == key)
                    ?? throw new NoUserCredentialFoundException($"No UserCredential found with given id {key}");
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
                var UserCredential = await Get(item.UserCredentialId);
                _context.Entry(UserCredential).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return UserCredential;
            }
            catch (NoUserCredentialFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the UserCredential. " + ex);
            }
        }
    }

}
