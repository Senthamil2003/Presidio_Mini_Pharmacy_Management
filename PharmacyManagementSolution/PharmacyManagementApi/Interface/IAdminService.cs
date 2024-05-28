using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IAdminService
    {
        public Task<SuccessPurchaseDTO> PurchaseMedicine(PurchaseDTO items);
        public Task<OrderDetailDTO[]> GetAllOrder();
        public Task<SuccessDeliveryDTO> DeliverOrder(int  orderId);
        public Task<string> AddVendor(VendorDTO vendordto);

    }
}
