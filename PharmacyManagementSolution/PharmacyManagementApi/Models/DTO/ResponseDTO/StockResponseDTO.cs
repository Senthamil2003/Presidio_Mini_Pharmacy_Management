namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class StockResponseDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set;}
        public string Category { get; set;}
        public double Amount { get; set;}
        public int AvailableQuantity { get; set;}
        public double Rating { get; set; }
    }
}
