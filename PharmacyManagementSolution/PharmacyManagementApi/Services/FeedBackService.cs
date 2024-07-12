using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using Microsoft.Extensions.Logging; // Added for logging

namespace PharmacyManagementApi.Services
{
    public class FeedBackService : IFeedbackService
    {
        private readonly IRepository<int, Medicine> _medicineRepo;
        private readonly IRepository<int, Customer> _customer;
        private readonly ITransactionService _transactionservice;
        private readonly IRepository<int, Feedback> _feedbackRepo;
        private readonly ILogger<FeedBackService> _logger;
        private readonly CustomerFeedbackRepository _customerFeedbackRepo;
        private readonly MedicineFeedbackRepository _medicienFeedbackRepo;

        public FeedBackService(ITransactionService transactionService,
            IRepository<int, Feedback> feedbackrepo,
            IRepository<int, Medicine> medicineRepo,
            IRepository<int, Customer> customer,
            CustomerFeedbackRepository customerFeedbackRepository,
            MedicineFeedbackRepository medicineFeedbackRepository,
            ILogger<FeedBackService> logger) 
        {
            _transactionservice = transactionService;
            _feedbackRepo = feedbackrepo;
            _medicineRepo = medicineRepo;
            _customer = customer;
            _customerFeedbackRepo= customerFeedbackRepository;
            _medicienFeedbackRepo= medicineFeedbackRepository;
            _logger = logger; 
        }

        /// <summary>
        /// Adds a new feedback for a medicine.
        /// </summary>
        /// <param name="feedbackRequest">The feedback request details.</param>
        /// <returns>A success feedback DTO with the feedback ID.</returns>
        public async Task<SuccessFeedbackDTO> AddFeedback(FeedbackRequestDTO feedbackRequest)
        {
            _logger.LogInformation("Adding feedback for medicine {MedicineId} by customer {CustomerId}", feedbackRequest.MedicineId, feedbackRequest.CustomerId); // Log start

            using (var transaction = await _transactionservice.BeginTransactionAsync())
            {
                try
                {
                    Feedback feedback = new Feedback()
                    {
                        CustomerId = feedbackRequest.CustomerId,
                        FeedbackTitle=feedbackRequest.Title,
                        FeedbackMessage = feedbackRequest.Feedback,
                        MedicineId = feedbackRequest.MedicineId,
                        Rating = feedbackRequest.Rating,
                        Date=DateTime.Now,
                    };

                    Medicine medicine = await _medicineRepo.Get(feedbackRequest.MedicineId);
                    //Feedback? checkMedicineFeedback = medicine.Feedbacks.FirstOrDefault(m => m.CustomerId == feedbackRequest.CustomerId);
                    //if (checkMedicineFeedback != null)
                    //{
                    //    _logger.LogWarning("Customer {CustomerId} has already given feedback for medicine {MedicineId}", feedbackRequest.CustomerId, feedbackRequest.MedicineId); // Log warning
                    //    throw new DuplicateValueException("The user has already given the feedback");
                    //}

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

                    _logger.LogInformation("Feedback {FeedbackId} added successfully for medicine {MedicineId} by customer {CustomerId}", result.FeedbackId, feedbackRequest.MedicineId, feedbackRequest.CustomerId); // Log success

                    await _transactionservice.CommitTransactionAsync();
                    return successFeedback;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding feedback for medicine {MedicineId} by customer {CustomerId}", feedbackRequest.MedicineId, feedbackRequest.CustomerId); // Log error
                    await _transactionservice.RollbackTransactionAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Views all the feedbacks given by a customer.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>A list of feedbacks given by the customer.</returns>
        public async Task<List<Feedback>> ViewMyFeedBack(int customerId)
        {
            _logger.LogInformation("Viewing feedbacks for customer {CustomerId}", customerId); // Log start

            try
            {
                // Check if the customer is present
               Customer customer= await _customerFeedbackRepo.Get(customerId);

                List<Feedback> feedbacks =customer.Feedbacks.ToList();
                if (feedbacks.Count() == 0)
                {
                    _logger.LogWarning("No feedback found for customer {CustomerId}", customerId); // Log warning
                    throw new NoFeedbackFoundException("No feedback found for the customer");
                }

                _logger.LogInformation("{Count} feedbacks found for customer {CustomerId}", feedbacks.Count, customerId); // Log success
                return feedbacks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing feedbacks for customer {CustomerId}", customerId); // Log error
                throw;
            }
        }

        /// <summary>
        /// Gets the feedback details for a medicine.
        /// </summary>
        /// <param name="medicineId">The medicine ID.</param>
        /// <returns>A medicine feedback DTO containing the average rating and all feedbacks for the medicine.</returns>
        public async Task<MedicineFeedbackDTO> MedicineFeedback(int medicineId)
        {
            _logger.LogInformation("Getting feedback details for medicine {MedicineId}", medicineId); // Log start

            try
            {
                Medicine medicine = await _medicienFeedbackRepo.Get(medicineId);
                List<Feedback> feedbacks = medicine.Feedbacks.ToList(); 
                if (feedbacks.Count() == 0)
                {
                    _logger.LogWarning("No feedback found for medicine {MedicineId}", medicineId); // Log warning
                    throw new NoFeedbackFoundException("No Feedback Found for the given medicine");
                }

                // Calculate the average rating
                double feedbackRating = feedbacks.Average(f => f.Rating);

               
                var ratingCounts = feedbacks.GroupBy(f => f.Rating)
                                            .ToDictionary(g => g.Key, g => g.Count());

                // Calculate the percentage of each rating
                int totalFeedbacks = feedbacks.Count;
                var ratingPercentages = new Dictionary<int, double>();
                for (int i = 1; i <= 5; i++)
                {
                    ratingPercentages[i] = ratingCounts.ContainsKey(i) ? (ratingCounts[i] / (double)totalFeedbacks) * 100 : 0;
                }
               List <FeedbackDTO> feedbackDTOs = new List<FeedbackDTO>();
                foreach(var feedback in feedbacks)
                {
                    FeedbackDTO feedbackDTO = new FeedbackDTO()
                    {
                        Rating = feedback.Rating,
                        CustomerId = feedback.CustomerId,
                        CustomerName=feedback.Customer.Name,
                        FeedbackId = feedback.FeedbackId,
                        FeedbackMessage = feedback.FeedbackMessage, 
                        FeedbackTitle = feedback.FeedbackTitle,
                        MedicineId= feedback.MedicineId,
                        Date=feedback.Date.Date,
                    };
                    feedbackDTOs.Add(feedbackDTO);
                }
               

                // Prepare the DTO
                MedicineFeedbackDTO medicineFeedback = new MedicineFeedbackDTO()
                {
                    FeedbackRating = feedbackRating,
                    Feedbacks = feedbackDTOs,
                    RatingPercentages = ratingPercentages
                };

                _logger.LogInformation("Feedback details retrieved successfully for medicine {MedicineId}", medicineId); // Log success
                return medicineFeedback;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback details for medicine {MedicineId}", medicineId); // Log error
                throw;
            }
        }

    }
}