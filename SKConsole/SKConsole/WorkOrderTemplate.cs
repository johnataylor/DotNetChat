// --------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// --------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Microsoft.FrontlineWorker.SmartAssistanceService.Model;
internal class WorkOrderTemplate
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("workOrderType")]
    public string WorkOrderType { get; set; } = string.Empty;

    [JsonPropertyName("incidentType")]
    public string IncidentType { get; set; } = string.Empty;

    [JsonPropertyName("customerNames")]
    public IEnumerable<string> CustomerNames { get; set; } = new List<string>();
}
