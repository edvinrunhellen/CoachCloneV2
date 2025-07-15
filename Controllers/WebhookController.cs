using Microsoft.AspNetCore.Mvc;
using CoachClone.Api.Services;


namespace CoachClone.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly AiService _aiService;

        public WebhookController()
        {
            // Här ska du egentligen injicera config (dependency injection), 
            // men för enkelhet: hårdkoda API-key just nu
            _aiService = new AiService("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", "org-3IZSFyYS9xKEhJV1VqcNAWSA");
        }

        [HttpPost("twilio")]
        public async Task<IActionResult> ReceiveTwilioWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var parsed = System.Web.HttpUtility.ParseQueryString(body);

            var from = parsed["From"];
            var message = parsed["Body"];

            // Hämta journaltext från DB här
            var journalText = "Exempeltext från coachens journaler...";

            var aiReply = await _aiService.GetAiReply(journalText, message);

            Console.WriteLine($"✅ AI reply: {aiReply}");

            return Ok();
        }
    }
}

