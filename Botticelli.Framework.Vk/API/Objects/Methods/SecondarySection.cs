﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.API.Objects.Methods;

public class SecondarySection
{
    [JsonPropertyName("$ref")]
    public string Ref { get; set; }
}