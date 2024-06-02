using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Repositories.General_Repositories;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using PharmacyManagementApi.Services;
using System;
using System.Threading.Tasks;

namespace PharmacyUnitTest
{
    [TestFixture]
    public class UserServiceTests:BaseSetup
    {
   
        [Test]
        public async Task AddToCart()
        {
            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 2,
                UserId = 1
            };
            var result = await _cartService.AddToCart(addToCart);
            Assert.That(result.Code, Is.EqualTo(200));
        }
        [Test]
        public async Task AddToCartFail()
        {
            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 200,
                UserId = 1
            };
            var exception = Assert.ThrowsAsync<OutOfStockException>(async () => await _cartService.AddToCart(addToCart));
            Assert.That(exception.Message, Is.EqualTo("Expected Quantity is not available in the stock"));
        }
        [Test]
        public async Task AddToCartDuplicateFail()
        {
            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 2,
                UserId = 1
            };
           await _cartService.AddToCart(addToCart);
            AddToCartDTO newaAdToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 3,
                UserId = 1
            };

            var exception = Assert.ThrowsAsync<DuplicateValueException>(async () => await _cartService.AddToCart(newaAdToCart));
            Assert.That(exception.Message, Is.EqualTo("There is already a cart with medicine Id try updateCart1"));
        }
        [Test]
        public async Task UpdateCart()
        {
            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 2,
                UserId = 1
            };
            await _cartService.AddToCart(addToCart);
            UpdateCartDTO newaAdToCart = new UpdateCartDTO()
            {
                MedicineId = 1,
                Quantity = 3,
                UserId = 1
            };
            var result = await _cartService.UpdateCart(newaAdToCart);
            Assert.That(result.CartId,Is.EqualTo(1));
            Assert.Pass();

          
        }
        [Test]
        public async Task UpdateCartFail()
        {


            UpdateCartDTO newaAdToCart = new UpdateCartDTO()
            {
                MedicineId = 1,
                Quantity = 3,
                UserId = 1
            };

            var exception = Assert.ThrowsAsync<NoCartFoundException>(async () => await _cartService.UpdateCart(newaAdToCart));
            Assert.That(exception.Message, Is.EqualTo("No cart Contains the Medicine Id , Add the cart first"));


        }
        [Test]
        public async Task UpdateCartQuantityFail()
        {


            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 3,
                UserId = 1
            };
           await _cartService.AddToCart(addToCart);
            UpdateCartDTO newAddToCart = new UpdateCartDTO()
            {
                MedicineId = 1,
                Quantity = 200,
                UserId = 1
            };

            var exception = Assert.ThrowsAsync<OutOfStockException>(async () => await _cartService.UpdateCart(newAddToCart));
            Assert.That(exception.Message, Is.EqualTo("Expected Quantity is not available in the stock"));

        }
        [Test]
        public async Task RemoveFromCart()
        {


            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 3,
                UserId = 1
            };
            await _cartService.AddToCart(addToCart);
            var result = await _cartService.RemoveFromCart(1,1);
            Assert.That(result.CartId, Is.EqualTo(1));
            Assert.Pass();



        }
        [Test]
        public async Task RemoveFromCartFail()
        {
            

            var exception = Assert.ThrowsAsync<NoCartFoundException>(async () => await _cartService.RemoveFromCart(1,1));
            Assert.That(exception.Message, Is.EqualTo("No Cart found with given id 1"));
        

            Assert.Pass();

        }
        [Test]
        public async Task RemoveFromCartFailCustomer()
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
            await _authService.Register(register);
            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 2,
                UserId = 1
            };
            await _cartService.AddToCart(addToCart);

            var exception1 = Assert.ThrowsAsync<NoCartFoundException>(async () => await _cartService.RemoveFromCart(1, 2));
            Assert.That(exception1.Message, Is.EqualTo("The user have no such cart"));



            Assert.Pass();

        }
        [Test]
        public async Task CartCheckout()
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
            var result = await _cartService.Checkout(1);
            Assert.That(result.OrderId, Is.EqualTo(1));
            Assert.Pass();

        }
        [Test]
        public async Task CartCheckoutFailWithQuantity()
        {
            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 3,
                UserId = 1
            };
            await _cartService.AddToCart(addToCart);
            Medicine medicine =await _medicineRepo.Get(1);
            medicine.CurrentQuantity = 0;
            await _medicineRepo.Update(medicine); 

            var exception = Assert.ThrowsAsync<OutOfStockException>(async () => await _cartService.Checkout(1));
            Assert.That(exception.Message, Is.EqualTo("Expected Quantity is not available at the moment for "+addToCart.MedicineId));
            Assert.Pass();

        }
        [Test]
        public async Task CartCheckoutFailWithNoCart()
        {
            AddToCartDTO addToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 3,
                UserId = 1
            };
            await _cartService.AddToCart(addToCart);
           await _cartService.RemoveFromCart(1, 1);

            var exception = Assert.ThrowsAsync<CartEmptyException>(async () => await _cartService.Checkout(1));
            Assert.That(exception.Message, Is.EqualTo("Your Cart is Empty"));
            Assert.Pass();

        }

    }
}
