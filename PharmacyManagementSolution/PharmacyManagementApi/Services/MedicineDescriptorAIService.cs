using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace PharmacyManagementApi.Services
{
    [ExcludeFromCodeCoverage]
    public class MedicineDescriptorAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<MedicineDescriptorAIService> _logger;
        private readonly IConfiguration _configuration;

        public MedicineDescriptorAIService(HttpClient httpClient, string apiKey, ILogger<MedicineDescriptorAIService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _logger = logger;
            _configuration = configuration;
        }

        [ExcludeFromCodeCoverage]
        public async Task<string> GetSymptomAnalysisAsync(string symptoms)
        {
            var model = _configuration["OpenAI:Model"] ?? "gpt-3.5-turbo";
            var maxTokens = int.Parse(_configuration["OpenAI:MaxTokens"] ?? "100");
            var apiUrl = _configuration["OpenAI:ApiUrl"] ?? "https://api.openai.com/v1/chat/completions";

            var requestContent = new
            {
                model,
                messages = new[]
                {
                    new { role = "system", content = "I will give you a Medicine name. You need to give me the Medicine description, its category, the procedure of consuming the medicine age-wise, and the side effects of using the medicine."},
                    new { role = "user", content = symptoms }
                },
                max_tokens = maxTokens
            };

            var content = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            HttpResponseMessage response;
            try
            {
                _logger.LogInformation("Sending request to OpenAI API...");
                _logger.LogDebug($"Request payload: {JsonSerializer.Serialize(requestContent)}");

                response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogError("Unauthorized access to the OpenAI API. Check the API key.");
                throw new UnauthorizedAccessException("Access to the OpenAI API is unauthorized. Please check your API key.", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"HttpRequestException: {ex.Message}");
                _logger.LogError($"Status Code: {ex.StatusCode}");
                if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    _logger.LogError($"Request payload: {JsonSerializer.Serialize(requestContent)}");
                }
                throw new Exception("An error occurred while calling the OpenAI API.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                throw;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseString);
            var result = jsonResponse.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            _logger.LogInformation("Received response from OpenAI API.");
            return result;
        }
    }
}
