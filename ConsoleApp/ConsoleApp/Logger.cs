using Azure.AI.OpenAI;

namespace DotNetChat
{
    public static class ConsoleLogger
    {
        public static void LogPrompt(IEnumerable<ChatMessage> messages)
        {
            //string jsonString = JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true });
            //Console.WriteLine(jsonString);
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
    }
}
