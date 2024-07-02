namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class ProductDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public double SellingPrice { get; set; }
        public string Weight { get; set; }
        public int ItemPerPack { get; set; }   
        public string ImageBase64 { get; set; }
        public double FeedbackCount { get; set; }
        public double FeedbackSum { get; set; }


    }
}
