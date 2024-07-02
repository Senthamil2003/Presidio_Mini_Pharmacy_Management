namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class StockResponseDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set;}
        public string Category { get; set;}
        public double Amount { get; set;}
        public int AvailableQuantity { get; set;}
        public double FeedbackSum  { get; set; }
        public double FeedbackCount { get; set; }
        public string Image {  get; set;}
        public string Weight { get; set;}
        public int ItemPerPack { get; set; }
        public int PurchaseCount { get; set; }
        public string BrandName { get;set;}
    }
}
