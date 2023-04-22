﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Botticelli.Interfaces;
using Botticelli.Shared.API;
using Botticelli.Shared.API.Admin.Requests;
using Botticelli.Shared.API.Admin.Responses;
using Botticelli.Shared.API.Client.Requests;
using Botticelli.Shared.API.Client.Responses;
using Botticelli.Shared.Constants;

namespace Botticelli.Bus.ZeroMQTests.Mocks
{
    public class BotMock : IBot<BotMock>
    {
        public async Task<PingResponse> PingAsync(PingRequest request) 
            => PingResponse.GetInstance(request.Uid);

        public async Task<StartBotResponse> StartBotAsync(StartBotRequest request, CancellationToken token) 
            => StartBotResponse.GetInstance(request.Uid, string.Empty, AdminCommandStatus.Ok);

        public async Task<StopBotResponse> StopBotAsync(StopBotRequest request, CancellationToken token)
            => StopBotResponse.GetInstance(request.Uid, string.Empty, AdminCommandStatus.Ok);

        public async Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request, CancellationToken token) 
            => SendMessageResponse.GetInstance(request.Uid);

        public async Task<RemoveMessageResponse> DeleteMessageAsync(RemoveMessageRequest request, CancellationToken token)
            => RemoveMessageResponse.GetInstance(request.Uid);

        public BotType Type { get; }
    }
}