﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Configuration;

public class LogSettings : IDiscordLoggingConfig
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "Plugin Log Level (Verbose, Debug, Info, Warning, Error, Exception, Off)")]
    public DiscordLogLevel PluginLogLevel { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "Console Log Level (Verbose, Debug, Info, Warning, Error, Exception, Off)")]
    public DiscordLogLevel ConsoleLogLevel { get; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "File Log Level (Verbose, Debug, Info, Warning, Error, Exception, Off)")]
    public DiscordLogLevel FileLogLevel { get; }

    [JsonProperty(PropertyName = "File DateTime Format")]
    public string FileDateTimeFormat { get; }

    [JsonProperty(PropertyName = "Console Log Format")]
    public string ConsoleLogFormat { get; }

    [JsonConstructor]
    public LogSettings() { }

    public LogSettings(LogSettings settings)
    {
        PluginLogLevel = settings?.PluginLogLevel ?? DiscordLogLevel.Info;
        ConsoleLogLevel = settings?.ConsoleLogLevel ?? DiscordLogLevel.Info;
        FileLogLevel = settings?.FileLogLevel ?? DiscordLogLevel.Off;
        ConsoleLogFormat = settings?.ConsoleLogFormat ?? "[DiscordRoles] [{0}]: {1}";
        FileDateTimeFormat = settings?.FileDateTimeFormat ?? "HH:mm:ss";
    }
}