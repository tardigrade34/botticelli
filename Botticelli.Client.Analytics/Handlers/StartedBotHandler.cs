﻿using Botticelli.Analytics.Shared.Metrics;
using Botticelli.Framework.Events;
using Botticelli.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Botticelli.Client.Analytics.Handlers;


public class StartedBotHandler : BasicHandler<StartedBotEventArgs, StartedBotMetric>
{
    public StartedBotHandler(BotContext context, MetricsPublisher publisher, ILogger<StartedBotEventArgs> logger) : base(context, publisher, logger)
    {
    }

    protected override StartedBotMetric Convert(StartedBotEventArgs args, string botId) => new()
    {
        BotId = botId
    };
}