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
    public class UserServiceTests
    {
        private PharmacyContext context;
        private StockJoinedRepository _stockJoinedRepo;
        private ITransactionService _transactionService;
        private IReposiroty<int, Medicine> _medicineRepo;
        private IReposiroty<int, Cart> _cartRepo;
        private CustomerJoinedRepository _customerRepo;
        private IReposiroty<int, Order> _orderRepo;
        private IReposiroty<int, OrderDetail> _orderDetailRepo;
        private AdminService _adminService;
        private ICartService _cartService;
        private IReposiroty<int, Stock> _stockRepo;
        private IReposiroty<int, PurchaseDetail> _purchaseDetailRepo;
        private IReposiroty<int, Category> _categoryRepo;
        private IReposiroty<int, Vendor> _vendorRepo;
        private IReposiroty<int, Purchase> _purchaseRepo;
        private IReposiroty<int, DeliveryDetail> _deliveryDetailRepo;
        private UserCredentialRepository _credentialRepo;
        private TokenService _tokenService;
        private AuthService _authService;

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

            _stockJoinedRepo = new StockJoinedRepository(context);
            _transactionService = new TransactionRepository(context);
            _medicineRepo = new MedicineRepository(context);
            _cartRepo = new CartRepository(context);
            _customerRepo = new CustomerJoinedRepository(context);
            _orderRepo = new OrderRepository(context);
            _orderDetailRepo = new OrderDetailRepository(context);
            _purchaseRepo = new PurchaseRepository(context);
            _vendorRepo = new VendorRepository(context);
            _categoryRepo = new CategoryRepository(context);
            _purchaseDetailRepo = new PurchaseDetailRepository(context);
            _stockRepo = new StockRepository(context);
            _deliveryDetailRepo = new DeliveryDetailRepository(context);
            _credentialRepo = new UserCredentialRepository(context);
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("Yes, making the Email the primary key in the UserCredential class is a valid approach, especially since emails are unique to each user and often used as the primary identifier in authentication systems.");
            Mock<IConfigurationSection> congigTokenSection = new Mock<IConfigurationSection>();
            congigTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(congigTokenSection.Object);
            _tokenService = new TokenService(mockConfig.Object);
            _authService = new AuthService(_credentialRepo, _customerRepo, _tokenService);

            _adminService = new AdminService(
                _purchaseRepo,
                _vendorRepo,
                _medicineRepo,
                _categoryRepo,
                _purchaseDetailRepo,
                _stockRepo,
                _orderDetailRepo,
                _stockJoinedRepo,
                _deliveryDetailRepo,
                _orderRepo,
                _transactionService
            );

            _cartService = new CartService(
                _stockJoinedRepo,
                _customerRepo,
                _orderRepo,
                _orderDetailRepo,
                _transactionService,
                _medicineRepo,
                _cartRepo
            );

            VendorDTO vendor = new VendorDTO()
            {
                VendorName = "PharmacyVendor1",
                Address = "ABC",
                Phone = "1212123123"
            };
            await _adminService.AddVendor(vendor);

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

            await _adminService.PurchaseMedicine(purchaseDTO);
            RegisterDTO registerDTO = new RegisterDTO()
            {
                Email = "Bbc@123",
                Name = "Tonny",
                Address = "ABC 123 kpc",
                Password = "1234567",
                Phone = "123456 7890",
                Role = "User"
            };
          await  _authService.Register(registerDTO);
        }

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
            Assert.That(exception.Message, Is.EqualTo("Expected Quantity is not avalable in the stock"));
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
            AddToCartDTO newaAdToCart = new AddToCartDTO()
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
          
           
            AddToCartDTO newaAdToCart = new AddToCartDTO()
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
            AddToCartDTO newAddToCart = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 200,
                UserId = 1
            };

            var exception = Assert.ThrowsAsync<OutOfStockException>(async () => await _cartService.UpdateCart(newAddToCart));
            Assert.That(exception.Message, Is.EqualTo("Expected Quantity is not avalable in the stock"));

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
            var result = await _cartService.RemoveFromCart(1);
            Assert.That(result.CartId, Is.EqualTo(1));
            Assert.Pass();



        }
        [Test]
        public async Task RemoveFromCartFail()
        {

            var exception = Assert.ThrowsAsync<NoCartFoundException>(async () => await _cartService.RemoveFromCart(1));
            Assert.That(exception.Message, Is.EqualTo("No Cart found with given id 1"));
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
            Assert.That(exception.Message, Is.EqualTo("Expected Quantity is not avalable at the moment for "+addToCart.MedicineId));
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
           await _cartService.RemoveFromCart(1);

            var exception = Assert.ThrowsAsync<CartEmptyException>(async () => await _cartService.Checkout(1));
            Assert.That(exception.Message, Is.EqualTo("Your Cart is Empty"));
            Assert.Pass();

        }

    }
}
