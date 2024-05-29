using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
using PharmacyManagementApi.Context;
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
    public class BaseSetup
    {
        public PharmacyContext context;
        public StockJoinedRepository _stockJoinedRepo;
        public ITransactionService _transactionService;
        public IRepository<int, Medicine> _medicineRepo;
        public IRepository<int, Cart> _cartRepo;
        public CustomerJoinedRepository _customerRepo;
        public IRepository<int, Order> _orderRepo;
        public IRepository<int, OrderDetail> _orderDetailRepo;
        public AdminService _adminService;
        public IViewService _viewService;
        public ICartService _cartService;
        public IRepository<int, Stock> _stockRepo;
        public IRepository<int, PurchaseDetail> _purchaseDetailRepo;
        public IRepository<int, Category> _categoryRepo;
        public IRepository<int, Vendor> _vendorRepo;
        public IRepository<int, Purchase> _purchaseRepo;
        public IRepository<int, Medication> _medicationRepo;
        public IRepository<int, MedicationItem> _medicationItemRepo;
        public IRepository<int, DeliveryDetail> _deliveryDetailRepo;
        public IRepository<string, UserCredential> _credentialRepo;
        public IRepository<int, Feedback> _feedbackRepo;
        public IFeedbackService _feedbackService;
        public TokenService _tokenService;
        public AuthService _authService;
        public IMedicationService _medicationService;
        public ILogger<AdminService> _adminLogger;
        public ILogger<AuthService> _authLogger;
        public ILogger<MedicationService> _medicationLogger;
        public ILogger<FeedBackService> _feedbackLogger;
        public ILogger<CartService> _cartLogger;
        public ILogger<ViewService> _viewLogger;
        public ILogger<TokenService> _tokenLogger;

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

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();

            });
            _adminLogger = loggerFactory.CreateLogger<AdminService>();
            _authLogger = loggerFactory.CreateLogger<AuthService>();
            _medicationLogger = loggerFactory.CreateLogger<MedicationService>();
            _feedbackLogger = loggerFactory.CreateLogger<FeedBackService>();
            _cartLogger = loggerFactory.CreateLogger<CartService>();
            _viewLogger = loggerFactory.CreateLogger<ViewService>();
            _tokenLogger= loggerFactory.CreateLogger<TokenService>();

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
            _medicationRepo = new MedicationRepository(context);
            _medicationItemRepo = new MedicationItemRepository(context);
            _feedbackRepo = new FeedbackRepository(context);    
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("Yes, making the Email the primary key in the UserCredential class is a valid approach, especially since emails are unique to each user and often used as the primary identifier in authentication systems.");
            Mock<IConfigurationSection> congigTokenSection = new Mock<IConfigurationSection>();
            congigTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(congigTokenSection.Object);
            _tokenService = new TokenService(mockConfig.Object,_tokenLogger);
            _authService = new AuthService(_credentialRepo, _customerRepo, _tokenService,_authLogger);

            


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
                _transactionService,
                _adminLogger
            );

            _cartService = new CartService(
                _stockJoinedRepo,
                _customerRepo,
                _orderRepo,
                _orderDetailRepo,
                _transactionService,
                _medicineRepo,
                _cartRepo,
                _cartLogger
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
            await _authService.Register(registerDTO);
            _medicationService = new MedicationService(_transactionService,
                 _customerRepo,
                _medicationRepo,
                _medicationItemRepo,
                _medicineRepo,
                _orderRepo,
                _orderDetailRepo,
                _medicationLogger
                );
            _feedbackService = new FeedBackService(_transactionService, _feedbackRepo, _medicineRepo, _customerRepo,_feedbackLogger);
            FeedbackRequestDTO feedback = new FeedbackRequestDTO()
            {
                MedicineId = 1,
                CustomerId = 1,
                Feedback = "Good",
                Rating = 4

            };
            await _feedbackService.AddFeedback(feedback);
            _viewService = new ViewService(_stockJoinedRepo, _customerRepo,_viewLogger);

        }
        [TearDown]
        public void TearDown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

    }
}
