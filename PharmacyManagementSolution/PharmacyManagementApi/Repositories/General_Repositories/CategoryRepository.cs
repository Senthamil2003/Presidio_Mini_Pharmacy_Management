using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Repositories.General_Repositories
{
    public class CategoryRepository : IRepository<int, Category>
    {
        private readonly PharmacyContext _context;

        public CategoryRepository(PharmacyContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Category> Add(Category item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Category cannot be null");

            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while adding the Category. " + ex);
            }
        }

        public async Task<Category> Delete(int key)
        {
            try
            {
                var category = await Get(key);
                _context.Remove(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while deleting the Category. " + ex);
            }
        }

        public async Task<Category> Get(int key)
        {
            try
            {
                return await _context.Categories.SingleOrDefaultAsync(u => u.CategoryId == key)
                    ?? throw new NoCategoryFoundException($"No Category found with given id {key}");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error Occur while fetching data from Category. " + ex);
            }
        }

        public async Task<IEnumerable<Category>> Get()
        {
            try
            {
                return await _context.Categories.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while fetching the Categorys. " + ex);
            }
        }

        public async Task<Category> Update(Category item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Category cannot be null");

            try
            {
                var category = await Get(item.CategoryId);
                _context.Entry(category).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
                return category;
            }
         
            catch (Exception ex)
            {

                throw new RepositoryException("Error occurred while updating the Category. " + ex);
            }
        }
    }

}
