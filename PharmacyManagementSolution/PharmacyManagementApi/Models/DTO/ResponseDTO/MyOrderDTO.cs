namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
     public class MyOrderDTO
      {
            public int OrderDetailId { get; set; }
            public string MedicineName { get; set; }
            public List<DeliveryDetailDTO> DeliveryDetails { get; set; }
     }
}
