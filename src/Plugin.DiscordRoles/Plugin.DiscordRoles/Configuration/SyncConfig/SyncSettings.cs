using DiscordRolesPlugin.Configuration.Notifications;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Ext.Discord.Entities;

namespace DiscordRolesPlugin.Configuration.SyncConfig;

public class SyncSettings : BaseSyncSettings, INotificationSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "Sync Mode (Server, Discord, Bidirectional)", Order = 1)]
    public SyncMode SyncMode { get; set; }

    [JsonProperty(PropertyName = "Sync Remove Mode (Remove, Keep)", Order = 2)]
    public RemoveMode RemoveMode { get; set; }
        
    [JsonProperty(PropertyName = "Server Notification Settings", Order = 5)]
    public ServerNotificationSettings ServerNotifications { get; set; }
        
    [JsonProperty(PropertyName = "Player Notification Settings", Order = 6)]
    public PlayerNotificationSettings PlayerNotifications { get; set; }
        
    [JsonConstructor]
    public SyncSettings() { }

    public SyncSettings(string groupName, Snowflake discordRole, SyncMode source) : base(groupName, discordRole)
    {
        SyncMode = source;
        RemoveMode = RemoveMode.Remove;
        ServerNotifications = new ServerNotificationSettings(null);
        PlayerNotifications = new PlayerNotificationSettings(null);
    }

    public SyncSettings(SyncSettings settings) : base(settings)
    {
        SyncMode = settings?.SyncMode ?? SyncMode.Server;
        RemoveMode = settings?.RemoveMode ?? RemoveMode.Remove;
        ServerNotifications = new ServerNotificationSettings(settings?.ServerNotifications);
        PlayerNotifications = new PlayerNotificationSettings(settings?.PlayerNotifications);
    }
        
    public bool ShouldSerializeRemoveMode() => SyncMode != SyncMode.Bidirectional;
}