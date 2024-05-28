namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class FeedbackRequestDTO
    {
        public int CustomerId { get; set; }
        public int MedicineId { get; set; }
        public string Feedback {  get; set; }
        public double Rating { get; set; }

    }
}
