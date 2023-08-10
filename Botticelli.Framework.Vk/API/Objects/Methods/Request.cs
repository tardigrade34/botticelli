﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.API.Objects.Methods;

public class Request
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("$ref")]
    public string Ref { get; set; }
}