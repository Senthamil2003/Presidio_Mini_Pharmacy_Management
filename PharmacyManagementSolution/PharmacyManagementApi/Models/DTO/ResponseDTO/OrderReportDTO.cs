namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class OrderReportDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public double TotalAmount { get; set; }
        public int TotalQuantity { get; set; }
        public DateTime OrdereDate { get; set; }

    }
}
