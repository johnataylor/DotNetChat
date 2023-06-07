using Azure.AI.OpenAI;
using System.Text.Json;

namespace Orchestrator
{
    public static class ConsoleLogger
    {
        public static void LogPrompt(IEnumerable<ChatMessage> messages)
        {
            string jsonString = JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(jsonString);
        }

        public static void LogLlmResponse(string s)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(s);
            Console.ForegroundColor = prev;
        }

        public static void LogToolResponse(string s)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(s);
            Console.ForegroundColor = prev;
        }

        public static void LogFinalAnswer()
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("I have the Final Answer!");
            Console.ForegroundColor = prev;
        }

        public static void LogException(Exception e)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.ForegroundColor = prev;
        }

        public static void LogQuery(string query)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"query: {query}");
            Console.ForegroundColor = color;
        }

        public static void LogQueryResult(string result)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"query-result: {result}");
            Console.ForegroundColor = color;
        }

        public static void LogGeneratedSql(string sql)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"SQL: {sql}");
            Console.ForegroundColor = color;
        }
    }
}
