using System.ComponentModel;
using Newtonsoft.Json;

namespace DiscordRolesPlugin.Configuration;

public class NicknameSettings
{
    [DefaultValue(false)]
    [JsonProperty(PropertyName = "Sync Nicknames")]
    public bool SyncNicknames { get; set; }
        
    [DefaultValue(5f)]
    [JsonProperty(PropertyName = "Minimum time between syncing player nicknames (Minutes)")]
    public float TimeBetweenNicknameSync { get; set; }
        
    [DefaultValue(false)]
    [JsonProperty(PropertyName = "Sync Clan Tag")]
    public bool SyncClanTag { get; set; }
        
    [DefaultValue(false)]
    [JsonProperty(PropertyName = "Use AntiSpam On Discord Nickname")]
    public bool UseAntiSpam { get; set; }

    public NicknameSettings(NicknameSettings settings)
    {
        SyncNicknames = settings?.SyncNicknames ?? false;
        TimeBetweenNicknameSync = settings?.TimeBetweenNicknameSync ?? 5f;
        SyncClanTag = settings?.SyncClanTag ?? false;
        UseAntiSpam = settings?.UseAntiSpam ?? false;
    }
}