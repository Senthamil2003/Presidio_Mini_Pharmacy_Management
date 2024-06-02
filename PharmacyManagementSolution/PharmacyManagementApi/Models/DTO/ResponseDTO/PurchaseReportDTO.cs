namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class PurchaseReportDTO
    {
        public DateTime PurchaseDate { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int TotalAmount { get; set; }
        public int TotalQuantity { get; set; }

    }
}
