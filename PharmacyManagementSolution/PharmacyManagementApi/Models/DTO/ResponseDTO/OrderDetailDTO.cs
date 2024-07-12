namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class OrderDetailDTO
    {
        public DateTime Date { get; set; }
        public int OrderDetailId { get; set; }
        public int Customerid { get; set; }
        public string MedicineName {  get; set; }
        public int Quantity { get; set; }
        public bool status { get; set; }


    }
}
