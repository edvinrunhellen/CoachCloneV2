namespace CoachClone.Api.Utils
{
    public static class PromptBuilder
    {
        public static string BuildPrompt(string journalText, string userMessage, string tone = "empatisk, tydlig", string examples = "")
        {
            var prompt = $@"
Du är en coach. Du har följande journalanteckningar från klienten:
---
{journalText}
---

Så här vill coachen att du ska kommunicera: {tone}.

Eventuella exempel eller riktlinjer:
{examples}

Klientens fråga: {userMessage}

Svara på ett personligt och coach-liknande sätt.";
            return prompt;
        }
    }
}
