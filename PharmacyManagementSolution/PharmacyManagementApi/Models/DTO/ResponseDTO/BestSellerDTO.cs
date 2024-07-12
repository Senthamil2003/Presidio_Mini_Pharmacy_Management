namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class BestSellerDTO
    {
        public int MedicineId { get; set; } 
        public string MedicineName { get; set; }
        public double Rating { get; set; }
        public double Price { get; set; }
        public string Brand { get; set; }
        public string Image { get; set; }
        public double FeedbackCount { get; set; }

    }
}
