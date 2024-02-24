namespace DiscordRolesPlugin.Enums;

public enum SyncEvent : byte
{
    None,
    PluginLoaded,
    PlayerConnected,
    ServerGroupChanged,
    DiscordRoleChanged,
    DiscordNicknameChanged,
    PlayerLinkedChanged,
    DiscordServerJoinLeave
}