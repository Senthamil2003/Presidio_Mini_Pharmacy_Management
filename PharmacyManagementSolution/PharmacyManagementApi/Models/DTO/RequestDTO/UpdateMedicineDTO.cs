namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class UpdateMedicineDTO
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public double SellingPrice { get; set; }
        public IFormFile File { get; set; }
        public int Status { get; set; }


    }
}
