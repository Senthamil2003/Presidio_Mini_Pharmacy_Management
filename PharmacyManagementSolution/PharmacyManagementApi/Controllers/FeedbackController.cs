using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.ErrorDTO;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(IFeedbackService feedbackService, ILogger<FeedbackController> logger)
        {
            _feedbackService = feedbackService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new feedback for a customer.
        /// </summary>
        /// <param name="feedback">The feedback details.</param>
        /// <returns>A success response on successful addition of the feedback.</returns>
        [HttpPost("AddFeedback")]
        [ProducesResponseType(typeof(SuccessFeedbackDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessFeedbackDTO>> AddFeedback(FeedbackRequestDTO feedback)
        {
            try
            {
                _logger.LogInformation("Received a request to add a new feedback.");

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                feedback.CustomerId = userid;
                var result = await _feedbackService.AddFeedback(feedback);
                _logger.LogInformation("Feedback added successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding feedback: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the list of feedbacks for the current user.
        /// </summary>
        /// <returns>A list of feedbacks for the current user.</returns>
        [HttpGet("ViewMyFeedback")]
        [ProducesResponseType(typeof(SuccessFeedbackDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Feedback>>> ViewMyFeedback()
        {
            try
            {
                _logger.LogInformation("Received a request to view user's feedbacks.");

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _feedbackService.ViewMyFeedBack(userid);
                _logger.LogInformation("User's feedbacks retrieved successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving user's feedbacks: {ex.Message}");
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the feedback for a specific medicine.
        /// </summary>
        /// <param name="medicineId">The ID of the medicine.</param>
        /// <returns>The feedback details for the specified medicine.</returns>
        [HttpGet("ViewMedicineFeedback")]
        [ProducesResponseType(typeof(MedicineFeedbackDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MedicineFeedbackDTO>> ViewMedicineFeedback(int medicineId)
        {
            try
            {
                _logger.LogInformation($"Received a request to view feedback for medicine with ID: {medicineId}.");

                var result = await _feedbackService.MedicineFeedback(medicineId);
                _logger.LogInformation("Medicine feedback retrieved successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving medicine feedback: {ex.Message}");
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
    }
}