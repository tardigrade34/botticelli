﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.API.Objects;

public class Photo100
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }
}