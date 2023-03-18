﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Viber.Messages.Requests.Broadcast;

public class Sender
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }
}