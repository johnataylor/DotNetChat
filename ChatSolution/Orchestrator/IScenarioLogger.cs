using Azure.AI.OpenAI;

namespace Orchestrator
{
    public interface IScenarioLogger
    {
        void LogPrompt(IEnumerable<ChatMessage> messages);

        void LogPromptResponse(string content);

        void LogLlmResponse(string s);

        void LogToolResponse(string s);

        void LogFinalAnswer();

        void LogException(Exception e);

        void LogQuery(string query);

        void LogQueryResult(string result);

        void LogGeneratedSql(string sql);
    }
}
