﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.Messages.API.Objects;

public class SpecialServiceType
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("enum")]
    public List<string> Enum { get; set; }
}