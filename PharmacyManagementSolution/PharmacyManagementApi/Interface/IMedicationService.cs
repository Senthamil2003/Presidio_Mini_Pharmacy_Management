using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IMedicationService
    {
        public Task<SuccessMedicationDTO> AddMedication(AddMedicationDTO addMedication);
        public  Task<SuccessMedicationDTO> UpdateMedication(UpdateMedication addMedication);
        public  Task<SuccessCheckoutDTO> BuyMedication(int userId, int medicationId);
    }   
}
