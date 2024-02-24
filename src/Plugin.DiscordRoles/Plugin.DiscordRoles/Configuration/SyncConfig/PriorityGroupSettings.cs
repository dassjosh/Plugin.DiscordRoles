using System.Collections.Generic;
using DiscordRolesPlugin.Configuration.Notifications;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DiscordRolesPlugin.Configuration.SyncConfig;

public class PriorityGroupSettings : INotificationSettings
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "Sync Mode (Server, Discord, Bidirectional)")]
    public SyncMode SyncMode { get; set; }
        
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty("Priority Mode")]
    public PriorityMode PriorityMode { get; set; }
        
    [JsonProperty(PropertyName = "Priority Sync Settings")]
    public List<PrioritySyncSettings> SyncSettings { get; set; }
        
    [JsonProperty(PropertyName = "Server Notification Settings")]
    public ServerNotificationSettings ServerNotifications { get; set; }
        
    [JsonProperty(PropertyName = "Player Notification Settings")]
    public PlayerNotificationSettings PlayerNotifications { get; set; }

    [JsonConstructor]
    public PriorityGroupSettings() { }

    public PriorityGroupSettings(PriorityGroupSettings settings)
    {
        SyncMode = settings?.SyncMode ?? SyncMode.Server;
        PriorityMode = settings?.PriorityMode ?? PriorityMode.Highest;
        SyncSettings = settings?.SyncSettings ?? new List<PrioritySyncSettings>();
        ServerNotifications = new ServerNotificationSettings(settings?.ServerNotifications);
        PlayerNotifications = new PlayerNotificationSettings(settings?.PlayerNotifications);
        for (int index = 0; index < SyncSettings.Count; index++)
        {
            SyncSettings[index] = new PrioritySyncSettings(SyncSettings[index]);
        }
    }
}