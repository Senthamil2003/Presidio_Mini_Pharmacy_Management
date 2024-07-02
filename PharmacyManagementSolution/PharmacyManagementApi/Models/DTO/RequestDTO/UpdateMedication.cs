namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class UpdateMedication
    {
        public int CustomerId { get; set; }
        public int MedicationId { get; set; }
        public int MedicineId {  get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = "";

    }

}
