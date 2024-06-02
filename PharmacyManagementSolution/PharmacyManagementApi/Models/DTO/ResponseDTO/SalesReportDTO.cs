namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class SalesReportDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int TotalQuantitySold { get; set; }
    }
}
