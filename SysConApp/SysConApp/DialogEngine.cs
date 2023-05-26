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
            chatCompletionOptions.Temperature = 0;

            foreach (var message in messages)
            {
                chatCompletionOptions.Messages.Add(message);
            }

            var response = await openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionOptions);

            return response.Value.Choices[0].Message.Content;
        }

        private static List<ChatMessage> CreateMessages(List<string> transcript, string context)
        {
            var initialSystemMessage = "Assistant is a large language model trained by OpenAI.";

            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, initialSystemMessage)
            };

            var role = ChatRole.User;
            for (int i = 0; i < transcript.Count - 1; i++)
            {
                messages.Add(new ChatMessage(role, transcript[i]));
                role = role == ChatRole.User ? ChatRole.Assistant : ChatRole.User;
            }

            if (transcript.Count > 0)
            {

                if (!string.IsNullOrEmpty(context))
                {
                    //messages.Add(new ChatMessage(ChatRole.System, $"Answer the user's question using the following content: ```{context}```. If you cannot answer the question, you MUST say 'Sorry, I don't know the answer.'."));
                    //messages.Add(new ChatMessage(ChatRole.User, $"If the question concerns work orders say YEAH ITS A WORK ORDER."));

                    var question = transcript[transcript.Count - 1];
                    var lastMessage = $"If the question concerns work orders say exactly: 'ITS A WORK ORDER QUESTION'.\n\nHere is the question:\n\n{question}.";

                    messages.Add(new ChatMessage(ChatRole.User, $"If the question concerns work orders say YEAH ITS A WORK ORDER."));
                }
            }

            return messages;
        }
    }
}
