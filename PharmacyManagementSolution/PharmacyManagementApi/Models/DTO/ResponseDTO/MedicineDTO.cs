namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class MedicineDTO
    {
        public string Brandname { get; set; }
        public int MedicineId { get; set; }
        public int CategoryId { get; set; }
        public string MedicineName { get; set; }
        public string CategoryName { get; set; }
        public int CurrentQuantity { get; set; }
        public int Status { get; set; } 

    }
}
