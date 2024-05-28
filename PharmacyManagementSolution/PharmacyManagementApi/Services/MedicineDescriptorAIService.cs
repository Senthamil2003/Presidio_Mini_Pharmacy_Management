using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace PharmacyManagementApi.Services
{
    [ExcludeFromCodeCoverage]
    public class MedicineDescriptorAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
       
        public MedicineDescriptorAIService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }
        [ExcludeFromCodeCoverage]
        public async Task<string> GetSymptomAnalysisAsync(string symptoms)
        {
            var requestContent = new
            {
                model = "gpt-3.5-turbo", 
                messages = new[]
                {
                    new { role = "system", content = "I will give you some Medicine name  you need to give me the Medicine description and its category and the detail way of consume medicine day(example morning 1 tablet after lunch,evening 1 tablet before lunch)"},
                    new { role = "user", content = symptoms }
                },
                max_tokens = 100
            };

            var content = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseString);
            var result = jsonResponse.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return result;
        }
    }
}
