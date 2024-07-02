namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class UpdateCartDTO
    {
        public int UserId { get; set; }
        public int MedicineId { get; set; }
        public string Status { get; set; } = "Increase";
        public int Quantity { get; set; }
        public int CartId { get; set; }
    }
}
