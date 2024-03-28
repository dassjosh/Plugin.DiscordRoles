using DiscordRolesPlugin.Enums;
using Newtonsoft.Json;

namespace DiscordRolesPlugin.Configuration;

public class EventSettings
{
    [JsonProperty("Events To Sync Server Groups -> Discord Roles")]
    public EnabledSyncEvents ServerSync { get; set; }

    [JsonProperty("Events To Sync Discord Roles -> Server Groups")]
    public EnabledSyncEvents DiscordSync { get; set; }

    [JsonProperty("Events To Sync Discord Nickname")]
    public EnabledSyncEvents NicknameSync { get; set; }

    [JsonConstructor]
    public EventSettings() { }

    public EventSettings(EventSettings settings)
    {
        ServerSync = new EnabledSyncEvents
        {
            SyncOnPluginLoad = settings?.ServerSync?.SyncOnPluginLoad ?? true,
            SyncOnPlayerConnected = settings?.ServerSync?.SyncOnPlayerConnected ?? true,
            SyncOnServerGroupChanged = settings?.ServerSync?.SyncOnServerGroupChanged ?? true,
            SyncOnDiscordRoleChanged = settings?.ServerSync?.SyncOnDiscordRoleChanged ?? true,
            SyncOnDiscordNicknameChanged = settings?.ServerSync?.SyncOnDiscordNicknameChanged ?? false,
            SyncOnLinkedChanged = settings?.ServerSync?.SyncOnLinkedChanged ?? true,
            SyncOnDiscordServerJoinLeave = settings?.ServerSync?.SyncOnDiscordServerJoinLeave ?? true
        };

        DiscordSync = new EnabledSyncEvents
        {
            SyncOnPluginLoad = settings?.DiscordSync?.SyncOnPluginLoad ?? true,
            SyncOnPlayerConnected = settings?.DiscordSync?.SyncOnPlayerConnected ?? true,
            SyncOnServerGroupChanged = settings?.DiscordSync?.SyncOnServerGroupChanged ?? true,
            SyncOnDiscordRoleChanged = settings?.DiscordSync?.SyncOnDiscordRoleChanged ?? true,
            SyncOnDiscordNicknameChanged = settings?.DiscordSync?.SyncOnDiscordNicknameChanged ?? false,
            SyncOnLinkedChanged = settings?.DiscordSync?.SyncOnLinkedChanged ?? true,
            SyncOnDiscordServerJoinLeave = settings?.DiscordSync?.SyncOnDiscordServerJoinLeave ?? true
        };

        NicknameSync = new EnabledSyncEvents
        {
            SyncOnPluginLoad = settings?.NicknameSync?.SyncOnPluginLoad ?? true,
            SyncOnPlayerConnected = settings?.NicknameSync?.SyncOnPlayerConnected ?? true,
            SyncOnServerGroupChanged = settings?.NicknameSync?.SyncOnServerGroupChanged ?? false,
            SyncOnDiscordRoleChanged = settings?.NicknameSync?.SyncOnDiscordRoleChanged ?? false,
            SyncOnDiscordNicknameChanged = settings?.NicknameSync?.SyncOnDiscordNicknameChanged ?? true,
            SyncOnLinkedChanged = settings?.NicknameSync?.SyncOnLinkedChanged ?? true,
            SyncOnDiscordServerJoinLeave = settings?.NicknameSync?.SyncOnDiscordServerJoinLeave ?? false
        };
    }

    public bool IsServerEnabled(SyncEvent syncEvent)
    {
        return ServerSync.IsEnabled(syncEvent);
    }

    public bool IsDiscordEnabled(SyncEvent syncEvent)
    {
        return DiscordSync.IsEnabled(syncEvent);
    }

    public bool IsNicknameEnabled(SyncEvent syncEvent)
    {
        return NicknameSync.IsEnabled(syncEvent);
    }

    public bool IsAnyEnabled(SyncEvent syncEvent)
    {
        return IsServerEnabled(syncEvent) || IsDiscordEnabled(syncEvent) || IsNicknameEnabled(syncEvent);
    }
}