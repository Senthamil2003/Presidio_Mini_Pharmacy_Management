namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
     public class MyOrderDTO
      {
        public int OrderDetailId { get; set; }
            public string Image {  get; set; }
        public DateTime OrderDate { get; set; }
        public double Cost { get; set; }
        public bool status { get; set; }
        public string BrandName { get; set; }
            public string MedicineName { get; set; }
        public int ItemPerPack { get; set; }
            public List<DeliveryDetailDTO> DeliveryDetails { get; set; }
     }
}
