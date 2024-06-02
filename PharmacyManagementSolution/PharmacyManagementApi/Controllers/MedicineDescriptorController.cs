using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Models.DTO.ErrorDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Services;
using System.Diagnostics.CodeAnalysis;

namespace PharmacyManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ExcludeFromCodeCoverage]
    public class MedicineDescriptorController : ControllerBase
    {
        private readonly MedicineDescriptorAIService _aiService;
        private readonly ILogger<MedicineDescriptorController> _logger;

        public MedicineDescriptorController(MedicineDescriptorAIService aiService, ILogger<MedicineDescriptorController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        /// <summary>
        /// Gets a response from the symptom analysis AI service.
        /// </summary>
        /// <param name="message">The input message for the AI service.</param>
        /// <returns>The response from the AI service.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> GetChatResponse([FromBody] string message)
        {
            try
            {
                _logger.LogInformation($"Received a request for symptom analysis: {message}");

                var response = await _aiService.GetSymptomAnalysisAsync(message);
                _logger.LogInformation("Symptom analysis completed successfully.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during symptom analysis: {ex.Message}");
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }
    }
}