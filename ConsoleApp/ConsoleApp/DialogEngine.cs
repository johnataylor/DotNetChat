using Azure.AI.OpenAI;
using System.Text.Json.Nodes;

namespace DotNetChat
{
    public class DialogEngine
    {
        const int MaxAttempts = 10;
        private readonly IDictionary<string, Tool> _tools;
        private readonly MessageFactory _messageFactory;
        private readonly string _apiKey;

        public DialogEngine(List<Tool> tools, MessageFactory messageFactory, string apiKey)
        {
            _tools = tools.ToDictionary(tool => tool.Name);
            _messageFactory = messageFactory;
            _apiKey = apiKey;
        }

        public async Task RunAsync(List<string> transcript)
        {
            var messages = _messageFactory.CreateMessagesFollowingHumanInput(transcript, _tools.Values);

            for(var i=0; i<MaxAttempts; i++)
            {
                var llmResponse = await RunLlmAsync(messages);

                ConsoleLogger.LogLlmResponse(llmResponse.ToJson());

                if (llmResponse.Action == "Final Answer")
                {
                    ConsoleLogger.LogFinalAnswer();

                    transcript.Add(llmResponse.ActionInput);
                    return;
                }
                else
                {
                    if (_tools.TryGetValue(llmResponse.Action, out var tool))
                    {
                        var toolResponse = await tool.Function(llmResponse.ActionInput);

                        ConsoleLogger.LogToolResponse(toolResponse);

                        if (tool.ReturnDirect)
                        {
                            transcript.Add(toolResponse);
                            return;
                        }
                        else
                        {
                            messages = _messageFactory.CreateMessagesFollowingToolExecution(transcript, _tools.Values, llmResponse.ToJson(), toolResponse);
                        }
                    }
                    else
                    {
                        throw new Exception($"tool '{llmResponse.Action}' was not recognized.");
                    }
                }
            }

            throw new Exception($"reached max attempts");
        }

        private async Task<LlmResponse> RunLlmAsync(List<ChatMessage> messages)
        {
            var openAIClient = new OpenAIClient(_apiKey, new OpenAIClientOptions());

            var chatCompletionOptions = new ChatCompletionsOptions { StopSequences = { "\nObservation:", "\n\tObservation:" } };

            foreach (var message in messages)
            {
                chatCompletionOptions.Messages.Add(message);
            }

            ConsoleLogger.LogPrompt(chatCompletionOptions.Messages);

            var response = await openAIClient.GetChatCompletionsAsync("gpt-3.5-turbo", chatCompletionOptions);

            return new LlmResponse(response.Value.Choices[0].Message.Content);
        }

        private class LlmResponse
        {
            public string Action { get; init; }
            public string ActionInput { get; init; }

            public LlmResponse(string content)
            {
                try
                {
                    content = TrimJsonResponse(content);
                    var obj = JsonNode.Parse(content);
                    Action = obj?["action"]?.ToString() ?? throw new Exception("expected 'action' in response");
                    ActionInput = obj?["action_input"]?.ToString() ?? throw new Exception("expected 'action' in response");
                }
                catch (Exception e)
                {
                    throw new Exception(content, e);
                }
            }

            public string ToJson() => $"{{\n  \"action\": \"{Action}\",\n  \"action_input\": \"{ActionInput}\"\n}}";

            private static string TrimJsonResponse(string s)
            {
                var start = s.IndexOf("{");
                var end = s.LastIndexOf("}");
                return s.Substring(start, end - start + 1);
            }
        }
    }
}
