using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IViewService
    {
        public Task<StockResponseDTO[]> ShowAllProduct();
        public Task<List<MyOrderDTO>> GetAllOrders(int userId);
        public Task<List<AddMedicationDTO>> ViewMyMedications(int customerId);
        public Task<List<MyCartDTO>> ViewMyCart(int userId);
    }
}
