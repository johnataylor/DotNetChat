using System.Reflection;

namespace DotNetChat
{
    public class MessageFactoryFactory
    {
        public Task<MessageFactory> CreateAsync()
        {
            var introContent = ReadEmbeddedResource("introContent.txt");
            var toolResponseTemplate = ReadEmbeddedResource("toolResponseTemplate.txt");
            var aiResponseTemplate = ReadEmbeddedResource("aiResponseTemplate.txt");
            var userInputContent = ReadEmbeddedResource("userInputTemplate.txt");
            return Task.FromResult(new MessageFactory(introContent, userInputContent, aiResponseTemplate, toolResponseTemplate));
        }

        private string ReadEmbeddedResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Templates.{fileName}") ?? throw new Exception($"unable to load resource '{fileName}'");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
