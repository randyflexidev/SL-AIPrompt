using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using static System.Environment;

[ApiController]
[Route("api/ai")]
public class AIController : Controller
{
    private readonly AzureOpenAIClient _client;

    public AIController()
    {
        _client = new AzureOpenAIClient(
            new Uri(GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")),
            new Azure.AzureKeyCredential(GetEnvironmentVariable("AZURE_OPENAI_API_KEY"))
        );
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] string prompt)
    {
        try
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("system", "You are a helpful assistant."),
                new UserChatMessage("user", prompt)
            };

            ChatClient chatClient = _client.GetChatClient("gpt-4");
            var response = await chatClient.CompleteChatAsync(messages);

            var generatedText = response.Value?.Content[0]?.Text;

            return Json(new { success = true, message = generatedText });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
