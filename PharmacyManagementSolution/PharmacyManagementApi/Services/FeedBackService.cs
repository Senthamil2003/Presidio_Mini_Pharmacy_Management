using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;

namespace PharmacyManagementApi.Services
{
    public class FeedBackService:IFeedbackService
    {
        private readonly IRepository<int, Medicine> _medicineRepo;
        private readonly IRepository<int, Customer> _customer;
        private readonly ITransactionService _transactionservice;
        private readonly IRepository<int, Feedback> _feedbackRepo;

        public FeedBackService(ITransactionService transactionService,
            IRepository<int ,Feedback> feedbackrepo,
            IRepository<int ,Medicine> medicineRepo,
            IRepository<int,Customer> customer
            )
        {
            _transactionservice=transactionService;
            _feedbackRepo= feedbackrepo;
            _medicineRepo = medicineRepo;
            _customer= customer;
        }
        public async Task<SuccessFeedbackDTO> AddFeedback(FeedbackRequestDTO feedbackRequest)
        {
            using (var transaction = await _transactionservice.BeginTransactionAsync())
            {

                try
                {

                    Feedback feedback = new Feedback()
                    {
                        CustomerId = feedbackRequest.CustomerId,
                        FeedbackMessage = feedbackRequest.Feedback,
                        MedicineId = feedbackRequest.MedicineId,
                        Rating = feedbackRequest.Rating

                    };
                    Medicine medicine = await _medicineRepo.Get(feedbackRequest.MedicineId);
                    Feedback? checkMedicineFeedback=medicine.Feedbacks.FirstOrDefault(m => m.CustomerId == feedbackRequest.CustomerId);
                    if(checkMedicineFeedback != null)
                    {
                        throw new DuplicateValueException("The user is already ive the feedback");
                    }
                    medicine.FeedbackCount += 1;
                    medicine.FeedbackSum += feedbackRequest.Rating;
                    await _medicineRepo.Update(medicine);
                    Feedback result = await _feedbackRepo.Add(feedback);
                    SuccessFeedbackDTO successFeedback = new SuccessFeedbackDTO()
                    {
                        Code = 200,
                        Message = "Feedback Added Successfully",
                        FeedBackId = result.FeedbackId
                    };

                    await _transactionservice.CommitTransactionAsync();
                    return successFeedback;


                }

                catch
                {
                    await _transactionservice.RollbackTransactionAsync();
                    throw;
                }
            }

        } 
        public async Task<List<Feedback>> ViewMyFeedBack(int customerId)
        {
            try
            {
                //check wether customer present or not
                
               await _customer.Get(customerId);
               List<Feedback> feedbacks =(await _feedbackRepo.Get()).Where(f=>f.CustomerId==customerId).ToList();
                if(feedbacks.Count()==0 )
                {
                    throw new NoFeedbackFoundException("No feedback found for the customer");

                }
                return feedbacks;


            }
            catch
            {
                throw;
            }

        }
        public async Task<MedicineFeedbackDTO> MedicineFeedback(int medicineId)
        {
            try
            {

                Medicine medicine = await _medicineRepo.Get(medicineId);
                List<Feedback> feedbacks = medicine.Feedbacks.ToList();
                if (feedbacks.Count() == 0)
                {
                    throw new NoFeedbackFoundException("No Feedback Found for the given medicine");
                }
                MedicineFeedbackDTO medicineFeedback = new MedicineFeedbackDTO()
                {
                    FeedbackRating = medicine.FeedbackSum / medicine.FeedbackCount,
                    Feedbacks = feedbacks
                };
                return medicineFeedback;


            }
            catch
            {
                throw;
            }
        }



    }
}
