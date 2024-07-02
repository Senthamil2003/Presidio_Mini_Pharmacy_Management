namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class MyCartDTO
    {
        public int MedicineId { get; set; }
        public int CartId { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public double Cost { get; set; }
        public string Image {  get; set; }
        public string Brand { get; set; }
        public int ItemPerPack { get; set; }
        public string Weight { get; set; }


    }
}
