using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IFeedbackService
    {
        public Task<SuccessFeedbackDTO> AddFeedback(FeedbackRequestDTO feedbackRequest);
        public Task<List<Feedback>> ViewMyFeedBack(int customerId);
        public Task<MedicineFeedbackDTO> MedicineFeedback(int medicineId);
    }
}
