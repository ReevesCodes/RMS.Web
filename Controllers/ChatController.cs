using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _configuration["OpenAI:ApiKey"]);
    }

    public IActionResult Chatbot() => View();

    
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChatbotMessage([FromBody] UserMessage userMessage)
    {
        if (string.IsNullOrWhiteSpace(userMessage.Message))
            return Json(new { reply = "Please say something." });

        // Prepare OpenAI API request (chat/completions for GPT-4 or GPT-3.5)
        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "user", content = userMessage.Message }
            }
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("chat/completions", jsonContent);
        if (!response.IsSuccessStatusCode)
            return Json(new { reply = "Sorry, I could not get a response right now." });

        var responseString = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(responseString);
        var reply = jsonDoc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return Json(new { reply = reply.Trim() });
    }

    public class UserMessage
    {
        public string Message { get; set; }
    }
}

