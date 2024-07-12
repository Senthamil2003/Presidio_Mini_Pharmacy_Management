namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class AddMedicationItemDTO
    {
        public int MedicationId { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
    }
}
