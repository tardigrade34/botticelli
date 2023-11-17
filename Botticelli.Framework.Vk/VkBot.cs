﻿using BotDataSecureStorage;
using Botticelli.BotBase.Utils;
using Botticelli.Client.Analytics;
using Botticelli.Framework.Events;
using Botticelli.Framework.Exceptions;
using Botticelli.Framework.Vk.Messages.API.Requests;
using Botticelli.Framework.Vk.Messages.API.Responses;
using Botticelli.Framework.Vk.Messages.Handlers;
using Botticelli.Interfaces;
using Botticelli.Shared.API;
using Botticelli.Shared.API.Admin.Requests;
using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Botticelli.Shared.Constants;
using Botticelli.Shared.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Botticelli.Framework.Vk.Messages;

public class VkBot : BaseBot<VkBot>
{
    private readonly IBotUpdateHandler _handler;
    private readonly MessagePublisher _messagePublisher;
    private readonly LongPollMessagesProvider _messagesProvider;
    private readonly SecureStorage _secureStorage;
    private readonly VkStorageUploader _vkUploader;
    private bool _eventsAttached;

    public VkBot(LongPollMessagesProvider messagesProvider,
        MessagePublisher messagePublisher,
        SecureStorage secureStorage,
        VkStorageUploader vkUploader,
        IBotUpdateHandler handler,
        MetricsProcessor metrics,
        ILogger<VkBot> logger) : base(logger, metrics)
    {
        _messagesProvider = messagesProvider;
        _messagePublisher = messagePublisher;
        _secureStorage = secureStorage;
        _handler = handler;
        _vkUploader = vkUploader;
    }

    public override BotType Type => BotType.Vk;


    protected override async Task<StopBotResponse> InnerStopBotAsync(StopBotRequest request, CancellationToken token)
    {
        try
        {
            await _messagesProvider.Stop();

            return StopBotResponse.GetInstance(request.Uid, string.Empty, AdminCommandStatus.Ok);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        return StopBotResponse.GetInstance(request.Uid, "Error stopping a bot", AdminCommandStatus.Fail);
    }

    [Obsolete($"Use {nameof(SetBotContext)}")]
    public override async Task SetBotKey(string key, CancellationToken token)
        => _messagesProvider.SetApiKey(key);

    protected override async Task<StartBotResponse> InnerStartBotAsync(StartBotRequest request, CancellationToken token)
    {
        try
        {
            Logger.LogInformation($"{nameof(StartBotAsync)}...");
            var response = await base.StartBotAsync(request, token);

            if (IsStarted)
            {
                Logger.LogInformation($"{nameof(StartBotAsync)}: already started");

                return response;
            }

            if (response.Status != AdminCommandStatus.Ok || IsStarted) return response;

            if (!_eventsAttached)
            {
                _messagesProvider.OnUpdates += (args, ct) =>
                {
                    var updates = args?.Response?.Updates;

                    if (updates == default || !updates.Any()) return;

                    _handler.HandleUpdateAsync(updates, ct);
                };

                _eventsAttached = true;
            }

            await _messagesProvider.Start(token);

            IsStarted = true;
            Logger.LogInformation($"{nameof(StartBotAsync)}: started");

            return response;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }

        return StartBotResponse.GetInstance(AdminCommandStatus.Fail, "error");
    }

    public override async Task SetBotContext(BotContext context, CancellationToken token)
    {
        var currentContext = _secureStorage.GetBotContext(BotDataUtils.GetBotId());

        if (currentContext?.BotKey != context.BotKey)
        {
            var stopRequest = StopBotRequest.GetInstance();
            var startRequest = StartBotRequest.GetInstance();
            await StopBotAsync(stopRequest, token);

            _secureStorage.SetBotContext(context);

            await _messagesProvider.Stop();
            SetApiKey(context);

            await _messagesProvider.Start(token);
            await StartBotAsync(startRequest, token);
        }
        else
        {
            SetApiKey(context);
        }
    }

    private void SetApiKey(BotContext context)
    {
        _messagesProvider.SetApiKey(context.BotKey);
        _messagePublisher.SetApiKey(context.BotKey);
        _vkUploader.SetApiKey(context.BotKey);
    }

    private string CreateVkAttach(VkSendPhotoResponse fk, string type)
        => $"{type}" +
           $"{fk.Response?.FirstOrDefault()?.OwnerId.ToString()}" +
           $"_{fk.Response?.FirstOrDefault()?.Id.ToString()}";


    private string CreateVkAttach(VkSendVideoResponse fk, string type)
        => $"{type}" +
           $"{fk.Response?.OwnerId.ToString()}" +
           $"_{fk.Response?.VideoId.ToString()}";


    private string CreateVkAttach(VkSendAudioResponse fk, string type)
        => $"{type}" +
           $"{fk.AudioResponseData.AudioMessage.OwnerId}" +
           $"_{fk.AudioResponseData.AudioMessage.Id}";

    private string CreateVkAttach(VkSendDocumentResponse fk, string type)
        => $"{type}" +
           $"{fk.DocumentResponseData.Document.OwnerId}" +
           $"_{fk.DocumentResponseData.Document.Id}";


    protected override async Task<SendMessageResponse> InnerSendMessageAsync<TSendOptions>(SendMessageRequest request,
        ISendOptionsBuilder<TSendOptions> optionsBuilder,
        CancellationToken token)
    {
        foreach (var userId in request.Message.ChatIds)
            try
            {
                var requests = await CreateRequestsWithAttachments(request,
                    userId,
                    token);

                foreach (var vkRequest in requests)
                    await _messagePublisher.SendAsync(vkRequest, token);
            }
            catch (Exception ex)
            {
                throw new BotException("Can't send a message!", ex);
            }

        MessageSent.Invoke(this, new MessageSentBotEventArgs
        {
            Message = request.Message
        });

        return new SendMessageResponse(request.Uid, string.Empty);
    }

    protected override async Task<RemoveMessageResponse> InnerDeleteMessageAsync(RemoveMessageRequest request,
        CancellationToken token) => throw new NotImplementedException();

    private async Task<IEnumerable<VkSendMessageRequest>> CreateRequestsWithAttachments(SendMessageRequest request,
        string userId,
        CancellationToken token)
    {
        var currentContext = _secureStorage.GetBotContext(BotDataUtils.GetBotId());
        var result = new List<VkSendMessageRequest>(100);
        var first = true;

        if (request.Message.Attachments == default)
        {
            var vkRequest = new VkSendMessageRequest
            {
                AccessToken = currentContext.BotKey,
                UserId = userId,
                Body = first ? request.Message.Body : string.Empty,
                Lat = request?.Message.Location?.Latitude,
                Long = request?.Message.Location?.Longitude,
                ReplyTo = request?.Message.ReplyToMessageUid,
                Attachment = null
            };
            result.Add(vkRequest);

            return result;
        }

        foreach (var attach in request.Message?.Attachments)
            try
            {
                var vkRequest = new VkSendMessageRequest
                {
                    AccessToken = currentContext.BotKey,
                    UserId = userId,
                    Body = first ? request.Message.Body : string.Empty,
                    Lat = request?.Message.Location?.Latitude,
                    Long = request?.Message.Location?.Longitude,
                    ReplyTo = request?.Message.ReplyToMessageUid,
                    PeerId = request?.Message.ChatIds.FirstOrDefault(),
                    Attachment = null
                };

                switch (attach)
                {
                    case BinaryAttachment ba:
                    {
                        switch (ba)
                        {
                            case { MediaType: MediaType.Image }:
                            case { MediaType: MediaType.Sticker }:
                                var sendPhotoResponse = await _vkUploader.SendPhotoAsync(vkRequest,
                                    ba.Name,
                                    ba.Data,
                                    token);
                                if (sendPhotoResponse != default)
                                    vkRequest.Attachment = CreateVkAttach(sendPhotoResponse, "photo");

                                break;
                            case { MediaType: MediaType.Video }:
                                //var sendVideoResponse = await _vkUploader.SendVideoAsync(vkRequest,
                                //                                                         ba.Name,
                                //                                                         ba.Data,
                                //                                                         token);

                                //if (sendVideoResponse != default) vkRequest.Attachment = CreateVkAttach(sendVideoResponse, currentContext, "video");

                                break;
                            case { MediaType: MediaType.Voice }:
                            case { MediaType: MediaType.Audio }:
                                var sendAudioMessageResponse = await _vkUploader.SendAudioMessageAsync(vkRequest,
                                    ba.Name,
                                    ba.Data,
                                    token);
                                if (sendAudioMessageResponse != default)
                                    vkRequest.Attachment = CreateVkAttach(sendAudioMessageResponse, "doc");


                                break;
                            case { MediaType: MediaType.Document }:
                                var sendDocMessageResponse = await _vkUploader.SendDocsMessageAsync(vkRequest,
                                    ba.Name,
                                    ba.Data,
                                    token);
                                if (sendDocMessageResponse != default)
                                    vkRequest.Attachment = CreateVkAttach(sendDocMessageResponse, "doc");


                                break;
                        }
                    }

                        break;
                    case InvoiceAttachment:
                        // Not implemented 
                        break;
                }

                result.Add(vkRequest);
                first = false;
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"Error sending a message with attach: {attach.Uid}", ex);
            }

        return result;
    }

    public override async Task<RemoveMessageResponse> DeleteMessageAsync(RemoveMessageRequest request,
        CancellationToken token) => throw new NotImplementedException();

    public override event MsgSentEventHandler MessageSent;
    public override event MsgReceivedEventHandler MessageReceived;
    public override event MsgRemovedEventHandler MessageRemoved;
    public override event MessengerSpecificEventHandler MessengerSpecificEvent;
}