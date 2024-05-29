using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.Controllers;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Repositories.General_Repositories;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using PharmacyManagementApi.Services;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace PharmacyUnitTest
{
    public class AdminServiceTest:BaseSetup
    {

        [Test]
        public async Task PurchaseSuccessTest()
        {


            var purchaseItem1 = new PurchaseItem
            {
                VendorName = "PharmacyVendor1",
                MedicineName = "Paracetamol",
                MedicineCategory = "Analgesic",
                Amount = 100,
                Quantity = 50,
                ExpiryDate = new DateTime(2025, 12, 31),
                StorageRequirement = "Room Temperature",
                DosageForm = "Tablet"
            };

            var purchaseItem2 = new PurchaseItem
            {
                VendorName = "PharmacyVendor1",
                MedicineName = "Ibuprofen",
                MedicineCategory = "Anti-inflammatory",
                Amount = 150,
                Quantity = 30,
                ExpiryDate = new DateTime(2024, 10, 31),
                StorageRequirement = "Cool and Dry Place",
                DosageForm = "Capsule"
            };

            var purchaseDTO = new PurchaseDTO
            {
                DateTime = DateTime.Now,
                Items = new[] { purchaseItem1, purchaseItem2 }
            };

            SuccessPurchaseDTO result = await _adminService.PurchaseMedicine(purchaseDTO);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.IsNotNull(result);
        }
        [Test]
        public async Task PurchaseFailTest()
        {
            var purchaseItem1 = new PurchaseItem
            {
                VendorName = "sample",
                MedicineName = "Paracetamol",
                MedicineCategory = "Analgesic",
                Amount = 100,
                Quantity = 50,
                ExpiryDate = new DateTime(2025, 12, 31),
                StorageRequirement = "Room Temperature",
                DosageForm = "Tablet"
            };

            var purchaseDTO = new PurchaseDTO
            {
                DateTime = DateTime.Now,
                Items = new[] { purchaseItem1 }
            };

          
            var exception = Assert.ThrowsAsync<NoVendorFoundException>(async () => await _adminService.PurchaseMedicine(purchaseDTO));

          
            Assert.That(exception.Message, Is.EqualTo("No vendor found, add the vendor"));
        }
        [Test]
        public async Task PurchaseException()
        {
            var purchaseItem1 = new PurchaseItem
            {
                VendorName = "sample",
                MedicineName = "Paracetamol",
                MedicineCategory = "Analgesic",
                Amount = 100,
                Quantity = 50,
                ExpiryDate = new DateTime(2025, 12, 31),
                StorageRequirement = "Room Temperature",
                DosageForm = "Tablet"
            };

            var purchaseDTO = new PurchaseDTO
            {
                DateTime = DateTime.Now,
                Items = new[] { purchaseItem1 }
            };


            var exception = Assert.ThrowsAsync<NoVendorFoundException>(async () => await _adminService.PurchaseMedicine(purchaseDTO));
            Assert.Pass();
        }
        [Test]
        public async Task PurcahseToUpdateMedicine()
        {
            var purchaseItem1 = new PurchaseItem
            {
                VendorName = "PharmacyVendor1",
                MedicineName = "Paracetamol",
                MedicineCategory = "Analgesic",
                Amount = 100,
                Quantity = 50,
                ExpiryDate = new DateTime(2025, 12, 31),
                StorageRequirement = "Room Temperature",
                DosageForm = "Tablet"
            };
            var purchaseItem2 = new PurchaseItem
            {
                VendorName = "PharmacyVendor1",
                MedicineName = "Paracetamol",
                MedicineCategory = "Analgesic",
                Amount = 100,
                Quantity = 30,
                ExpiryDate = new DateTime(2025, 12, 31),
                StorageRequirement = "Room Temperature",
                DosageForm = "Tablet"
            };

            var purchaseDTO = new PurchaseDTO
            {
                DateTime = DateTime.Now,
                Items = new[] { purchaseItem1 ,purchaseItem2}
            };
            SuccessPurchaseDTO result = await _adminService.PurchaseMedicine(purchaseDTO);
            Assert.That(result.Code, Is.EqualTo(200));

        }
        [Test]
        public async Task AddVendor()
        {
            VendorDTO vendor =new VendorDTO();

            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _adminService.AddVendor(vendor));
            Assert.Pass();
        }
        [Test]
        public async Task GetAllOrder()
        {
            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 3,
                UserId = 1
            };
            await _cartService.AddToCart(addToCart);
            AddToCartDTO newAddToCart = new AddToCartDTO()
            {
                MedicineId = 2,
                Quantity = 2,
                UserId = 1
            };
            await _cartService.AddToCart(newAddToCart);
            await _cartService.Checkout(1);
            var result =await _adminService.GetAllOrder();
            Assert.That(result.Count(),Is.EqualTo(2));

            Assert.Pass();
        }
        [Test]
        public async Task GetAllOrderFail()
        {

            
            var exception = Assert.ThrowsAsync<NoOrderFoundException>(async () => await _adminService.GetAllOrder());
            Assert.Pass();

            
        }
        [Test]
        public async Task DeliverOrder()
        {
            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 50,
                UserId = 1
            };
            await _cartService.AddToCart(addToCart);
            await _cartService.Checkout(1);
           var result= await _adminService.DeliverOrder(1);


            Assert.That(result.Code, Is.EqualTo(200));
            Assert.Pass();


        }


    }


}
