﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.General_Repositories;
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
    private readonly IRepository<int, Brand> _brandRepository;
    private readonly IRepository<int, Customer> _customer;
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
        IRepository<int, Brand> brandRepo,
        IRepository<int, Customer> customer,
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
        _brandRepository = brandRepo;
        _logger = logger;
        _customer = customer;
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
                    totalSum += item.Amount * item.Quantity;
                    Medicine medicine = (await _medicineRepo.Get(item.MedicineId));

            

                        medicine.CurrentQuantity += item.Quantity;
                    medicine.RecentPurchasePrice = item.Amount;
                 
                        await _medicineRepo.Update(medicine);


                    PurchaseDetail purchaseDetail = new PurchaseDetail()
                    {
                        Amount = item.Amount,
                        ExpiryDate = item.ExpiryDate,
                        MedicineId = item.MedicineId,
                        VendorId = item.VendorId,
                        Quantity = item.Quantity,
                        PurchaseId = purchase.PurchaseId,        
                        TotalSum = item.Amount * item.Quantity
                    };
                    await _purchaseDetailRepo.Add(purchaseDetail);

                    Stock stock = new Stock()
                    {
                        MedicineId = purchaseDetail.MedicineId,
                        ExpiryDate = purchaseDetail.ExpiryDate,    
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

            var orderDetailsDto =( await _orderDetailRepo.Get())
               
                .Select(od => new OrderDetailDTO
                {
                    Date=od.Order.OrderDate.Date,
                    MedicineName = od.Medicine.MedicineName,
                    OrderDetailId = od.OrderDetailId,
                    Quantity = od.Quantity,
                    status=od.DeliveryStatus,
                    Customerid = od.Order.CustomerId

                })
                .ToArray();

            if (!orderDetailsDto.Any())
            {
                throw new NoOrderFoundException("No order found for Admin");
            }

            _logger.LogInformation("Fetched {Count} pending orders.", orderDetailsDto.Length);
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
                VendorName = vendorDto.VendorName,
                Mail=vendorDto.Mail,
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
    public async Task<DashboardDTO> GetDashBoardValue()
    {
        try
        {
           int totalCustomer=(await _customer.Get()).Count();
            double totalPurchase =(await _purchaseRepo.Get()).Sum(e => e.TotalAmount.GetValueOrDefault());
            double totalOrder = (await _orderRepository.Get()).Sum(e => e.TotalAmount);
            int medicineCount = (await _medicineRepo.Get()).Count();
            DashboardDTO dashboardDTO = new DashboardDTO()
            {
                OrdersAmount = totalOrder,
                CustomerCount = totalCustomer,
                MedicineCount = medicineCount,
                PurchaseAmount = totalPurchase,
            };
            return dashboardDTO;


        }
        catch
        {
            throw;
        }

    }
    public async Task<List<MedicineDTO>> GetAllMedicine()
    {
        try
        {
            List<Medicine> medicine = (await _medicineRepo.Get()).ToList();
            List<MedicineDTO> medicineDTOs = new List<MedicineDTO>();
            foreach (Medicine item in medicine)
            {
                MedicineDTO medicineDTO = new MedicineDTO()
                {
                    CategoryId = item.CategoryId,
                    MedicineId = item.MedicineId,
                    MedicineName = item.MedicineName,
                    CategoryName=(await _categoryRepo.Get(item.CategoryId)).CategoryName,
                    Status=item.status,
                    CurrentQuantity=item.CurrentQuantity,
                    Brandname=(await _brandRepository.Get(item.BrandId)).BrandName,
                };
                medicineDTOs.Add(medicineDTO);
            }
            return medicineDTOs;

        }
        catch
        {
            throw;
        }
    }
    public async Task<Category> GetCategory(int CategoryId)
    {
        try
        {
            return (await _categoryRepo.Get(CategoryId));

        }
        catch
        {
            throw;

        }
    }
    public async Task<string> UpdateMedicine(UpdateMedicineDTO updateData)
    {
     
        Medicine medicine = await _medicineRepo.Get(updateData.MedicineId);

        int categoryId = (await _categoryRepo.Get()).SingleOrDefault(e => e.CategoryName == updateData.Category).CategoryId;
        if(categoryId == 0)
        {
            Category category = new Category()
            {
                CategoryName = updateData.Category,
            };
           await _categoryRepo.Add(category);
            categoryId = category.CategoryId;
        }
        medicine.MedicineName = updateData.MedicineName;
        medicine.CategoryId = categoryId;
        medicine.Description = updateData.Description;
        medicine.SellingPrice = updateData.SellingPrice;
        medicine.status =updateData.Status;

        if (updateData.File != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await updateData.File.CopyToAsync(memoryStream);
                medicine.Image = memoryStream.ToArray();
            }
        }

        await _medicineRepo.Update(medicine);
       
        return "Update successful";
    }
    public async Task<MedicineDetailDTO> GetMedicine(int Id)
    {
        try
        {
            Medicine medicine = await _medicineRepo.Get(Id);
            MedicineDetailDTO medicineDetail = new MedicineDetailDTO()
            {
                CategoryName =(await _categoryRepo.Get(medicine.CategoryId)).CategoryName,
                MedicineName = medicine.MedicineName,
                ImageBase64 = medicine.Image != null ? Convert.ToBase64String(medicine.Image) : null,
                Brand=(await _brandRepository.Get(medicine.BrandId)).BrandName,
                Description=medicine.Description,
                RecentPurchasePrice=medicine.RecentPurchasePrice,
                SellingPrice=medicine.SellingPrice,
                Status=medicine.status

            };
            return medicineDetail;

        }
        catch
        {
            throw;
        }
    }
    public async Task<SuccessAddMedicine> AddMedicine(AddMedicineDTO medicineDTO)
    {
        using (var transaction = await _transactionService.BeginTransactionAsync())
        {
            try
            {
             Medicine ceckmedicine=(await _medicineRepo.Get()).SingleOrDefault(e => e.MedicineName == medicineDTO.MedicineName);
                if (ceckmedicine != null)
                {
                    throw new DuplicateValueException("The Medicine is already exist");
                }
                int brandId = medicineDTO.BrandId;
                int CategoryId = medicineDTO.CategoryId;
                if (medicineDTO.IsnewBrand)
                {
                    Brand checkBrand = (await _brandRepository.Get()).SingleOrDefault(e => e.BrandName == medicineDTO.BrandName);
                    if (checkBrand != null)
                    {
                        throw new DuplicateValueException("The category already exist");
                    }
                    Brand brand = new Brand();
                    brand.BrandName = medicineDTO.BrandName;
                    if (medicineDTO.BrandImage != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await medicineDTO.BrandImage.CopyToAsync(memoryStream);
                            brand.Image = memoryStream.ToArray();

                        }
                    }

                    await _brandRepository.Add(brand);
                    brandId = brand.BrandId;
                }

                if (medicineDTO.IsnewCategory)
                {
                    Category checkCategory = (await _categoryRepo.Get()).SingleOrDefault(e => e.CategoryName == medicineDTO.CategoryName);
                    if (checkCategory != null)
                    {
                        throw new DuplicateValueException("The category already exist");
                    }
                    Category Category = new Category();
                    Category.CategoryName = medicineDTO.CategoryName;
                    if (medicineDTO.CategoryImage != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await medicineDTO.CategoryImage.CopyToAsync(memoryStream);
                            Category.Image = memoryStream.ToArray();

                        }
                    }

                    await _categoryRepo.Add(Category);
                    CategoryId = Category.CategoryId;

                }
                Medicine medicine = new Medicine();

                medicine.MedicineName = medicineDTO.MedicineName;
                medicine.BrandId = brandId;
                medicine.CategoryId = CategoryId;
                medicine.Description = medicineDTO.Description;
                medicine.SellingPrice = medicineDTO.SellingPrice;
                medicine.status = medicineDTO.Status;
                medicine.ItemPerPack=medicineDTO.ItemPerPack;
                medicine.Weight= medicineDTO.Weight;
                if (medicineDTO.MedicineImage != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await medicineDTO.MedicineImage.CopyToAsync(memoryStream);
                        medicine.Image = memoryStream.ToArray();

                    }
                }
                await _medicineRepo.Add(medicine);
                await _transactionService.CommitTransactionAsync();
                SuccessAddMedicine successAdd = new SuccessAddMedicine()
                {
                    Message = "Medicine Added successfully",
                    SuccessCode = 200
                };
                return successAdd;
            }

            catch
            {
                await _transactionService.RollbackTransactionAsync();
                throw;

            }
        }
       
    }
    public async Task<List<CategoryDTO>> GetAllCategory()
    {
        try
        {
          List<Category> categories= (await _categoryRepo.Get()).ToList();
            List<CategoryDTO> categoryDTOs = new List<CategoryDTO>();
            foreach (var item in categories)
            {
                CategoryDTO categoryDTO = new CategoryDTO()
                {
                    CategoryName = item.CategoryName,
                    Id = item.CategoryId
                };
                categoryDTOs.Add(categoryDTO);
                
            }
            return categoryDTOs;

        }
        catch
        {
            throw;
        }
    }
    public async Task<List<BrandDTO>> GetAllBrand()
    {
        try
        {
            List<Brand> categories = (await _brandRepository.Get()).ToList();
            List<BrandDTO> brandDTOs = new List<BrandDTO>();
            foreach (var item in categories)
            {
                BrandDTO brandDTO = new BrandDTO()
                {
                    BrandName = item.BrandName,
                    Id = item.BrandId
                };
                brandDTOs.Add(brandDTO);

            }
            return brandDTOs;

        }
        catch
        {
            throw;
        }
    }
    public async Task<List<Vendor>> GetAllVendor()
    {
        try
        {
           return (await _vendorRepo.Get()).ToList();
            
        }
        catch
        {
            throw;
        }

    }

}
