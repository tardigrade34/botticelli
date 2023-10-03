﻿using FluentEmail.MailKitSmtp;

namespace Botticelli.Server.Settings;

public class ServerSettings
{
    /// <summary>
    ///     Token lifetime minutes
    /// </summary>
    public int TokenLifetimeMin { get; set; }

    public SmtpClientOptions SmtpClientOptions { get; set; }

    public string ServerEmail { get; set; }

    public string ServerUrl { get; set; }
}