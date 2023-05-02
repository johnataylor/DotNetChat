using Azure.AI.OpenAI;

namespace DotNetChat
{
    public class MessageFactory
    {
        private string _introContent;
        private string _userInputTemplate;
        private string _aiResponseTemplate;
        private string _toolResponseTemplate;

        public MessageFactory(string introContent, string userInputTemplate, string aiResponseTemplate, string toolResponseTemplate)
        {
            _introContent = introContent.Replace("\r\n", "\n");
            _userInputTemplate = userInputTemplate.Replace("\r\n", "\n");
            _aiResponseTemplate = aiResponseTemplate.Replace("\r\n", "\n");
            _toolResponseTemplate = toolResponseTemplate.Replace("\r\n", "\n");
        }

        public List<ChatMessage> CreateMessagesFollowingHumanInput(List<string> transcript, IEnumerable<Tool> tools)
        {
            var messages = new List<ChatMessage> { new ChatMessage(ChatRole.System, GetIntroContent()) };
            var role = ChatRole.User;
            for (int i = 0; i < transcript.Count - 1; i++)
            {
                messages.Add(new ChatMessage(role, transcript[i]));
                role = role == ChatRole.User ? ChatRole.Assistant : ChatRole.User;
            }
            messages.Add(new ChatMessage(ChatRole.User, GetUserInputContent(tools, transcript.Last())));
            return messages;
        }

        public List<ChatMessage> CreateMessagesFollowingToolExecution(List<string> transcript, IEnumerable<Tool> tools, string aiReponseThatTriggeredTool, string toolResponse)
        {
            var messages = CreateMessagesFollowingHumanInput(transcript, tools);
            messages.Add(new ChatMessage(ChatRole.Assistant, GetAIResponseContent(aiReponseThatTriggeredTool)));
            messages.Add(new ChatMessage(ChatRole.User, GetToolResponseContent(toolResponse)));
            return messages;
        }

        private string GetIntroContent() => _introContent;

        private string GetUserInputContent(IEnumerable<Tool> tools, string userInput)
        {
            var toolDescriptions = string.Join("\n", tools.Select(tool => $"> {tool.Name}: {tool.Description}"));
            var toolList = string.Join(", ", tools.Select(tool => tool.Name));
            return string.Format(_userInputTemplate, toolDescriptions, toolList, userInput);
        }
        private string GetToolResponseContent(string toolResponse) => string.Format(_toolResponseTemplate, toolResponse);

        private string GetAIResponseContent(string aiResponse) => string.Format(_aiResponseTemplate, aiResponse);
    }
}
