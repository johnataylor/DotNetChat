
using Microsoft.FrontlineWorker.SmartAssistanceService.Model;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using System.Diagnostics;
using System.Text.Json;

static void Test0()
{
    var apiKey = File.ReadAllText(@"C:\keys\openai.txt");

    var config = new KernelConfig();
    //config.AddOpenAITextCompletionService("text-davinci-003", apiKey);
    config.AddOpenAIChatCompletionService("gpt-3.5-turbo", apiKey);

    //config.AddConversationAICompletionBackend(loggerFactory, powerPlatformClient, requestContextFactory, openAiConfiguration.AiBuilderTextModelId);

    var kernel = Kernel.Builder
        .WithConfiguration(config)
        .Build();

    var emailBody = File.ReadAllText(@"C:\private\DotNetChat\SKConsole\SKConsole\email.txt");
    var extractTemplate = File.ReadAllText(@"C:\private\DotNetChat\SKConsole\SKConsole\extractTemplate.txt");

    var workOrderTypesStr = "WorkOrderType1, WorkOrderType2, WorkOrderType3";
    var prioritiesStr = "Low, Medium, High";
    var incidentTypesStr = "IncidentType1, IncidentType2, IncidentType3";
    var workOrderTemplateStr = JsonSerializer.Serialize(new WorkOrderTemplate());

    ConversationSummarySkill conversationSummarySkill = new(kernel);
    var coreSkills = kernel.ImportSkill(conversationSummarySkill, nameof(ConversationSummarySkill));

    var semanticFunction = kernel.CreateSemanticFunction(extractTemplate, maxTokens: 1000);

    var context = new ContextVariables();
    context.Set("INPUT", emailBody);

    context.Set("WorkOrderTemplate", workOrderTemplateStr);
    context.Set("WorkOrderTypes", workOrderTypesStr);
    context.Set("Priorities", prioritiesStr);
    context.Set("IncidentTypes", incidentTypesStr);

    var result = kernel.RunAsync(context, semanticFunction).Result;

    if (!result.ErrorOccurred)
    {
        var workOrderStr = result.Result;

        var workOrder = JsonSerializer.Deserialize<WorkOrderTemplate>(workOrderStr) ?? new WorkOrderTemplate();
        var json = JsonSerializer.Serialize(workOrder, new JsonSerializerOptions { WriteIndented = true });

        Console.WriteLine(json);
    }
    else
    {
        Console.WriteLine(result.LastErrorDescription);
    }
}

static void Test1()
{
    var apiKey = File.ReadAllText(@"C:\keys\openai.txt");

    var config = new KernelConfig();
    //config.AddOpenAITextCompletionService("text-davinci-003", apiKey);
    config.AddOpenAIChatCompletionService("gpt-3.5-turbo", apiKey);

    //config.AddConversationAICompletionBackend(loggerFactory, powerPlatformClient, requestContextFactory, openAiConfiguration.AiBuilderTextModelId);

    var kernel = Kernel.Builder
        .WithConfiguration(config)
        .Build();

    var emailBody = File.ReadAllText(@"C:\private\DotNetChat\SKConsole\SKConsole\email.txt");
    var extractTemplate = File.ReadAllText(@"C:\private\DotNetChat\SKConsole\SKConsole\extractTemplate2.txt");

    var workOrderTypesStr = "WorkOrderType1, WorkOrderType2, WorkOrderType3";
    var prioritiesStr = "Low, Medium, High";
    var incidentTypesStr = "IncidentType1, IncidentType2, IncidentType3";

    var semanticFunction = kernel.CreateSemanticFunction(extractTemplate, maxTokens: 2000, temperature: 0.1);

    var context = new ContextVariables();
    context.Set("INPUT", emailBody);

    context.Set("WorkOrderTypes", workOrderTypesStr);
    context.Set("Priorities", prioritiesStr);
    context.Set("IncidentTypes", incidentTypesStr);

    var result = kernel.RunAsync(context, semanticFunction).Result;

    if (!result.ErrorOccurred)
    {
        var workOrderStr = result.Result;

        var workOrder = JsonSerializer.Deserialize<WorkOrderTemplate>(workOrderStr) ?? new WorkOrderTemplate();
        var json = JsonSerializer.Serialize(workOrder, new JsonSerializerOptions { WriteIndented = true });

        Console.WriteLine(json);
    }
    else
    {
        Console.WriteLine(result.LastErrorDescription);
    }
}

var stopwatch = new Stopwatch();

stopwatch.Start();

//Test0();
Test1();

stopwatch.Stop();

Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");
