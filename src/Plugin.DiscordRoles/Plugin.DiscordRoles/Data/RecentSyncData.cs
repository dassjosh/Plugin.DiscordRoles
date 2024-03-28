using DiscordRolesPlugin.Configuration;
using DiscordRolesPlugin.Plugins;
using Oxide.Ext.Discord.Entities;
using Oxide.Plugins;

namespace DiscordRolesPlugin.Data;

public class RecentSyncData
{
    private readonly string _playerId;
    private readonly Hash<string, int> _removedGroupCount = new();
    private readonly Hash<Snowflake, int> _removedRoleCount = new();
    private readonly ConflictSettings _settings;

    public RecentSyncData(ConflictSettings settings, string playerId)
    {
        _settings = settings;
        _playerId = playerId;
    }

    public void OnGroupRemoved(string group)
    {
        int count = _removedGroupCount[group];
        count += 1;
        _removedGroupCount[group] = count;
        if (count > _settings.GroupConflictLimit)
        {
            DiscordRoles.Instance.Data.GetOrCreatePlayerData(_playerId).OnGroupSyncConflict(group);
        }
    }

    public void OnRoleRemoved(Snowflake role)
    {
        int count = _removedRoleCount[role];
        count += 1;
        _removedRoleCount[role] = count;
        if (count > _settings.RoleConflictLimit)
        {
            DiscordRoles.Instance.Data.GetOrCreatePlayerData(_playerId).OnRoleSyncConflict(role);
        }
    }
}