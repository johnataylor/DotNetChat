
using ConsoleApp;
using Orchestrator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await Loop();

        static async Task Loop()
        {
            try
            {
                string? state = null;

                var dialogEngine = CreateDialogEngine();

                while (true)
                {
                    Console.Write("user: ");
                    var userInput = Console.ReadLine() ?? string.Empty;

                    if (userInput == "bye")
                    {
                        break;
                    }

                    if (userInput.ToLower().Trim() == string.Empty)
                    {
                        continue;
                    }

                    (state, string assistantResponse) = await dialogEngine.RunAsync(state, userInput);

                    Console.WriteLine($"assistant: {assistantResponse}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("exception...");
                ConsoleLogger.LogException(e);
            }
        }

        static IDialogEngine CreateDialogEngine()
        {
            var dataVerseTools = new DataVerseTools();

            string apiKey = File.ReadAllText(@"C:\keys\openai.txt");

            return new DialogEngine(dataVerseTools.GetTools(), apiKey);
        }
    }
}