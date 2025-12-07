using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GeminiApiKey.Controllers;

/// <summary>
///     Controller responsible for communicating with the Gemini API.
///     Provides an endpoint to send a user message and return the AI response.
/// </summary>
[EnableCors("AllowAngular")]
[ApiController]
[Route("api/[controller]")]
public class GeminiController : ControllerBase
{
    private readonly string _apiKey;
    private readonly ILogger<GeminiController> _logger;
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    ///     Constructor: retrieves the Gemini API key from appsettings and injects the logger.
    /// </summary>
    public GeminiController(IConfiguration configuration, ILogger<GeminiController> logger, IHttpClientFactory clientFactory)
    {
        // Load the API key from configuration (appsettings)
        _apiKey = configuration["GeminiApiKey"] ?? throw new InvalidOperationException("Gemini API Key not found");

        _logger = logger;
        _clientFactory = clientFactory;
    }

    /// <summary>
    ///     Sends a message to the Gemini 2.5 Flash model and returns the AI response.
    /// </summary>
    /// <param name="request">The request object containing the user message.</param>
    /// <returns>The AI-generated response or an error.</returns>
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] AskRequest request)
    {
        // Validate message
        if (string.IsNullOrWhiteSpace(request.message))
            return BadRequest(new { error = "Message is required" });

        try
        {
            using var client = _clientFactory.CreateClient();;

            // Attach API Key to request header
            client.DefaultRequestHeaders.Add("x-goog-api-key", _apiKey);

            // Prepare request payload
            var data = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[] { new { text = request.message } }
                    }
                }
            };

            // Send request to Gemini API
            var response = await client.PostAsJsonAsync(
                "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent",
                data
            );

            // Handle non-success responses
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Gemini API Error: {errorContent}");

                return StatusCode(
                    (int)response.StatusCode,
                    new { error = "Failed to get response from AI" }
                );
            }

            // Read AI response content
            var responseContent = await response.Content.ReadAsStringAsync();
            return Ok(responseContent);
        }
        catch (Exception ex)
        {
            // Log internal errors
            _logger.LogError(ex, "Error in Ask endpoint");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}
