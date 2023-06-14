using Orchestrator;

namespace DataVerseClient
{
    public interface IToolProvider
    {
        IEnumerable<Tool> GetTools(IScenarioLogger scenarioLogger);
    }
}
