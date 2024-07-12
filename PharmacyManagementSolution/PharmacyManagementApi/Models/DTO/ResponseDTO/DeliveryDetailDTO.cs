namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class DeliveryDetailDTO
    {
        public int DeliveryId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public string MedicineName { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
