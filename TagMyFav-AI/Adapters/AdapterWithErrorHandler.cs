﻿using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;

namespace TagMyFav_AI.Adapters
{
    public class AdapterWithErrorHandler: CloudAdapter
    {
        public AdapterWithErrorHandler(BotFrameworkAuthentication botFrameworkAuthentication, ILogger<CloudAdapter> logger) : base(botFrameworkAuthentication, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                
                logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");
                // Send a message to the user
                await turnContext.SendActivityAsync($"The bot encountered an unhandled error: {exception.Message}");
                await turnContext.SendActivityAsync("To continue to run this bot, please fix the bot source code.");
                // Send a trace activity
                await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
            };
        }
    }
}
