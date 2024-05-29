using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Repositories.General_Repositories;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using PharmacyManagementApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyUnitTest
{
    public class ViewServiceTest:BaseSetup
    {

        [Test]
        public async Task ShowAllProduct()
        {

            var result = await _viewService.ShowAllProduct();
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Length, Is.EqualTo(2));

            var paracetamol = result.FirstOrDefault(p => p.MedicineName == "Paracetamol");
            var ibuprofen = result.FirstOrDefault(p => p.MedicineName == "Ibuprofen");

            Assert.IsNotNull(paracetamol);
            Assert.IsNotNull(ibuprofen);

            Assert.That(paracetamol.AvailableQuantity, Is.EqualTo(50));
            Assert.That(ibuprofen.AvailableQuantity, Is.EqualTo(30));
        }
   

        [Test]
        public async Task ShowProductQuantityCheck()
        {

            var result = await _viewService.ShowAllProduct();
            var paracetamol = result.FirstOrDefault(p => p.MedicineName == "Paracetamol");
            var ibuprofen = result.FirstOrDefault(p => p.MedicineName == "Ibuprofen");
            Assert.IsNotNull(paracetamol);
            Assert.IsNotNull(ibuprofen);
            Assert.That(paracetamol.AvailableQuantity, Is.EqualTo(50));
            Assert.That(ibuprofen.AvailableQuantity, Is.EqualTo(30));
        }
        [Test]
        public async Task GetAllMyOrder()
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
            await _adminService.DeliverOrder(1);
            var result = await _viewService.GetAllOrders(1);
            Assert.That(result.Count(), Is.EqualTo(2));

            Assert.Pass();

        }
        [Test]
        public async Task GetAllMyOrderFail()
        {


            var exception = Assert.ThrowsAsync<NoOrderFoundException>(async () => await _viewService.GetAllOrders(1));
            Assert.That(exception.Message, Is.EqualTo("There is no order for the customer"));

            Assert.Pass();

        }
        [Test]
        public async Task ViewMyMedication()
        {
            var item1 = new MedicationItemDTO
            {
                MedicineId = 1,
                Quantity = 1,
            };

            var add = new AddMedicationDTO
            {
                CustomerId = 1,
                MedicationName = "sample",
                medicationItems = new[] { item1 }
            };

             await _medicationService.AddMedication(add);

            var result = await _viewService.ViewMyMedications(1);

           
            Assert.That(result.Count, Is.EqualTo(1));

            Assert.Pass();

        }
        [Test]
        public async Task ViewMyMedicationFail()
        {
            var exception = Assert.ThrowsAsync<NoMedicationFoundException>(async () => await _viewService.ViewMyMedications(1));
            Assert.That(exception.Message, Is.EqualTo("Customer have no medication"));
           

            Assert.Pass();

        }



    }
}
