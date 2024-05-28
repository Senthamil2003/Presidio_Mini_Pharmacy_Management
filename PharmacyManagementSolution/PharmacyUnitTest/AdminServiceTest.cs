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
    public class Tests
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
                Quantity = 40,
                ExpiryDate = new DateTime(2024, 12, 31),
                StorageRequirement = "Room Temperature",
                DosageForm = "Tablet"
            };
            var purchaseItem3 = new PurchaseItem
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
                Items = new[] { purchaseItem1, purchaseItem2 ,purchaseItem3}
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
            await _authService.Register(registerDTO);
           

        }

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
