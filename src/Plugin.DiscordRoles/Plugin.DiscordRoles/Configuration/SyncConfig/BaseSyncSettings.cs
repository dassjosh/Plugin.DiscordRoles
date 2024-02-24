using DiscordRolesPlugin.Enums;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities;

namespace DiscordRolesPlugin.Configuration.SyncConfig;

public abstract class BaseSyncSettings
{
    [JsonProperty(PropertyName = "Server Group Name", Order = 3)]
    public string GroupName { get; set; }

    [JsonProperty(PropertyName = "Discord Role ID", Order = 4)]
    public Snowflake RoleId { get; set; }

    [JsonIgnore]
    public DiscordRole Role { get; set; }
        
    protected BaseSyncSettings() { }

    protected BaseSyncSettings(string groupName, Snowflake roleId)
    {
            GroupName = groupName;
            RoleId = roleId;
        }

    protected BaseSyncSettings(BaseSyncSettings settings)
    {
            GroupName = settings?.GroupName ?? "Group";
            RoleId = settings?.RoleId ?? default(Snowflake);
        }
        
    public string GetInfoString(SyncMode mode)
    {
            switch (mode)
            {
                case SyncMode.Server:
                    return $"{GroupName} -> {Role.Name}";
                case SyncMode.Discord:
                    return $"{Role.Name} -> {GroupName}";
                case SyncMode.Bidirectional:
                    return $"{GroupName} <-> {Role.Name}";
            }

            return null;
        }
}