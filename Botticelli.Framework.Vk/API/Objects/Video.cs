﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.API.Objects;

public class Video
{
    [JsonPropertyName("$ref")]
    public string Ref { get; set; }
}