using Azure.AI.OpenAI;
using System.Text.Json;

namespace Orchestrator
{
    public static class ConsoleLogger
    {
        public static void LogPrompt(IEnumerable<ChatMessage> messages)
        {
            string jsonString = JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
            Log(ConsoleColor.Yellow, jsonString);
        }
        public static void LogPromptResponse(string content)
        {
            Log(ConsoleColor.DarkYellow, content);
        }

        public static void LogLlmResponse(string s)
        {
            Log(ConsoleColor.Green, s);
        }

        public static void LogToolResponse(string s)
        {
            Log(ConsoleColor.Magenta, s);
        }

        public static void LogFinalAnswer()
        {
            Log(ConsoleColor.Green, "I have the Final Answer!");
        }

        public static void LogException(Exception e)
        {
            Log(ConsoleColor.Red, e.Message);
        }

        public static void LogQuery(string query)
        {
            Log(ConsoleColor.Cyan, $"query: {query}");
        }

        public static void LogQueryResult(string result)
        {
            Log(ConsoleColor.Cyan, $"query-result: {result}");
        }

        public static void LogGeneratedSql(string sql)
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
