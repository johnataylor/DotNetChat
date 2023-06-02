using Azure.AI.OpenAI;

namespace SysConApp
{
    internal static class ConsoleLogger
    {
        public static void LogPrompt(List<ChatMessage> messages)
        {
            foreach (var message in messages)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"{message.Role}: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{message.Content}");
                Console.ForegroundColor = color;
            }
        }

        public static void LogResponse(string response)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"gpt-response: {response}");
            Console.ForegroundColor = color;
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
