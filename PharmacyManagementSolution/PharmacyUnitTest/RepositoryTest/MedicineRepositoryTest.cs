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
    public class MedicineRepositoryTest
    {

        private IRepository<int, Category> _categoryRepo;
        private IRepository<int, Medicine> _medicineRepo;
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
            _categoryRepo = new CategoryRepository(context);
            _medicineRepo = new MedicineRepository(context);
            Category category = new Category()
            {
                CategoryId = 1,
                CategoryName = "sample"
            };
            await _categoryRepo.Add(category);


        }
        [Test]
        public async Task FailAdd()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _medicineRepo.Add(null));
            Assert.That(exception.Message, Is.EqualTo("Medicine cannot be null (Parameter 'item')"));

        }
        [Test]
        public async Task DuplicateFailAdd()
        {
            Medicine medicine = new Medicine()
            {
                MedicineId = 1,
                MedicineName = "aspirin",
                CategoryId = 1
            };
            await _medicineRepo.Add(medicine);

            Medicine medicine1 = new Medicine()
            {
                MedicineId = 1,
                MedicineName = "aspirin",
                CategoryId = 1
            };
          

            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _medicineRepo.Add(medicine1));

        }
        [Test]
        public async Task Delete()
        {
            Medicine medicine = new Medicine()
            {
                MedicineName = "aspirin",
                CategoryId = 1
            };
            await _medicineRepo.Add(medicine);

            
            var reult = await _medicineRepo.Delete(1);
            Assert.That(reult.MedicineId, Is.EqualTo(1));



        }
        [Test]
        public async Task MedicineDelete()
        {
            Medicine medicine = new Medicine()
            {
                MedicineName = "aspirin",
                CategoryId = 1
            };
            await _medicineRepo.Add(medicine);

          
            var reult = await _medicineRepo.Delete(1);
            Assert.That(reult.MedicineId, Is.EqualTo(1));



        }
        [Test]
        public async Task FailDelete()
        {
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _medicineRepo.Delete(1));

        }
        [Test]
        public async Task UpdateCategory()
        {
            Medicine medicine = new Medicine()
            {
                MedicineId = 1,
                MedicineName = "aspirin",
                CategoryId = 1
            };
            await _medicineRepo.Add(medicine);

            Medicine medicine1 = new Medicine()
            {
                MedicineId = 1,
                MedicineName = "aspirin1",
                CategoryId = 1
            };
          

            var result = await _medicineRepo.Update(medicine1);
            Assert.That(result.CategoryId, Is.EqualTo(1));

        }
        [Test]
        public async Task UpdateCategoryFail()
        {
            Medicine medicine = new Medicine()
            {
                MedicineName = "aspirin",
                CategoryId = 1
            };


            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _medicineRepo.Update(medicine));

        }
    }
}
