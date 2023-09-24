﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.Messages.API.Responses;

public class Groups
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("items")]
    public Items Items { get; set; }
}