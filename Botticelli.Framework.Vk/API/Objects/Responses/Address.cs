﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.API.Objects.Responses;

public class Address
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}