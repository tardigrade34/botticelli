﻿using System.ComponentModel.DataAnnotations;

namespace Botticelli.Server.Analytics.Models;

public class MetricModel : IMetricModel
{
    public string Name { get; set; }

    [Key] public string Id { get; set; }

    public string BotId { get; set; }
    public DateTime Timestamp { get; set; }
    public string InternalValue { get; set; }
}