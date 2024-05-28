using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Repositories.General_Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyUnitTest.RepositoryTest
{
    public class PurchaseRepositoryTest
    {
        private IReposiroty<int, Purchase> _purchaseRepo;
        private IReposiroty<int, PurchaseDetail> _purchaseDetailRepo;
        private PharmacyContext context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PharmacyContext>()
                      .UseSqlite("DataSource=:memory:")
                      .Options;

            context = new PharmacyContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            _purchaseRepo=new PurchaseRepository(context);
            _purchaseDetailRepo=new PurchaseDetailRepository(context);
        }

        [Test]
        public async Task PurchaseAddFailTestNull()
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _purchaseRepo.Add(null));
            _purchaseRepo.Add(null);
        }
        [Test]
        public async Task PurchaseAddFailTest()
        {
            Purchase purchase = new Purchase()
            {
                PurchaseDate = DateTime.Now,
                PurchaseId = 1,
                TotalAmount = 10
            };
            await _purchaseRepo.Add(purchase);
            Purchase purchase2 = new Purchase()
            {
                PurchaseDate = DateTime.Now,
                PurchaseId = 1,
                TotalAmount = 10
            };
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _purchaseRepo.Add(purchase2));
            _purchaseRepo.Add(null);
        }
        [Test]
        public async Task PurchaseDelete()
        {
            Purchase purchase = new Purchase()
            {
                PurchaseDate = DateTime.Now,
                PurchaseId = 1,
                TotalAmount = 10
            };
           await _purchaseRepo.Add(purchase);
            var result = _purchaseRepo.Delete(1);
           Assert.IsNotNull(result);
  
           
        }
        [Test]
        public async Task PurchaseDeleteFail()
        {

            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _purchaseRepo.Delete(1));


        }
        [Test]
        public async Task PurchaseUpdateFailNull()
        {

            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () => await _purchaseRepo.Update(null));
        }
        [Test]
        public async Task PurchaseUpdateFail()
        {
            Purchase purchase = new Purchase()
            {
                TotalAmount = 10,
                PurchaseDate = DateTime.Now,
                PurchaseId = 10,
            };
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _purchaseRepo.Update(purchase));
        }

    }
}
