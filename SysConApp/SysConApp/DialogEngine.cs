using Azure.AI.OpenAI;
using System.Text;

namespace SysConApp
{
    internal class DialogEngine
    {
        private const string Model = "gpt-3.5-turbo";

        private readonly string _apiKey;

        public DialogEngine(string apiKey)
        {
            _apiKey = apiKey;
        }

        public bool Verbose { get; set; }

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

            if (Verbose)
            {
                ConsoleLogger.LogPrompt(messages);
            }

            var response = await openAIClient.GetChatCompletionsAsync(Model, chatCompletionOptions);
            var content = response.Value.Choices[0].Message.Content;

            if (Verbose)
            {
                ConsoleLogger.LogResponse(content);
            }

            return content;
        }

        private static List<ChatMessage> CreateMessages(List<string> transcript, string context)
        {
            var contextContent = string.IsNullOrEmpty(context) ? "NOTHING" : context;

            var systemMessage = new StringBuilder();
            systemMessage.AppendLine("Assistant is a large language model trained by OpenAI.");
            systemMessage.AppendLine($"The following content can be used to answer questions: ```{contextContent}```.");

            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, systemMessage.ToString())
            };

            var role = ChatRole.User;
            for (int i = 0; i < transcript.Count - 1; i++)
            {
                messages.Add(new ChatMessage(role, transcript[i]));
                role = role == ChatRole.User ? ChatRole.Assistant : ChatRole.User;
            }

            var question = transcript[transcript.Count - 1];

            var lastMessage = new StringBuilder();
            lastMessage.AppendLine("Answer the question using the content provided in this conversation. If you are unable to answer the question, you MUST say ```NO DATA``` and NOTHING ELSE.");
            lastMessage.AppendLine();
            lastMessage.AppendLine($"Question: ```{question}```");

            messages.Add(new ChatMessage(ChatRole.User, lastMessage.ToString()));

            return messages;
        }
    }
}
