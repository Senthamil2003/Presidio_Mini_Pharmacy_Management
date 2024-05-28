namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class MyOrderDTO
    {
        public int OrderDetailId { get; set; }
        public string MedicineName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int Quantity { get; set; }

    }
}
