using Azure.Core;
using Azure.Identity;
using Orchestrator;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ConsoleApp
{
    public class DataVerseTools
    {
        private AccessToken? _accessToken;

        public DataVerseTools()
        {
        }

        public IEnumerable<Tool> GetTools()
        {
            var tools = new List<Tool>
            {
                new Tool(
                    "work-orders",
                    "provides access to information about work orders, takes a natural language query as input",
                    //"provides access to information about work orders, takes a natural language query as input, if you have a work order number make sure to include that in the query, if there is any ambiguity please use the last work order number mentioned",
                    false,
                    QueryWorkOrders
                ),
                new Tool(
                    "account-information",
                    "provides access to information about accounts, takes a natural language query as input",
                    false,
                    QueryAccounts
                ),
                new Tool(
                    "lookup-manuals",
                    "provides access to equipment manuals, takes a natural language query as input",
                    false,
                    QueryManuals
                ),
            };

            return tools;
        }

        private async Task<string> QueryWorkOrders(string query)
        {
            await EnsureAccessTokenIsInitialized();
            var tables = new string[] { "msdyn_workorderproduct", "msdyn_workorder", "msdyn_workorderservicetask", "msdyn_priority" };
            return await FetchStructuredDataAsync(query, tables, _accessToken, true);
        }
        private async Task<string> QueryAccounts(string query)
        {
            await EnsureAccessTokenIsInitialized();
            var tables = new string[] { };
            return await FetchStructuredDataAsync(query, tables, _accessToken, true);
        }

        private async Task<string> QueryManuals(string query)
        {
            await EnsureAccessTokenIsInitialized();
            return await FetchUnstructuredDataAsync(query, _accessToken, true);
        }

        private async Task EnsureAccessTokenIsInitialized()
        {
            if (_accessToken == null)
            {
                var cred = new DefaultAzureCredential(
                    new DefaultAzureCredentialOptions
                    {
                        ExcludeVisualStudioCredential = true,
                        ExcludeInteractiveBrowserCredential = false,
                        InteractiveBrowserCredentialClientId = "51f81489-12ee-4a9e-aaae-a2591f45987d",
                    });
                _accessToken = await cred.GetTokenAsync(new TokenRequestContext(new[] { "https://aurorabapenvc1989.crm10.dynamics.com/user_impersonation" }));
            }
        }

        static async Task<string> FetchStructuredDataAsync(string queryText, string[] tables, AccessToken? accessToken, bool verbose)
        {
            try
            {
                using HttpClient client = new();
                HttpRequestHeaders headers = client.DefaultRequestHeaders;
                headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                headers.Add("Host", "aurorabapenvc1989.crm10.dynamics.com");
                headers.Add("Authorization", $"Bearer {accessToken?.Token}");

                tables.Select(n => new { name = n }).ToArray();

                var requestBody = new
                {
                    queryText = queryText,
                    entityParameters = tables.Length == 0 ? null : tables.Select(n => new { name = n }).ToArray(),
                    options = new[] { "GetResultsSummary" },
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://aurorabapenvc1989.crm10.dynamics.com/api/copilot/v1.0/QueryStructuredData", content);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var obj = JsonNode.Parse(responseContent);

                if (verbose)
                {
                    var sql = obj?["query"]?["tSql"]?.GetValue<string>() ?? string.Empty;
                    ConsoleLogger.LogGeneratedSql(sql);
                }

                var queryResult = obj?["queryResult"];
                if (queryResult == null)
                {
                    throw new Exception("no queryResult");
                }

                var result = queryResult["result"];
                if (result == null)
                {
                    throw new Exception("no result");
                }

                if (result.AsArray().Count == 0)
                {
                    // no data matched the query
                    return string.Empty;
                }

                var summary = queryResult["summary"];
                if (summary == null)
                {
                    throw new Exception("no summary");
                }

                return summary.GetValue<string>();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                if (ex.InnerException != null)
                {
                    await Console.Out.WriteLineAsync(ex.InnerException.Message);
                }
                throw;
            }
        }

        static async Task<string> FetchUnstructuredDataAsync(string queryText, AccessToken? accessToken, bool verbose)
        {
            try
            {
                using HttpClient client = new();
                HttpRequestHeaders headers = client.DefaultRequestHeaders;
                headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                headers.Add("Host", "aurorabapenvc1989.crm10.dynamics.com");
                headers.Add("Authorization", $"Bearer {accessToken?.Token}");

                var requestBody = new
                {
                    searchText = queryText,
                    entityParameters = new[] { new { name = "msdyn_kbattachment" } },
                    totalResultCount = true,
                    top = 1
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://aurorabapenvc1989.crm10.dynamics.com/api/copilot/v1.0/QueryTextContext", content);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                if (ex.InnerException != null)
                {
                    await Console.Out.WriteLineAsync(ex.InnerException.Message);
                }
                throw;
            }
        }
    }
}
