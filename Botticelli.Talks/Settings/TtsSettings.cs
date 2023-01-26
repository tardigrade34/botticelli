﻿namespace Botticelli.Talks.Settings
{
    public class TtsSettings<TOptions>
    {
        public string? EngineType { get; set; }
        public string? EngineConnection { get; set; }
        public string? Language { get; set; }
        public string? DefaultVoice { get; set; }
        public TOptions? EngineAdditionalOptions { get; set; }
    }
}
