using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;

namespace PortfolioApi;


public class LlmChat
{
    private readonly ILogger<LlmChat> _logger;
    private readonly DatabaseContext _dbContext;
    private readonly Client _geminiClient;

    public LlmChat(ILogger<LlmChat> logger, DatabaseContext dbContext, Client geminiClient)
    {
        _logger = logger;
        _dbContext = dbContext;
        _geminiClient = geminiClient;
    }

    [Function("LlmChat")]
    public async Task<HttpResponseData> Chat(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "llm-chat/chat")]
        HttpRequestData req, HttpClient httpClient
    )
    {
        _logger.LogInformation("LLM chat called");

        var request = await JsonSerializer.DeserializeAsync<ChatRequest>(req.Body);

        // Verify input
        if (request == null || string.IsNullOrEmpty(request.Message))
        {
            var badRes = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRes.WriteStringAsync("Message required");
            return badRes;
        }

        // Parse request into Gemini format
        List<Content> contents = [
            new Content { Role = "user", Parts = [ new Part { Text = request.Message } ] }
        ];

        // TODO: Images
        // https://ai.google.dev/gemini-api/docs/image-understanding?hl=de

        try
        {
            var response = await _geminiClient.Models.GenerateContentAsync(
                model: "gemini-2.5-flash",
                contents
            );

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(new { response = response?.Candidates?[0]?.Content?.Parts?[0].Text });
            return res;
        }
        catch (Exception ex)
        {
            _logger.LogError("Gemini error: {Status}", ex.Message);
            var errRes = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errRes.WriteAsJsonAsync(new { error = "LLM service error" });
            return errRes;
        }
    }
}