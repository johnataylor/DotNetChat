using Orchestrator;

namespace ConsoleApp
{
    internal class DataVerseTools
    {

        [Tool(Description = "Provides information about work orders. Input is a textual inquiry concerning work orders.")]

        public static Task<string> WorkOrderInquires(string query)
        {
            return Task.FromResult("the summary for work order 00052 is 'install tires'");
        }
    }
}
