using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IMedicationService
    {
        public Task<SuccessMedicationDTO> AddMedication(AddMedicationDTO addMedication);
        public  Task<SuccessMedicationDTO> UpdateMedication(UpdateMedication addMedication);
        public  Task<SuccessCheckoutDTO> BuyMedication(int userId, int medicationId);
        public Task<SuccessMedicationDTO> AddMedicationItem(AddMedicationItemDTO updateMedication);
        public Task<SuccessRemoveDTO> RemoveMedication(int customerId, int MedicationId);
        public Task<SuccessRemoveDTO> RemoveMedicationItem(int medicationId, int customerId, int MedicationItemId);
    }   
}
