using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Services;
using System.Diagnostics.CodeAnalysis;

namespace PharmacyManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ExcludeFromCodeCoverage]
    public class MedicineDescriptorController : ControllerBase
    {
        private MedicineDescriptorAIService _aiService;

        public MedicineDescriptorController(MedicineDescriptorAIService aiService) {
            _aiService=aiService;
        }
        [HttpPost]
        public async Task<ActionResult<string>> GetChatResponse([FromBody] string message)
        {
            var response = await _aiService.GetSymptomAnalysisAsync(message);
            return Ok(response);
        }
    }
}
