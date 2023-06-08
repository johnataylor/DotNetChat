using System.Reflection;

namespace Orchestrator
{
    internal class MessageFactoryProvider : IMessageFactoryProvider
    {
        private static string _introContent;
        private static string _toolResponseTemplate;
        private static string _aiResponseTemplate;
        private static string _userInputContent;

        static MessageFactoryProvider()
        {
            _introContent = ReadEmbeddedResource("introContent.txt");
            _toolResponseTemplate = ReadEmbeddedResource("toolResponseTemplate.txt");
            _aiResponseTemplate = ReadEmbeddedResource("aiResponseTemplate.txt");
            _userInputContent = ReadEmbeddedResource("userInputTemplate.txt");
        }

        public Task<MessageFactory> CreateAsync()
        {
            return Task.FromResult(new MessageFactory(_introContent, _userInputContent, _aiResponseTemplate, _toolResponseTemplate));
        }

        private static string ReadEmbeddedResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Templates.{fileName}") ?? throw new Exception($"unable to load resource '{fileName}'");
            using var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();
            return result.Replace("\r\n", "\n");
        }
    }
}
