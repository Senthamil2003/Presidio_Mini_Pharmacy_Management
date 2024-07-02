namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class FeedbackDTO
    {
        public int FeedbackId { get; set; }
        public string FeedbackTitle { get; set; }
        public string FeedbackMessage { get; set; }
        public string CustomerName { get; set; }
        public int MedicineId { get; set; }
        public int CustomerId { get; set; }
        public double Rating { get; set; }
        public DateTime Date {  get; set; }
      
    }
}
