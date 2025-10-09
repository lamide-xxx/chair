using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Chair.Api.DTOs.Ai;
using Microsoft.AspNetCore.Mvc;

namespace Chair.Api.Controllers;

[ApiController]
[Route("api/{controller}")]
public class AiController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    
    public AiController (IConfiguration config)
    {
        _config = config;
        _httpClient = new HttpClient();
    }

    [HttpPost("recommend-stylist")]
    public async Task<IActionResult> RecommendStylist([FromBody] AiStylistRequest request)
    {
        var apiKey = _config["OPENAI_API_KEY"];
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        string prompt = $@"
                          You are an assistant that recommends hairstylists based on user preferences.
                          The user is looking for: {request.Preference}.
                          Here is a list of stylists (JSON): {JsonSerializer.Serialize(request.Stylists)}.
                          Suggest 3 stylists from the list that best match the preference, and explain why.
                          Respond in JSON: [{{id, name, reason}}]
                          ";
        var body = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = "You are a helpful AI stylist assistant." },
                new { role = "user", content = prompt }
            }
        };
        
        var response = await _httpClient.PostAsJsonAsync(
            "https://api.openai.com/v1/chat/completions",
            body);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var result = await response.Content.ReadAsStringAsync();
        return Ok(result);

    }
}