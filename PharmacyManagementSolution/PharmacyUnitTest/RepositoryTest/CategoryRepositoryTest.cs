using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Repositories.General_Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyUnitTest.RepositoryTest
{
    public class CategoryRepositoryTest:BaseSetup
    {
        private IRepository<int, Category> _categoryRepo;
        private PharmacyContext context;
        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<PharmacyContext>()
          .UseSqlite("DataSource=:memory:")
          .Options;

            context = new PharmacyContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            _categoryRepo=new CategoryRepository(context);


        }
        [Test]
        public async Task FailAdd()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _categoryRepo.Add(null));
            Assert.That(exception.Message, Is.EqualTo("Category cannot be null (Parameter 'item')"));

            

        }
        [Test]
        public async Task DuplicateFailAdd()
        {
            Category category = new Category()
            {
                CategoryId = 1,
                CategoryName = "sample"
            };
            await _categoryRepo.Add(category);
            Category category1 = new Category()
            {
                CategoryId = 1,
                CategoryName = "sample"
            };
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _categoryRepo.Add(category1));

        }
        [Test]
        public async Task Delete()
        {
            Category category = new Category()
            {
                CategoryId = 1,
                CategoryName = "sample"
            };
            await _categoryRepo.Add(category);
            var reult =await _categoryRepo.Delete(1);
            Assert.That(reult.CategoryId, Is.EqualTo(1));   



        }
        [Test]
        public async Task DeleteFail()
        {
            Category category = new Category()
            {
                CategoryId = 1,
                CategoryName = "sample"
            };
            await _categoryRepo.Add(category);
            var reult = await _categoryRepo.Delete(1);
            Assert.That(reult.CategoryId, Is.EqualTo(1));



        }
        [Test]
        public async Task FailDelete()
        {
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _categoryRepo.Delete(1));
         
        }
        [Test]
        public async Task UpdateCategory()
        {
            Category category = new Category()
            {
                CategoryId = 1,
                CategoryName = "sample"
            }; 

            await _categoryRepo.Add(category);
            Category category1 = new Category()
            {
                CategoryId = 1,
                CategoryName = "dample"
            };
            var result = await _categoryRepo.Update(category1);
            Assert.That(result.CategoryId, Is.EqualTo(1));

        }
        [Test]
        public async Task UpdateCategoryFail()
        {
            Category category = new Category()
            {
                CategoryId = 1,
                CategoryName = "sample"
            };

            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _categoryRepo.Update(category));
             



        }

    }
}
