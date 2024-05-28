using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PharmacyManagementApi.Context;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
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
    public class FeedBackServiceTest
    {
        private ITransactionService _transactionService;
        private IReposiroty<int, Medicine> _medicineRepo;
        private IReposiroty<int, Feedback> _feedbackRepo;
        private IFeedbackService _feedbackService;
        private PharmacyContext context;
        private StockJoinedRepository _stockJoinedRepo;

        private IReposiroty<int, Cart> _cartRepo;
        private CustomerJoinedRepository _customerRepo;
        private IReposiroty<int, Order> _orderRepo;
        private IReposiroty<int, OrderDetail> _orderDetailRepo;
        private AdminService _adminService;
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
            _feedbackRepo=new FeedbackRepository(context);
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
                Items = new[] { purchaseItem1, purchaseItem2, purchaseItem3 }
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


            _feedbackService = new FeedBackService(_transactionService, _feedbackRepo, _medicineRepo,_customerRepo);
            FeedbackRequestDTO feedback=new FeedbackRequestDTO()
            {
                MedicineId = 1,
                CustomerId = 1,
                Feedback= "Good",
                Rating = 4

            };
           await _feedbackService.AddFeedback(feedback);

        }
        [Test]
        public async Task AddFeedback()
        {
            FeedbackRequestDTO feedback = new FeedbackRequestDTO()
            {
                MedicineId = 2,
                CustomerId = 1,
                Feedback = "Good",
                Rating = 4

            };
           var result=  await _feedbackService.AddFeedback(feedback);
            Assert.That(result.Code, Is.EqualTo(200));

        }
        [Test]
        public async Task AddFeedbackDuplicateFail()
        {
            FeedbackRequestDTO feedback = new FeedbackRequestDTO()
            {
                MedicineId = 1,
                CustomerId = 1,
                Feedback = "Good",
                Rating = 4

            };
            var exception = Assert.ThrowsAsync<DuplicateValueException>(async () => await _feedbackService.AddFeedback(feedback));
           
            Assert.Pass();

        }
        [Test]
        public async Task ViewMyFeedback()
        {

           var result=await _feedbackService.ViewMyFeedBack(1);
           Assert.That(result.Count, Is.EqualTo(1));
            Assert.Pass();

        }
        [Test]
        public async Task ViewMyFeedbackNocustomerFail()
        {

            var exception = Assert.ThrowsAsync<NoCustomerFoundException>(async () => await _feedbackService.ViewMyFeedBack(5));
         
            Assert.Pass();
            Assert.Pass();

        }
        [Test]
        public async Task ViewMyFeedbackNoFeedbackFail()
        {
            RegisterDTO registerDTO = new RegisterDTO()
            {
                Email = "Bbb@123",
                Name = "Tonny",
                Address = "ABC 123 kpc",
                Password = "1234567",
                Phone = "123456 7890",
                Role = "User"
            };
            await _authService.Register(registerDTO);

            var exception = Assert.ThrowsAsync<NoFeedbackFoundException>(async () => await _feedbackService.ViewMyFeedBack(2));

            Assert.Pass();
            Assert.Pass();

        }
        [Test]
        public async Task MedicineFeedback()
        {
            MedicineFeedbackDTO result =await _feedbackService.MedicineFeedback(1);

            Assert.That(result.FeedbackRating, Is.EqualTo(4));

            Assert.Pass();
            Assert.Pass();

        }
        [Test]
        public async Task ViewFeedbackFail()
        {

            var exception = Assert.ThrowsAsync<NoFeedbackFoundException>(async () => await _feedbackService.MedicineFeedback(2));
            Assert.Pass();
            Assert.Pass();

        }

    }
}
