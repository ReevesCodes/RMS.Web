


using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using RMS.Data.Entities;  // Adjust namespace to your actual Product entity
using RMS.Data.Services;  // Adjust namespace to your actual services
using System.Collections.Generic;

public class ChatbotController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProductService _productService;

    // Store API key securely (use config/environment variables in production)
    private readonly string _apiKey = "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";

    public ChatbotController(IHttpClientFactory httpClientFactory, IProductService productService)
    {
        _httpClientFactory = httpClientFactory;
        _productService = productService;
    }

    public IActionResult Index()
    {
        return View(); // Views/Chatbot/Index.cshtml
    }

    [HttpPost]
    public async Task<IActionResult> GetReply([FromBody] UserMessage request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return Json(new { error = true, message = "Message cannot be empty." });
        }

        string lowerMessage = request.Message.ToLower();
        string reply = null;
        string redirectUrl = null;

        // 1️⃣ Predefined navigation commands
        if (lowerMessage.Contains("shop") || lowerMessage.Contains("products"))
        {
            reply = "Opening the shop page for you...";
            redirectUrl = Url.Action("Index", "Shop");
        }
        else if (lowerMessage.Contains("cart"))
        {
            reply = "Opening your shopping cart...";
            redirectUrl = Url.Action("Index", "Cart");
        }
        else if (lowerMessage.Contains("checkout"))
        {
            reply = "Taking you to checkout...";
            redirectUrl = Url.Action("Checkout", "Order");
        }
        else if (lowerMessage.Contains("support"))
        {
            reply = "Taking you to support page...";
            redirectUrl = Url.Action("Index", "Support");
        }
        else if (lowerMessage.StartsWith("find "))
        {
            string query = lowerMessage.Replace("find ", "").Trim();
            reply = $"Searching for {query}...";
            redirectUrl = Url.Action("Search", "Shop", new { query });
        }

        // 2️⃣ Compare two or more products
        else if (lowerMessage.Contains("difference between") || lowerMessage.Contains("compare"))
        {
            var allProducts = _productService.GetAllProducts().ToList();
            var matchedProducts = allProducts
                .Where(p => lowerMessage.Contains(p.Name.ToLower()))
                .ToList();

            if (matchedProducts.Count >= 2)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Here’s a quick comparison:\n");

                foreach (var product in matchedProducts)
                {
                    sb.AppendLine($"🛒 **{product.Name}**");
                    sb.AppendLine($"• Price: ${product.Price}");
                    sb.AppendLine($"• Description: {product.Description}");
                    sb.AppendLine($"• Stock Available: {product.Stock}");
                    sb.AppendLine();
                }

                reply = sb.ToString();
            }
            else
            {
                reply = "I need at least two product names to compare. Try asking like: 'What's the difference between Speaker Mini and Speaker XL?'";
            }
        }

        // 3️⃣ Get details about a single product
        else if (lowerMessage.StartsWith("tell me about "))
        {
            string productName = lowerMessage.Replace("tell me about ", "").Trim();
            var product = _productService.GetAllProducts()
                          .FirstOrDefault(p => p.Name.ToLower().Contains(productName));

            if (product != null)
            {
                reply = $"Here are the details for {product.Name}:\n" +
                        $"• Price: ${product.Price}\n" +
                        $"• Description: {product.Description}\n" +
                        $"• Stock Available: {product.Stock}";
            }
            else
            {
                reply = $"Sorry, I couldn't find any product named '{productName}'.";
            }
        }

        // 4️⃣ Fallback to GPT API if no custom command matched
        if (reply == null)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var content = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a helpful eCommerce shopping assistant for an online store. Only respond to shopping-related questions such as browsing products, finding items, checking the cart, or helping with checkout. Do not answer anything unrelated like politics, celebrities, or world news."
                    },
                    new { role = "user", content = request.Message }
                }
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(content),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new
                    {
                        error = true,
                        status = (int)response.StatusCode,
                        message = "OpenAI API error",
                        details = responseString
                    });
                }

                dynamic result = JsonConvert.DeserializeObject(responseString);
                reply = result?.choices[0]?.message?.content?.ToString()?.Trim()
                        ?? "Sorry, I couldn't understand that.";
            }
            catch (System.Exception ex)
            {
                return Json(new
                {
                    error = true,
                    message = "Exception while contacting OpenAI",
                    details = ex.Message
                });
            }
        }

        return Json(new { error = false, reply, redirect = redirectUrl });
    }

    public class UserMessage
    {
        public string Message { get; set; }
    }
}

