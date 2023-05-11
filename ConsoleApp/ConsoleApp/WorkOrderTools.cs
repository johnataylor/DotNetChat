namespace DotNetChat
{
    public class WorkOrderTools
    {
        [Tool(Description = "Retrieves new work orders. Returns the work order identifiers.")]
        public static Task<string> GetNewWorkOrders(string _)
        {
            return Task.FromResult("New work orders are: WO-1234567, WO-3456789");
        }

        [Tool(Description = "Provides information about the albums by the artist. Input is the name of the artist.")]
        public static Task<string> GetWorkOrderDetails(string workOrder)
        {
            switch (workOrder)
            {
                case "WO-1234567":
                    return Task.FromResult(workOrder + " is at NRV hospital and the RDT is for 3:32 PM today.");
                case "WO-3456789":
                    return Task.FromResult(workOrder + " is at Swedish hospital and the RDT is for 8:00 AM tomorrow.");
                default:
                    return Task.FromResult("I don't know about any ablbums made by " + workOrder);
            }
        }
    }
}
