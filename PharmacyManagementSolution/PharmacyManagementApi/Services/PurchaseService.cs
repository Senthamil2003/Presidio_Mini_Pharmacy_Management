using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models;

public class PurchaseService : IPurchaseService
{
    private readonly IReposiroty<int, Purchase> _purchaseRepo;
    private readonly IReposiroty<int, Vendor> _vendorRepo;
    private readonly IReposiroty<int, Medicine> _medicineRepo;
    private readonly IReposiroty<int, Category> _categoryRepo;
    private readonly IReposiroty<int, PurchaseDetail> _purchaseDetailRepo;
    private readonly IReposiroty<int, Stock> _stockRepository;
    private readonly ITransactionService _transactionService;

    public PurchaseService(
        IReposiroty<int, Purchase> purchaseRepo,
        IReposiroty<int, Vendor> vendorRepo,
        IReposiroty<int, Medicine> medicineRepo,
        IReposiroty<int, Category> categoryRepo,
        IReposiroty<int, PurchaseDetail> purchaseDetailRepo,
        IReposiroty<int, Stock> stockRepository,
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
                    if (medicine == null)
                    {
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
                            CategoryId = category.CategoryId
                        };
                        await _medicineRepo.Add(medicine);
                    }
                    Category? Checkcategory = (await _categoryRepo.Get()).SingleOrDefault(c => c.CategoryName == item.MedicineCategory)?? throw new CategoryMedicineMisMatchException("Given Category is not matched with the Medicine");
                    Vendor vendor = (await _vendorRepo.Get()).SingleOrDefault(v => v.VendorName == item.VendorName)
    ?? throw new NoVendorFoundException("No vendor found, add the vendor");
                    int vendorId = vendor.VendorId;
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
}
