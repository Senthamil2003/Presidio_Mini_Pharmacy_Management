using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using System.Diagnostics.CodeAnalysis;

public class AdminService : IAdminService
{
    private readonly IReposiroty<int, Purchase> _purchaseRepo;
    private readonly IReposiroty<int, Vendor> _vendorRepo;
    private readonly IReposiroty<int, Medicine> _medicineRepo;
    private readonly IReposiroty<int, Category> _categoryRepo;
    private readonly IReposiroty<int, PurchaseDetail> _purchaseDetailRepo;
    private readonly IReposiroty<int, Stock> _stockRepository;
    private readonly ITransactionService _transactionService;
    private readonly IReposiroty<int, OrderDetail> _orderDetailRepo;
    private readonly StockJoinedRepository _stockJoinedRepo;
    private readonly IReposiroty<int, DeliveryDetail> _deliveryDetailRepo;
    private readonly IReposiroty<int, Order> _orderRepository;

    public AdminService(
        IReposiroty<int, Purchase> purchaseRepo,
        IReposiroty<int, Vendor> vendorRepo,
        IReposiroty<int, Medicine> medicineRepo,
        IReposiroty<int, Category> categoryRepo,
        IReposiroty<int, PurchaseDetail> purchaseDetailRepo,
        IReposiroty<int, Stock> stockRepository,
        IReposiroty<int ,OrderDetail> orderDetailRepo,
        StockJoinedRepository stockJoinedRepo,
        IReposiroty<int, DeliveryDetail> deliveryDetailRepo,
        IReposiroty<int ,Order> orderRepository,
        ITransactionService transactionService
        )
    {
        _purchaseRepo = purchaseRepo;
        _vendorRepo = vendorRepo;
        _medicineRepo = medicineRepo;
        _categoryRepo = categoryRepo;
        _purchaseDetailRepo = purchaseDetailRepo;
        _stockRepository = stockRepository;
        _transactionService= transactionService;
        _orderDetailRepo= orderDetailRepo;
        _stockJoinedRepo = stockJoinedRepo;
        _deliveryDetailRepo= deliveryDetailRepo;
        _orderRepository=orderRepository;
    }


    public async Task<SuccessPurchaseDTO> PurchaseMedicine(PurchaseDTO items)
    {
        using (var transaction = await _transactionService.BeginTransactionAsync())
        {
            try
            {
                Purchase purchase = new Purchase()
                {
                    PurchaseDate = items.DateTime,
                };

                await _purchaseRepo.Add(purchase);
                double totalSum = 0;
                foreach (PurchaseItem item in items.Items)
                {
                    totalSum += item.Amount;

                    Medicine? medicine = (await _medicineRepo.Get()).SingleOrDefault(m => m.MedicineName == item.MedicineName);
                    int f = 0;
                    if (medicine == null)
                    {
                        f = 1;
                        Category? category = (await _categoryRepo.Get()).SingleOrDefault(c => c.CategoryName == item.MedicineCategory);
                        if (category == null)
                        {
                            category = new Category()
                            {
                                CategoryName = item.MedicineCategory
                            };
                            await _categoryRepo.Add(category);
                        }
                        medicine = new Medicine()
                        {
                            MedicineName = item.MedicineName,
                            CategoryId = category.CategoryId,
                            CurrentQuantity=item.Quantity,
                            SellingPrice=item.Amount
                            
                        };
                        await _medicineRepo.Add(medicine);
                    }
                    Category? Checkcategory = (await _categoryRepo.Get()).SingleOrDefault(c => c.CategoryName == item.MedicineCategory)?? throw new CategoryMedicineMisMatchException("Given Category is not matched with the Medicine");
                    Vendor vendor = (await _vendorRepo.Get()).SingleOrDefault(v => v.VendorName == item.VendorName)
    ?? throw new NoVendorFoundException("No vendor found, add the vendor");
                    int vendorId = vendor.VendorId;
                    if (f == 0)
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
 
                SuccessPurchaseDTO purchaseDTO = new SuccessPurchaseDTO()
                {
                    Code = 200,
                    Message = "OK",
                    PurchaseId = purchase.PurchaseId
                };
                return purchaseDTO;
            }
            catch
            {
                await _transactionService.RollbackTransactionAsync();
                throw;
            }
        }       
    }
    public  async Task<OrderDetailDTO[]> GetAllOrder()
    {
        try
        {
           List<OrderDetail> orderDetails= (await _orderDetailRepo.Get()).Where(od=>!od.DeliveryStatus).ToList();
           OrderDetailDTO[] orderDetaills=new OrderDetailDTO[orderDetails.Count];
            if (orderDetails.Count == 0)
            {
                throw new NoOrderFoundException("No order Found for Admin");
            }
            int ct = 0;
           foreach (var orderDetail in orderDetails)
            {
                OrderDetailDTO Detail = new OrderDetailDTO()
                {
                    MedicineName = orderDetail.Medicine.MedicineName,
                    OrderDetailId = orderDetail.OrderDetailId,
                    Quantity = orderDetail.Quantity
                };
                orderDetaills[ct] = Detail;
                ct++;

            }
           return orderDetaills;
            
        }
        catch
        {
            throw;

        }

    }
  
    public async Task<SuccessDeliveryDTO> DeliverOrder(int orderDetailId)
    {
        using (var transaction = await _transactionService.BeginTransactionAsync())
        {
            try
            {
                 OrderDetail orderDetail= await _orderDetailRepo.Get(orderDetailId);
               
                int userId = (await _orderRepository.Get(orderDetail.OrderId)).CustomerId;
                var result = (await _stockRepository.Get()).Where(s => s.MedicineId == orderDetail.MedicineId).OrderBy(s => s.ExpiryDate).ToList();
                int orderQuantity = orderDetail.Quantity;
                int ct = 0;
                int Quantity;
                while (orderQuantity > 0)
                {
                    Stock updateStock = await _stockRepository.Get(result[ct].StockId);
                    if (result[ct].Quantity > orderQuantity)
                    {
                        Quantity = orderQuantity;
                        updateStock.Quantity -= orderQuantity;
                        orderQuantity = 0;
                        await _stockRepository.Update(updateStock);
                    }
                    else
                    {
                        Quantity = result[ct].Quantity;
                        orderQuantity -= result[ct].Quantity; 
                        await _stockRepository.Delete(updateStock.StockId);

                    }
                    orderDetail.DeliveryStatus = true;
                    await _orderDetailRepo.Update(orderDetail);
                    DeliveryDetail delivery = new DeliveryDetail()
                    {
                        CustomerId = userId,
                        ExpiryDate = updateStock.ExpiryDate,
                        MedicineId = orderDetail.MedicineId,
                        OrderDetailId = orderDetail.OrderDetailId,
                        Quantity = Quantity
                    };
                    await _deliveryDetailRepo.Add(delivery);
                    ct++;
                }

               await _transactionService.CommitTransactionAsync();
                SuccessDeliveryDTO deliveryDTO = new SuccessDeliveryDTO()
                {
                    Code = 200,
                    Message = "Delivery successful",
                    OrderDetailId = orderDetail.OrderDetailId,
                };
               return deliveryDTO;
            }
            catch
            {
                await _transactionService.RollbackTransactionAsync();
                throw;
            }
        }

    }
    public async Task<string> AddVendor(VendorDTO vendordto)
    {
        try
        {

            Vendor vendor = new Vendor()
            {
                Phone = vendordto.Phone,
                Address = vendordto.Address,
                VendorName = vendordto.VendorName
            };

            await _vendorRepo.Add(vendor);
            return "success";

        }
        catch
        {
            throw;
        }
      
    }
}
