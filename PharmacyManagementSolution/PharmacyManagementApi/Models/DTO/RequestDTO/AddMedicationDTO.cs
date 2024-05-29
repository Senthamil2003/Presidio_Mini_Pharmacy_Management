namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class AddMedicationDTO
    {
        public int CustomerId { get; set; }
        public string MedicationName { get; set; }
        public MedicationItemDTO[] medicationItems { get; set; }
    }
}
