﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.API.Objects.Methods;

public class GroupsFilter
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("enum")]
    public List<string> Enum { get; set; }
}