// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using BotFrameworkApp;
using DataVerseClient;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Orchestrator;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class OpenAIBot : ActivityHandler
    {
        private readonly IChatState _chatState;
        private readonly IToolProvider _toolProvider;
        private readonly TraceMessageScenarioLogger _scenarioLogger;

        public OpenAIBot(IChatState chatState, IToolProvider toolProvider, TraceMessageScenarioLogger scenarioLogger)
        {
            _chatState = chatState;
            _toolProvider = toolProvider;
            _scenarioLogger = scenarioLogger;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _scenarioLogger.SetTurnContext(turnContext); // awkward hop between DI systems!

            var userInput = turnContext.Activity.Text;

            var dialogEngine = CreateDialogEngine();

            var state = await _chatState.LoadAsync();

            (state, string assistantResponse) = await dialogEngine.RunAsync(state, userInput);

            await _chatState.SaveAsync(state);

            await turnContext.SendActivityAsync(MessageFactory.Text(assistantResponse), cancellationToken);
        }

        //protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    var welcomeText = "Hello!";
        //    foreach (var member in membersAdded)
        //    {
        //        if (member.Id != turnContext.Activity.Recipient.Id)
        //        {
        //            await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
        //        }
        //    }
        //}

        private IDialogEngine CreateDialogEngine()
        {
            var tools = _toolProvider.GetTools(_scenarioLogger);
            string apiKey = File.ReadAllText(@"C:\keys\openai.txt");
            return new DialogEngine(tools, apiKey, _scenarioLogger);
        }
    }
}
