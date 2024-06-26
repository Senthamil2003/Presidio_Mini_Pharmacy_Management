namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class MedicineDetailDTO
    {
        public string MedicineName { get; set; }
        public string CategoryName { get; set; }    
        public string Description { get; set; }
        public string Brand { get; set; }
        public double SellingPrice { get; set; }
        public double RecentSellingPrice { get; set; }
        public int Status { get; set; }

        public string ImageBase64 { get; set; }

    }
}
