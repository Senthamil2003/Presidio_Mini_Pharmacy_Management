using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IUserService
    {
        public Task<StockResponseDTO[]> ShowAllProduct();
    }
}
