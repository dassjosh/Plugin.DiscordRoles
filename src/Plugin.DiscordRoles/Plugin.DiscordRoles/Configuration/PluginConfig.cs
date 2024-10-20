using System.Collections.Generic;
using System.ComponentModel;
using DiscordRolesPlugin.Configuration.SyncConfig;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Configuration;

public class PluginConfig
{
    [DefaultValue("")]
    [JsonProperty(PropertyName = "Discord Bot Token")]
    public string DiscordApiKey { get; set; }
            
    [JsonProperty(PropertyName = "Discord Server ID (Optional if bot only in 1 guild)")]
    public Snowflake GuildId { get; set; }
        
    [DefaultValue(2.5f)]
    [JsonProperty(PropertyName = "Time between processing players (Seconds)")]
    public float UpdateRate { get; set; }
            
    [JsonProperty(PropertyName = "Action To Perform By Event")]
    public EventSettings EventSettings { get; set; }
        
    [JsonProperty(PropertyName = "Conflict Settings")]
    public ConflictSettings ConflictSettings { get; set; }

    [JsonProperty(PropertyName = "Sync Settings")]
    public List<SyncSettings> SyncSettings { get; set; }
        
    [JsonProperty(PropertyName = "Priority Group Settings")]
    public List<PriorityGroupSettings> PriorityGroupSettings { get; set; }

    [JsonProperty(PropertyName = "Nickname Sync Settings")]
    public NicknameSettings Nickname { get; set; }
        
    [JsonProperty(PropertyName = "Plugin Log Settings")]
    public LogSettings LogSettings { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [DefaultValue(DiscordLogLevel.Info)]
    [JsonProperty(PropertyName = "Discord Extension Log Level (Verbose, Debug, Info, Warning, Error, Exception, Off)")]
    public DiscordLogLevel ExtensionDebugging { get; set; }
}