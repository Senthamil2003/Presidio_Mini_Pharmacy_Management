using Castle.Core.Resource;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PharmacyUnitTest
{
    public class MedicationServiceTest:BaseSetup
    {
        [Test]
        public async Task AddMedication()
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

            var result = await _medicationService.AddMedication(add);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Medication added successfully"));
            Assert.That(result.MedicationId, Is.GreaterThan(0));
        }
        [Test]
        public async Task AddMedicationFail()
        {
            var item1 = new MedicationItemDTO
            {
                MedicineId = 5,
                Quantity = 1,
            };

            var add = new AddMedicationDTO
            {
                CustomerId = 2,
                MedicationName = "sample",
                medicationItems = new[] { item1 }
            };
            var exception = Assert.ThrowsAsync<RepositoryException>(async () => await _medicationService.AddMedication(add));
           
           

        }
        [Test]
        public async Task AddMedicationFailExxception()
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

            var exception = Assert.ThrowsAsync<NoMedicationFoundException>(async () => await _medicationService.BuyMedication(2,1));
            Assert.That(exception.Message, Is.EqualTo("The customer has no such medication"));


        }
            [Test]
        public async Task UpdateMedication()
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
            UpdateMedication update = new UpdateMedication()
            {
                CustomerId = 1,
                MedicationId = 1,
                MedicineId = 1,
                Quantity = 2,
                Status = "Increase"
            };
            var result = await _medicationService.UpdateMedication(update);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Medication updated successfully"));
            Assert.That(result.MedicationId, Is.GreaterThan(0));
        }
        [Test]
        public async Task UpdateMedicationNewAdd()
        {
            var item1 = new MedicationItemDTO
            {
                MedicineId = 1,
                Quantity = 3,
            };

            var add = new AddMedicationDTO
            {
                CustomerId = 1,
                MedicationName = "sample",
                medicationItems = new[] { item1 }
            };

            await _medicationService.AddMedication(add);
            UpdateMedication update = new UpdateMedication()
            {
                CustomerId = 1,
                MedicationId = 1,
                MedicineId = 2,
                Quantity = 2,
                Status = "Increase"
            };
            var result = await _medicationService.UpdateMedication(update);
            Assert.That(result.Code, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Medication updated successfully"));
            Assert.That(result.MedicationId, Is.GreaterThan(0));

            UpdateMedication update1 = new UpdateMedication()
            {
                CustomerId = 1,
                MedicationId = 1,
                MedicineId = 2,
                Quantity = 1,
                Status = "Decrease"
            };
            var result2 = await _medicationService.UpdateMedication(update1);
            Assert.That(result2.Code, Is.EqualTo(200));
            Assert.That(result2.Message, Is.EqualTo("Medication updated successfully"));
            UpdateMedication update2 = new UpdateMedication()
            {
                CustomerId = 1,
                MedicationId = 1,
                MedicineId = 2,
                Quantity = 200,
                Status = "Decrease"
            };
            var exception = Assert.ThrowsAsync<NegativeValueException>(async () => await _medicationService.UpdateMedication(update2));
            Assert.That(exception.Message, Is.EqualTo("The expected quantity is not available in the medication"));
        }
        [Test]
        public async Task AddNewfail()
        {
            var item1 = new MedicationItemDTO
            {
                MedicineId = 1,
                Quantity = 3,
            };
            var item2 = new MedicationItemDTO
            {
                MedicineId = 1,
                Quantity = 3,
            };

            var add = new AddMedicationDTO
            {
                CustomerId = 1,
                MedicationName = "sample",
                medicationItems = new[] { item1 ,item2}
            };

      
            var exception = Assert.ThrowsAsync<DuplicateValueException>(async () => await _medicationService.AddMedication(add));
            Assert.That(exception.Message, Is.EqualTo("Duplicate medicine present in the Medication"));
        }

            [Test]
        public async Task UpdateMedicineFail()
        {
            RegisterDTO register = new RegisterDTO()
            {
                Address = "121",
                Email = "abc",
                Phone = "2113",
                Role = "user",
                Name = "sam",
                Password = "1234"

            };
            await  _authService.Register(register);
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
            UpdateMedication update1 = new UpdateMedication()
            {
                CustomerId = 2,
                MedicationId = 1,
                MedicineId = 1,
                Quantity = 4,
                Status = "Decrease"
            };

            var exception = Assert.ThrowsAsync<NoMedicationFoundException>(async () => await _medicationService.UpdateMedication(update1));
            Assert.That(exception.Message, Is.EqualTo("The customer have no such medication found for medication Id 1"));



        }
        [Test]
        public async Task Checkout()
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
      
            var result = await _medicationService.BuyMedication(1,1);
            Assert.That(result.Code, Is.EqualTo(200));
          
        }
        [Test]
        public async Task CheckoutFail()
        {
            var item1 = new MedicationItemDTO
            {
                MedicineId = 1,
                Quantity = 200,
            };

            var add = new AddMedicationDTO
            {
                CustomerId = 1,
                MedicationName = "sample",
                medicationItems = new[] { item1 }
            };

            await _medicationService.AddMedication(add);


            var exception = Assert.ThrowsAsync<OutOfStockException>(async () => await _medicationService.BuyMedication(1,1));
            Assert.That(exception.Message, Is.EqualTo("Expected Quantity is not available at the moment for " + 1));
           

        }

    }


}
