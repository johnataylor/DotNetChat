namespace Orchestrator
{
    public interface IToolProvider
    {
        Task<List<Tool>> GetToolsAsync();
    }
}
