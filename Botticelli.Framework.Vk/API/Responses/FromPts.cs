﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.Messages.API.Responses;

public class FromPts
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
}