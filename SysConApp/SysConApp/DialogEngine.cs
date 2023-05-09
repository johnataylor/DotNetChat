using Azure.AI.OpenAI;

namespace SysConApp
{
    internal class DialogEngine
    {
        private readonly string _apiKey;

        public DialogEngine(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task RunAsync(List<string> transcript, string context)
        {
            var messages = CreateMessages(transcript, context);
            var response = await RunLlmAsync(messages);
            transcript.Add(response);
        }

        private async Task<string> RunLlmAsync(List<ChatMessage> messages)
        {
            var openAIClient = new OpenAIClient(_apiKey, new OpenAIClientOptions());

            var chatCompletionOptions = new ChatCompletionsOptions();

            foreach (var message in messages)
            {
                chatCompletionOptions.Messages.Add(message);
            }

            var response = await openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionOptions);

            return response.Value.Choices[0].Message.Content;
        }

        private static List<ChatMessage> CreateMessages(List<string> transcript, string context)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, "Assistant is a large language model trained by OpenAI.")
            };

            var role = ChatRole.User;
            for (int i = 0; i < transcript.Count; i++)
            {
                messages.Add(new ChatMessage(role, transcript[i]));
                role = role == ChatRole.User ? ChatRole.Assistant : ChatRole.User;
            }

            if (string.IsNullOrEmpty(context))
            {
                context = "EMPTY";
            }

            messages.Add(new ChatMessage(ChatRole.System, $"Answer the user's question using ONLY this content: {context}. If you cannot answer the question, say 'Sorry, I don't know the answer to this one'"));

            return messages;
        }
    }
}
