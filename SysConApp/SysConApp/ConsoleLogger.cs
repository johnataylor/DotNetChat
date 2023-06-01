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
            Console.WriteLine($"response: {response}");
            Console.ForegroundColor = color;
        }
    }
}
