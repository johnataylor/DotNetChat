using Azure.AI.OpenAI;

namespace SysConApp
{
    public class ToolPicker
    {
        private string _apiKey;

        public ToolPicker(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<string> RunAsync(string userInput)
        {
            var prompt = CreatePrompt(userInput);
            var response = await RunLlmAsync(prompt);
            return response;
        }

        private async Task<string> RunLlmAsync(string prompt)
        {
            var openAIClient = new OpenAIClient(_apiKey, new OpenAIClientOptions());

            var completionOptions = new CompletionsOptions
            {
                Prompts = { prompt },
                StopSequences = { "Observation:" },
                MaxTokens = 2000,
            };

            var response = await openAIClient.GetCompletionsAsync("text-davinci-003", completionOptions);

            return response.Value.Choices[0].Text;
        }

        private string CreatePrompt(string userInput)
        {
            return userInput;
        }

        // "I need to find out work order identifiers to get any details about them.\r\nAction: GetNewWorkOrders\r\nAction Input: none\r\n"

    }
}
