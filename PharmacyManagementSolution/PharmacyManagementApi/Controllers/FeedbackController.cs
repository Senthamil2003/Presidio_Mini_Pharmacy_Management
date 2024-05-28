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
        private readonly IFeedbackService _feebackservice;

        public FeedbackController(IFeedbackService feedbackService) {
            _feebackservice=feedbackService;
        }
        [HttpPost("AddFeedback")]
        [ProducesResponseType(typeof(SuccessFeedbackDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessPurchaseDTO>> AddFeedback(FeedbackRequestDTO feedback)
        {
            try
            {
                var result = await _feebackservice.AddFeedback(feedback);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        [HttpGet("ViewMyFeedback")]
        [ProducesResponseType(typeof(SuccessFeedbackDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Feedback>>> ViewMyFeedback(int userId)
        {
            try
            {
                var result = await _feebackservice.ViewMyFeedBack(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpGet("ViewMedicineFeedback")]
        [ProducesResponseType(typeof(MedicineFeedbackDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MedicineFeedbackDTO>> ViewMedicineFeedback(int medicineId)
        {
            try
            {
                var result = await _feebackservice.MedicineFeedback(medicineId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }
}
