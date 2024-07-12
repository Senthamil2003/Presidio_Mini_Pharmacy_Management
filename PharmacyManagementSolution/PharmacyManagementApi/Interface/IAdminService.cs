using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IAdminService
    {
        public Task<SuccessPurchaseDTO> PurchaseMedicine(PurchaseDTO items);
        public Task<OrderDetailDTO[]> GetAllOrder();
        public Task<SuccessDeliveryDTO> DeliverOrder(int  orderId);
        public Task<SuccessVendorDTO> AddVendor(VendorDTO vendordto);
        public Task<List<MedicineDTO>> GetAllMedicine();
        public Task<Category> GetCategory(int CategoryId);
        public Task<string> UpdateMedicine(UpdateMedicineDTO updateData);
        public Task<MedicineDetailDTO> GetMedicine(int Id);
        public Task<List<CategoryDTO>> GetAllCategory();
        public Task<List<BrandDTO>> GetAllBrand();
        public  Task<SuccessAddMedicine> AddMedicine(AddMedicineDTO medicineDTO);
        public Task<DashboardDTO> GetDashBoardValue();
        public Task<List<Vendor>> GetAllVendor();

    }
}
