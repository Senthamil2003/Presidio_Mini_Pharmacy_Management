namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class AddMedicineDTO
    {
    
        public string MedicineName { get; set; }
        public bool IsnewBrand { get; set; }
        public bool IsnewCategory { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public double SellingPrice { get; set; }
        public IFormFile MedicineImage { get; set; }
        public IFormFile? BrandImage { get; set; }
        public IFormFile? CategoryImage { get; set; }
        public int Status { get; set; }

    }
}
