﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.API.Objects.Methods;

public class GroupsGroupAgeLimits
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("default")]
    public int Default { get; set; }

    [JsonPropertyName("minimum")]
    public int Minimum { get; set; }

    [JsonPropertyName("enumNames")]
    public List<string> EnumNames { get; set; }

    [JsonPropertyName("enum")]
    public List<int> Enum { get; set; }
}