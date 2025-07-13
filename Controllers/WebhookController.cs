using Microsoft.AspNetCore.Mvc;

namespace CoachClone.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        [HttpPost("twilio")]
        public async Task<IActionResult> ReceiveTwilioWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            Console.WriteLine("Webhook received from Twilio:");
            Console.WriteLine(body);

            return Ok();
        }
    }
}
