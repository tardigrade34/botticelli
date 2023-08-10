﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.API.Objects.Methods;

public class MemberStatus
{
    [JsonPropertyName("$ref")]
    public string Ref { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}