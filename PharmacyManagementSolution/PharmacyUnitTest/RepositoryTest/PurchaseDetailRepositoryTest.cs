using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Repositories.General_Repositories;
using PharmacyManagementApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyUnitTest.RepositoryTest
{
    public class PurchaseDetailRepositoryTest
    {
        private IRepository<int, Purchase> _purchaseRepo;
        private IRepository<int, PurchaseDetail> _purchaseDetailRepo;
        private PharmacyContext context;
        private IRepository<int, Medicine> _medicineRepo;
        private IRepository<int, Category> _categoryRepo;
        private IRepository<int, Vendor> _vendorRepo;

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
            _purchaseRepo = new PurchaseRepository(context);
            _purchaseDetailRepo = new PurchaseDetailRepository(context);
            _medicineRepo = new MedicineRepository(context);    
            _categoryRepo = new CategoryRepository(context);
            _vendorRepo = new VendorRepository(context);
            Category category = new Category()
            {
                CategoryName = "sample",

            };
            Vendor vendor = new Vendor()
            {
                VendorName = "sample",
                Address = "abc",
                Phone = "12122312"
            };
           await _vendorRepo.Add(vendor);
            await _categoryRepo.Add(category);
            Medicine medicine = new Medicine()
            {
                MedicineName = "aspirin",
                CategoryId = 1,
                CurrentQuantity = 1,

            };
            await _medicineRepo.Add(medicine);    
            
            Purchase purchase = new Purchase()
            {
                PurchaseDate = DateTime.Now,
                PurchaseId = 1,
                TotalAmount = 10
                
            };
            await _purchaseRepo.Add(purchase);
        }

        [Test]
        public async Task purchaseDetailNullAddFail()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _purchaseDetailRepo.Add(null));
           
        }
        [Test]
        public async Task purchaseDetailDuplicateAddFail()
        {
            PurchaseDetail purchaseDetail = new PurchaseDetail()
            {
                PurchaseDetailId=1,
                Amount = 10,
                DosageForm = "sample",
                ExpiryDate = DateTime.Now,
                PurchaseId = 1,
                MedicineId = 1,
                Quantity = 1,
                StorageRequirement = "nothing",
                TotalSum = 10,
                VendorId = 1,
            };
            await _purchaseDetailRepo.Add(purchaseDetail);
            PurchaseDetail purchaseDetail1 = new PurchaseDetail()
            {
                PurchaseDetailId=1,
                Amount = 10,
                DosageForm = "sample",
                ExpiryDate = DateTime.Now,
                PurchaseId = 1,
                MedicineId = 1,
                Quantity = 1,
                StorageRequirement = "nothing",
                TotalSum = 10,
                VendorId = 1,
            };


            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _purchaseDetailRepo.Add(purchaseDetail1));

        }
        [Test]
        public async Task purchaseDetailDelete()
        {
            PurchaseDetail purchaseDetail = new PurchaseDetail()
            {
                PurchaseDetailId = 1,
                Amount = 10,
                DosageForm = "sample",
                ExpiryDate = DateTime.Now,
                PurchaseId = 1,
                MedicineId = 1,
                Quantity = 1,
                StorageRequirement = "nothing",
                TotalSum = 10,
                VendorId = 1,
            };
            await _purchaseDetailRepo.Add(purchaseDetail);
           var result= await _purchaseDetailRepo.Delete(1);
            Assert.That(result, Is.Not.Null);   

        }
        [Test]
        public async Task purchaseDetailNullDeleteFail()
        {
           var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _purchaseDetailRepo.Delete(1));
        
        }
        [Test]
        public async Task PurchaseGetAll()
        {
            var result=await _purchaseDetailRepo.Get();
            Assert.IsNotNull(result);

        }
        [Test]
        public async Task PurchaseUpdate()
        {
            PurchaseDetail purchaseDetail = new PurchaseDetail()
            {
                PurchaseDetailId = 1,
                Amount = 10,
                DosageForm = "sample",
                ExpiryDate = DateTime.Now,
                PurchaseId = 1,
                MedicineId = 1,
                Quantity = 1,
                StorageRequirement = "nothing",
                TotalSum = 10,
                VendorId = 1,
            };
            await _purchaseDetailRepo.Add(purchaseDetail);


            PurchaseDetail purchaseDetail1 = new PurchaseDetail()
            {
                PurchaseDetailId = 1,
                Amount = 10,
                DosageForm = "sample",
                ExpiryDate = DateTime.Now,
                PurchaseId = 1,
                MedicineId = 1,
                Quantity = 1,
                StorageRequirement = "soething",
                TotalSum = 10,
                VendorId = 1,
            };
            var result = await _purchaseDetailRepo.Update(purchaseDetail1);
            Assert.IsNotNull(result);

        }
        [Test]
        public async Task PurchaseUpdateNullFail()
        {

            PurchaseDetail purchaseDetail = new PurchaseDetail()
            {
                PurchaseDetailId = 1,
                Amount = 10,
                DosageForm = "sample",
                ExpiryDate = DateTime.Now,
                PurchaseId = 1,
                MedicineId = 1,
                Quantity = 1,
                StorageRequirement = "nothing",
                TotalSum = 10,
                VendorId = 1,
            };
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _purchaseDetailRepo.Update(purchaseDetail));
           
            Assert.Pass();

        }


    }
}
