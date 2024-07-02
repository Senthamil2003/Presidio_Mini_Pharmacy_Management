namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class MedicineFeedbackDTO
    {
        public Dictionary<int, double> RatingPercentages { get; set; }
        public double  FeedbackRating { get; set; }  
        public List<FeedbackDTO> Feedbacks {  get; set; }

    }
}
