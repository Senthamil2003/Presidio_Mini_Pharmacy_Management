namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class MedicineFeedbackDTO
    {
        public double  FeedbackRating { get; set; }  
        public List<Feedback> Feedbacks {  get; set; }   

    }
}
