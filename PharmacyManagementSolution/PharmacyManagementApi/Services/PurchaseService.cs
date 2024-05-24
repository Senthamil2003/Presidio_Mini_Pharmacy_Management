using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;

namespace PharmacyManagementApi.Services
{
    public class PurchaseService
    {
        private readonly IReposiroty<int, Purchase> _purchaseRepo;
        private readonly IReposiroty<int, Vendor> _vendorRepo;
        private readonly IReposiroty<int, Medicine> _medicineRepo;
        private readonly IReposiroty<int, Category> _categoryRepo;
        private readonly IReposiroty<int, PurchaseDetail> _purchaseDetailRepo;
        private readonly IReposiroty<int, Stock> _stockRepository;

        public PurchaseService(
            IReposiroty<int, Purchase> purchaseRepo,
            IReposiroty<int, Vendor> vendorRepo,
            IReposiroty<int, Medicine> medicineRepo,
            IReposiroty<int, Category> categoryRepo,
            IReposiroty<int, PurchaseDetail> purchaseDetailRepo,
            IReposiroty<int, Stock> stockRepository)
        {
            _purchaseRepo = purchaseRepo;
            _vendorRepo = vendorRepo;
            _medicineRepo = medicineRepo;
            _categoryRepo = categoryRepo;
            _purchaseDetailRepo = purchaseDetailRepo;
            _stockRepository = stockRepository;
        }
        public Task<Purchase> PurchaseMedicine(PurchaseDTO[] purchases)
        {
            try
            {
                foreach (PurchaseDTO purchase in purchases)
                {

                }

            }
            catch 
            {
                throw;
            }

        }
    }
}
