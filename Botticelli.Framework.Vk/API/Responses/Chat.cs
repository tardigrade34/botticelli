﻿// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

using System.Text.Json.Serialization;
namespace Botticelli.Framework.Vk.API.Responses;

public class Chat
{
    [JsonPropertyName("$ref")]
    public string Ref { get; set; }
}