﻿using Botticelli.Server.Data;
using Botticelli.Server.Data.Entities;
using Botticelli.Shared.API.Admin.Responses;

namespace Botticelli.Server.Services;

/// <summary>
/// This class is intended for managing bots state (start/ stop/ block/ remove)
/// </summary>
public class BotManagementService : IBotManagementService
{
    private readonly BotInfoContext _context;

    public BotManagementService(BotInfoContext context) => _context = context;

    /// <summary>
    /// Gets a bot required status for answering on a poll request from a bot
    /// </summary>
    /// <param name="botId"></param>
    /// <returns></returns>
    public async Task<BotStatus?> GetRequiredBotStatus(string botId)
        => _context.BotInfos.FirstOrDefault(b => b.BotId == botId)?.Status ?? BotStatus.Unknown;

    /// <summary>
    /// Sets a needed bot status in a database
    /// </summary>
    /// <param name="botId"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task SetRequiredBotStatus(string botId, BotStatus status)
    {
        var botInfo = _context.BotInfos.FirstOrDefault(b => b.BotId == botId);

        if (botInfo == default)
        {
            botInfo = new BotInfo
            {
                BotId = botId,
                LastKeepAlive = null,
                Status = status
            };

            _context.BotInfos.Add(botInfo);
        }
        else
            _context.BotInfos.Update(botInfo);

        await _context.SaveChangesAsync();
    }
}