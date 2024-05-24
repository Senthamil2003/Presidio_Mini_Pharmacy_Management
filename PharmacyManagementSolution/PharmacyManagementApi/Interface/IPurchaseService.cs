using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Interface
{
    public interface IPurchaseService
    {
        public Task<SuccessPurchaseDTO> PurchaseMedicine(PurchaseDTO items);
    }
}
