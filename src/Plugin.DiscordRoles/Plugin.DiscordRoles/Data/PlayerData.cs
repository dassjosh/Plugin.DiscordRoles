using System;
using System.Collections.Generic;
using DiscordRolesPlugin.Plugins;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities;

namespace DiscordRolesPlugin.Data;

public class PlayerData
{
    public string PlayerId { get; set; }
    public readonly List<string> IgnoreGroup = new();
    public readonly List<Snowflake> IgnoreRole = new();

    [JsonConstructor]
    public PlayerData() { }

    public PlayerData(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            throw new ArgumentNullException(nameof(playerId));
        }
        PlayerId = playerId;
    }

    public bool CanRemoveGroup(string group) => !IgnoreGroup.Contains(group);
    public bool CanRemoveRole(Snowflake role) => !IgnoreRole.Contains(role);

    public void OnGroupAdded(string group)
    {
        IgnoreGroup.Remove(group);
        CheckCleanup();
    }

    public void OnGroupSyncConflict(string group)
    {
        IgnoreGroup.Add(group);
        DiscordRoles.Instance.Data.OnDataChanged();
    }

    public void OnRoleAdded(Snowflake role)
    {
        IgnoreRole.Remove(role);
        CheckCleanup();
    }

    public void OnRoleSyncConflict(Snowflake role)
    {
        IgnoreRole.Add(role);
        DiscordRoles.Instance.Data.OnDataChanged();
    }

    private void CheckCleanup()
    {
        if (IsEmpty())
        {
            DiscordRoles.Instance.Data.Cleanup(PlayerId);
        }
    }

    private bool IsEmpty() => IgnoreGroup.Count == 0 && IgnoreRole.Count == 0;
}