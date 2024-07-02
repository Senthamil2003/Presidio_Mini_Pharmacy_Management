using System.ComponentModel.DataAnnotations;

namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class FeedbackRequestDTO
    {
        public int CustomerId { get; set; }
        public int MedicineId { get; set; }
        public string Title { get; set; }
        public string Feedback {  get; set; }

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public double Rating { get; set; }

    }
}
