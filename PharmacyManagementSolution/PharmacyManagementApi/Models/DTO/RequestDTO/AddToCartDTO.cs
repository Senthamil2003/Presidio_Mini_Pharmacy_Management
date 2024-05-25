namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class AddToCartDTO
    {
        public int UserId { get; set; } 
        public int MedicineId { get; set; }
        public int Quantity { get; set;}
    }
}
