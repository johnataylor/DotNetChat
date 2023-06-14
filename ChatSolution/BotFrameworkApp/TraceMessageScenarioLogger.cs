using Azure.AI.OpenAI;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.TraceExtensions;
using Orchestrator;
using System.Text.Json;

namespace BotFrameworkApp
{
    public class TraceMessageScenarioLogger : IScenarioLogger
    {
        private ITurnContext? _turnContext;

        public void SetTurnContext(ITurnContext turnContext)
        {
            _turnContext = turnContext;
        }

        public TraceMessageScenarioLogger()
        {
        }

        public void LogException(Exception e)
        {
            _turnContext.TraceActivityAsync("LogException", e.Message);
        }

        public void LogFinalAnswer()
        {
            _turnContext.TraceActivityAsync("I have the FinalAnswer!");
        }

        public void LogGeneratedSql(string sql)
        {
            _turnContext.TraceActivityAsync("SQL", sql);
        }

        public void LogLlmResponse(string s)
        {
            _turnContext.TraceActivityAsync("LlmResponse", s);
        }

        public void LogPrompt(IEnumerable<ChatMessage> messages)
        {
            string jsonString = JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
            _turnContext.TraceActivityAsync("Prompt", jsonString);
        }

        public void LogPromptResponse(string content)
        {
            _turnContext.TraceActivityAsync("PromptResponse", content);
        }

        public void LogQuery(string query)
        {
            _turnContext.TraceActivityAsync("query", query);
        }

        public void LogQueryResult(string result)
        {
            _turnContext.TraceActivityAsync("query Result", result);
        }

        public void LogToolResponse(string s)
        {
            _turnContext.TraceActivityAsync("tool Result", s);
        }
    }
}
