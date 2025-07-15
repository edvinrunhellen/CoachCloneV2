using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using CoachClone.Api.Utils;

namespace CoachClone.Api.Services
{
    public class AiService
    {
        private readonly OpenAIService _openAiService;

        public AiService(string apiKey, string organizationId)
        {
            _openAiService = new OpenAIService(new OpenAiOptions
            {
                ApiKey = apiKey,
                Organization = organizationId// ← lägg till denna rad
            });

        }

        public async Task<string> GetAiReply(string journalText, string userMessage)
        {
            var prompt = PromptBuilder.BuildPrompt(journalText, userMessage);

            Console.WriteLine("✅ Prompt to AI:");
            Console.WriteLine(prompt);

            var completion = await _openAiService.Completions.CreateCompletion(new CompletionCreateRequest
            {
                Prompt = prompt,
                MaxTokens = 200,
                Temperature = 0.7f,
                Model = "gpt-4o-mini-2024-07-18"
            });

            Console.WriteLine("✅ AI raw response object: " + completion);

            var choice = completion?.Choices?.FirstOrDefault();

            if (choice == null || string.IsNullOrWhiteSpace(choice.Text))
            {
                return "⚠️ AI kunde inte generera något svar.";
            }

            return choice.Text.Trim();
        }

    }
}
