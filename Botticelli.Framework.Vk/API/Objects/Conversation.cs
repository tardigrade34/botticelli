﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.Messages.API.Objects;

public class Conversation
{
    [JsonPropertyName("$ref")]
    public string Ref { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }
}