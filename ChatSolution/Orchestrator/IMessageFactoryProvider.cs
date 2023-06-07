namespace Orchestrator
{
    internal interface IMessageFactoryProvider
    {
        Task<MessageFactory> CreateAsync();
    }
}
