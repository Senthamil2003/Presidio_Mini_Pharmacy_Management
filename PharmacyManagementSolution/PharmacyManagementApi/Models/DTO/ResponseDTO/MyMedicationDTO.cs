using PharmacyManagementApi.Models.DTO.RequestDTO;

namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class MyMedicationDTO
    {
        public int MedicationId { get; set; }
        public int CustomerId { get; set; }
        public string MedicationName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Description { get; set; }
        public int TotalCount { get; set; }
        public MedicationItemDTO[] medicationItems { get; set; }

    }
}
