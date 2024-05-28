using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IViewService
    {
        public Task<StockResponseDTO[]> ShowAllProduct();
        public Task<List<MyOrderDTO>> GetAllOrders(int userId);
    }
}
