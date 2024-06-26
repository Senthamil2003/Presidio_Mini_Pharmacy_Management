namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class AddCategoryDTO
    {
        public string CategoryName { get; set; }
        public IFormFile File { get; set; }
    }
}
