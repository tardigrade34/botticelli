﻿using Botticelli.Framework.Commands.Validators;
using Botticelli.Interfaces;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Commands.Processors;

public abstract class CommandProcessor<TCommand> : ICommandProcessor
        where TCommand : class, ICommand
{
    private readonly IBot _botClient;
    protected readonly ILogger _logger;
    protected readonly ICommandValidator<TCommand> _validator;

    protected CommandProcessor(IBot botClient,
                               ILogger logger,
                               ICommandValidator<TCommand> validator)
    {
        _botClient = botClient;
        _botClient = botClient;
        _logger = logger;
        _validator = validator;
    }

    public async Task ProcessAsync(long chatId, CancellationToken token, params string[] args)
    {
        var request = SendMessageRequest.GetInstance();
        request.Message = new Message(Guid.NewGuid().ToString());

        try
        {
            if (await _validator.Validate(chatId, args))
                await InnerProcess(chatId, token, args);
            else
            {
                request.Message.Body = _validator.Help();
                await _botClient.SendMessageAsync(request, token);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {GetType().Name}: {ex.Message}");
        }
    }

    protected abstract Task InnerProcess(long chatId, CancellationToken token, params string[] args);
}