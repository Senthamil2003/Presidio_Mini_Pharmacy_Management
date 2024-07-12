using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IViewService
    {
        public Task<StockResponseDTO[]> GetMedicineByCategory(string category);
        public Task<StockResponseDTO[]> ShowAllProduct(string searchContent);
        public Task<List<MyOrderDTO>> GetAllOrders(int userId);
        public Task<List<MyMedicationDTO>> ViewMyMedications(int customerId);
        public Task<List<MyCartDTO>> ViewMyCart(int userId);
        public Task<List<BestSellerDTO>> GetBestSeller();
        public  Task<List<BestCategoryDTO>> GetBestCategory();
        public Task<ProductDTO> GetMedicine(int Id);
        public Task<MedicationItemTotalDTO> ViewMedicationItem(int customerId, int medicationId);
        public  Task<List<OnlyCartItem>> ViewMyCartOnly(int userId);
    }
}
