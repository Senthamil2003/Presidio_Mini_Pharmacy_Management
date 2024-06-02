using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;

public class AdminService : IAdminService
{
    private readonly IRepository<int, Purchase> _purchaseRepo;
    private readonly IRepository<int, Vendor> _vendorRepo;
    private readonly IRepository<int, Medicine> _medicineRepo;
    private readonly IRepository<int, Category> _categoryRepo;
    private readonly IRepository<int, PurchaseDetail> _purchaseDetailRepo;
    private readonly IRepository<int, Stock> _stockRepository;
    private readonly ITransactionService _transactionService;
    private readonly IRepository<int, OrderDetail> _orderDetailRepo;
    private readonly StockJoinedRepository _stockJoinedRepo;
    private readonly IRepository<int, DeliveryDetail> _deliveryDetailRepo;
    private readonly IRepository<int, Order> _orderRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IRepository<int, Purchase> purchaseRepo,
        IRepository<int, Vendor> vendorRepo,
        IRepository<int, Medicine> medicineRepo,
        IRepository<int, Category> categoryRepo,
        IRepository<int, PurchaseDetail> purchaseDetailRepo,
        IRepository<int, Stock> stockRepository,
        IRepository<int, OrderDetail> orderDetailRepo,
        StockJoinedRepository stockJoinedRepo,
        IRepository<int, DeliveryDetail> deliveryDetailRepo,
        IRepository<int, Order> orderRepository,
        ITransactionService transactionService,
        ILogger<AdminService> logger
        )
    {
        _purchaseRepo = purchaseRepo;
        _vendorRepo = vendorRepo;
        _medicineRepo = medicineRepo;
        _categoryRepo = categoryRepo;
        _purchaseDetailRepo = purchaseDetailRepo;
        _stockRepository = stockRepository;
        _transactionService = transactionService;
        _orderDetailRepo = orderDetailRepo;
        _stockJoinedRepo = stockJoinedRepo;
        _deliveryDetailRepo = deliveryDetailRepo;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the purchase of medicines, adding new medicines and categories as needed,
    /// and updates the stock and purchase details.
    /// </summary>
    /// <param name="items">Details of the purchase.</param>
    /// <returns>Details of the successful purchase.</returns>
    public async Task<SuccessPurchaseDTO> PurchaseMedicine(PurchaseDTO items)
    {
        using (var transaction = await _transactionService.BeginTransactionAsync())
        {
            try
            {
                _logger.LogInformation("Starting PurchaseMedicine transaction.");

                Purchase purchase = new Purchase() { PurchaseDate = items.DateTime };
                await _purchaseRepo.Add(purchase);
                double totalSum = 0;

                foreach (PurchaseItem item in items.Items)
                {
                    totalSum += item.Amount*item.Quantity;
                    Medicine? medicine = (await _medicineRepo.Get()).SingleOrDefault(m => m.MedicineName == item.MedicineName);
                    int isNewMedicine = 0;

                    if (medicine == null)
                    {
                        isNewMedicine = 1;
                        Category? category = (await _categoryRepo.Get()).SingleOrDefault(c => c.CategoryName == item.MedicineCategory);
                        if (category == null)
                        {
                            category = new Category() { CategoryName = item.MedicineCategory };
                            await _categoryRepo.Add(category);
                        }

                        medicine = new Medicine()
                        {
                            MedicineName = item.MedicineName,
                            CategoryId = category.CategoryId,
                            CurrentQuantity = item.Quantity,
                            SellingPrice = item.Amount
                        };
                        await _medicineRepo.Add(medicine);
                    }

                    Category? checkCategory = (await _categoryRepo.Get()).SingleOrDefault(c => c.CategoryName == item.MedicineCategory)
                        ?? throw new CategoryMedicineMisMatchException("Given Category is not matched with the Medicine");

                    Vendor vendor = (await _vendorRepo.Get()).SingleOrDefault(v => v.VendorName == item.VendorName)
                        ?? throw new NoVendorFoundException("No vendor found, add the vendor");

                    if (isNewMedicine == 0)
                    {
                        medicine.CurrentQuantity += item.Quantity;
                        await _medicineRepo.Update(medicine);
                    }

                    PurchaseDetail purchaseDetail = new PurchaseDetail()
                    {
                        Amount = item.Amount,
                        DosageForm = item.DosageForm,
                        ExpiryDate = item.ExpiryDate,
                        MedicineId = medicine.MedicineId,
                        VendorId = vendor.VendorId,
                        Quantity = item.Quantity,
                        PurchaseId = purchase.PurchaseId,
                        StorageRequirement = item.StorageRequirement,
                        TotalSum = item.Amount * item.Quantity
                    };
                    await _purchaseDetailRepo.Add(purchaseDetail);

                    Stock stock = new Stock()
                    {
                        MedicineId = purchaseDetail.MedicineId,
                        ExpiryDate = purchaseDetail.ExpiryDate,
                        SellingPrice = item.Amount,
                        PurchaseDetailId = purchaseDetail.PurchaseDetailId,
                        Quantity = purchaseDetail.Quantity
                    };

                    await _stockRepository.Add(stock);
                }

                purchase.TotalAmount = totalSum;
                await _purchaseRepo.Update(purchase);

                await _transactionService.CommitTransactionAsync();

                _logger.LogInformation("PurchaseMedicine transaction completed successfully.");

                return new SuccessPurchaseDTO()
                {
                    Code = 200,
                    Message = "OK",
                    PurchaseId = purchase.PurchaseId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in PurchaseMedicine, rolling back transaction.");
                await _transactionService.RollbackTransactionAsync();
                throw;
            }
        }
    }

    /// <summary>
    /// Retrieves all pending order details for the admin.
    /// </summary>
    /// <returns>An array of order details.</returns>
    public async Task<OrderDetailDTO[]> GetAllOrder()
    {
        try
        {
            _logger.LogInformation("Fetching all pending orders.");
            List<OrderDetail> orderDetails = (await _orderDetailRepo.Get()).Where(od => !od.DeliveryStatus).ToList();

            if (orderDetails.Count == 0)
            {
                throw new NoOrderFoundException("No order found for Admin");
            }

            OrderDetailDTO[] orderDetailsDto = orderDetails.Select(orderDetail => new OrderDetailDTO()
            {
                MedicineName = orderDetail.Medicine.MedicineName,
                OrderDetailId = orderDetail.OrderDetailId,
                Quantity = orderDetail.Quantity,
                Customerid=orderDetail.Order.CustomerId
            }).ToArray();

            _logger.LogInformation("Fetched {Count} pending orders.", orderDetails.Count);

            return orderDetailsDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching orders.");
            throw;
        }
    }

    /// <summary>
    /// Processes the delivery of an order.
    /// </summary>
    /// <param name="orderDetailId">The ID of the order detail to deliver.</param>
    /// <returns>Details of the successful delivery.</returns>
    public async Task<SuccessDeliveryDTO> DeliverOrder(int orderDetailId)
    {
        using (var transaction = await _transactionService.BeginTransactionAsync())
        {
            try
            {
                _logger.LogInformation("Starting delivery for OrderDetailId: {OrderDetailId}.", orderDetailId);

                OrderDetail orderDetail = await _orderDetailRepo.Get(orderDetailId);
                int userId = (await _orderRepository.Get(orderDetail.OrderId)).CustomerId;
                var stockItems = (await _stockRepository.Get())
                    .Where(s => s.MedicineId == orderDetail.MedicineId)
                    .OrderBy(s => s.ExpiryDate)
                    .ToList();

                int remainingQuantity = orderDetail.Quantity;
                int index = 0;

                while (remainingQuantity > 0 && index < stockItems.Count)
                {
                    Stock currentStock = await _stockRepository.Get(stockItems[index].StockId);
                    int quantityToDeliver;

                    if (currentStock.Quantity > remainingQuantity)
                    {
                        quantityToDeliver = remainingQuantity;
                        currentStock.Quantity -= remainingQuantity;
                        remainingQuantity = 0;
                        await _stockRepository.Update(currentStock);
                    }
                    else
                    {
                        quantityToDeliver = currentStock.Quantity;
                        remainingQuantity -= currentStock.Quantity;
                        await _stockRepository.Delete(currentStock.StockId);
                    }

                    DeliveryDetail delivery = new DeliveryDetail()
                    {
                        CustomerId = userId,
                        ExpiryDate = currentStock.ExpiryDate,
                        MedicineId = orderDetail.MedicineId,
                        OrderDetailId = orderDetail.OrderDetailId,
                        Quantity = quantityToDeliver
                    };
                    await _deliveryDetailRepo.Add(delivery);

                    index++;
                }

                orderDetail.DeliveryStatus = true;
                await _orderDetailRepo.Update(orderDetail);

                await _transactionService.CommitTransactionAsync();

                _logger.LogInformation("Delivery for OrderDetailId: {OrderDetailId} completed successfully.", orderDetailId);

                return new SuccessDeliveryDTO()
                {
                    Code = 200,
                    Message = "Delivery successful",
                    OrderDetailId = orderDetail.OrderDetailId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in DeliverOrder, rolling back transaction.");
                await _transactionService.RollbackTransactionAsync();
                throw;
            }
        }
    }


    /// <summary>
    /// Adds a new vendor to the system.
    /// </summary>
    /// <param name="vendorDto">Details of the vendor to add.</param>
    /// <returns>A success message.</returns>
    public async Task<SuccessVendorDTO> AddVendor(VendorDTO vendorDto)
    {
        try
        {
            _logger.LogInformation("Adding a new vendor: {VendorName}.", vendorDto.VendorName);

            Vendor vendor = new Vendor()
            {
                Phone = vendorDto.Phone,
                Address = vendorDto.Address,
                VendorName = vendorDto.VendorName
            };

            await _vendorRepo.Add(vendor);
            _logger.LogInformation("Vendor {VendorName} added successfully.", vendorDto.VendorName);
            SuccessVendorDTO successVendor = new SuccessVendorDTO()
            {
                Code = 200,
                Message = "Vendor Added Successfully",
                VendorId = vendor.VendorId,
            };
            return successVendor;

          
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding vendor.");
            throw;
        }
    }


}
