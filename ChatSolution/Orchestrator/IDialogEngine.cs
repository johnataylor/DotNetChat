namespace Orchestrator
{
    public interface IDialogEngine
    {
        Task<(string newState, string assistantResponse)> RunAsync(string? oldState, string userRequest);
    }
}
