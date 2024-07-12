namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class MedicationItemDetailDTO
    {
        public int MedicationItemId { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public double amount {  get; set; }
        public string Image { get; set; }
        public string BrandName { get; set; }
        public string Weight { get; set; }
        public int ItemPerPack { get; set; }
    }
}
