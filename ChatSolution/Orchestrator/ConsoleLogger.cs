using Azure.AI.OpenAI;
using System.Text.Json;

namespace Orchestrator
{
    public class ConsoleLogger : IScenarioLogger
    {
        public void LogPrompt(IEnumerable<ChatMessage> messages)
        {
            string jsonString = JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
            Log(ConsoleColor.Yellow, jsonString);
        }
        public void LogPromptResponse(string content)
        {
            Log(ConsoleColor.DarkYellow, content);
        }

        public void LogLlmResponse(string s)
        {
            Log(ConsoleColor.Green, s);
        }

        public void LogToolResponse(string s)
        {
            Log(ConsoleColor.Magenta, s);
        }

        public void LogFinalAnswer()
        {
            Log(ConsoleColor.Green, "I have the Final Answer!");
        }

        public void LogException(Exception e)
        {
            Log(ConsoleColor.Red, e.Message);
        }

        public void LogQuery(string query)
        {
            Log(ConsoleColor.Cyan, $"query: {query}");
        }

        public void LogQueryResult(string result)
        {
            Log(ConsoleColor.Cyan, $"query-result: {result}");
        }

        public void LogGeneratedSql(string sql)
        {
            Log(ConsoleColor.Magenta, $"SQL: {sql}");
        }

        private static void Log(ConsoleColor color, string message)
        {
            var existingColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = existingColor;
        }
    }
}
