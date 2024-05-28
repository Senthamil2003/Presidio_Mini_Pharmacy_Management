using Microsoft.EntityFrameworkCore;
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
using System.Numerics;
using System.Threading.Tasks;

namespace PharmacyUnitTest
{
    public class Tests
    {
        private PharmacyContext context;
        private IReposiroty<int, Purchase> _purchaseRepo;
        private IReposiroty<int, Vendor> _vendorRepo;
        private IReposiroty<int, Medicine> _medicineRepo;
        private IReposiroty<int, Category> _categoryRepo;
        private IReposiroty<int, PurchaseDetail> _purchaseDetailRepo;
        private IReposiroty<int, Stock> _stockRepo;
        private ITransactionService _transactionService;
        private IReposiroty<int, OrderDetail> _orderDetailRepo;
        private StockJoinedRepository _stockJoinedRepo;
        private IReposiroty<int, DeliveryDetail> _deliveryDetailRepo;
        private IReposiroty<int, Order> _orderRepository;
        private IAdminService _adminService;
        private IUserService _userService;
        private IReposiroty<int, Cart> _cartRepo;
        private IReposiroty<int, Order> _orderRepo;
        private CustomerJoinedRepository _customerRepo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PharmacyContext>()
                            .UseSqlite("DataSource=:memory:")
                            .Options;

            context = new PharmacyContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureDeleted(); // Ensure database is deleted
            context.Database.EnsureCreated(); // Ensure the schema is created

            _purchaseRepo = new PurchaseRepository(context);
            _purchaseDetailRepo = new PurchaseDetailRepository(context);
            _deliveryDetailRepo = new DeliveryDetailRepository(context);
            _stockRepo = new StockRepository(context);
            _medicineRepo = new MedicineRepository(context);
            _categoryRepo = new CategoryRepository(context);
            _orderRepository = new OrderRepository(context);
            _orderDetailRepo = new OrderDetailRepository(context);
            _vendorRepo = new VendorRepository(context);
            _transactionService = new TransactionRepository(context);
            _stockJoinedRepo = new StockJoinedRepository(context);

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
                _orderRepository,
                _transactionService
            );
            _userService = new UserService(
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
            _adminService.AddVendor(vendor);

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
        public async Task GetAllOrderTest()
        {
           

        }

    }


}
