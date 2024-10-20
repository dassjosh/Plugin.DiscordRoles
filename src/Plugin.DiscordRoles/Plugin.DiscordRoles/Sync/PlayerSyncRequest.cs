using System;
using System.Collections.Generic;
using System.Linq;
using DiscordRolesPlugin.Data;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Plugins;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Sync;

public class PlayerSyncRequest
{
    public readonly IPlayer Player;
    public readonly Snowflake MemberId;
    public readonly SyncEvent Event;
        
    public GuildMember Member { get; private set; }
    public bool IsLeaving { get; private set; }
        
    private string _playerName;
    private string _playerGroups;
    private string _playerRoles;
        
    private readonly List<Snowflake> _roles = new();
    private readonly List<string> _groups = new();
    private static readonly Permission Permission = Interface.Oxide.GetLibrary<Permission>();
    private readonly DiscordRoles _plugin = DiscordRoles.Instance;
    private readonly RecentSyncData _recentSync;

    public PlayerSyncRequest(IPlayer player, Snowflake memberId, SyncEvent sync, bool isLeaving)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));
        _groups.AddRange(Permission.GetUserGroups(player.Id));
        MemberId = memberId;
        Event = sync;
        IsLeaving = isLeaving;
        _recentSync = _plugin.GetRecentSync(player.Id);
    }

    public string PlayerName => _playerName ??= $"Player: {Player.Name}({Player.Id}) User: {Member?.User.FullUserName}({MemberId})";
    public string PlayerGroups => _playerGroups ??= string.Join(", ", _groups);
    public string PlayerRoles => _playerRoles ??= string.Join(", ", _roles.Select(r => DiscordRoles.Instance.Guild.Roles[r]?.Name ?? $"Unknown Role ({r})"));
    public bool HasGroup(string group) => _groups.Contains(group, StringComparer.InvariantCultureIgnoreCase);
    public bool HasRole(Snowflake roleId) => !IsLeaving && (_roles.Contains(roleId) || _plugin.Guild.Id == roleId);

    private void SetMember(GuildMember member)
    {
        if (member != null)
        {
            Member = member;
            _roles.Clear();
            _roles.AddRange(member.Roles);
            _playerRoles = null;
        }
    }
        
    public void AddServerGroup(string group)
    {
        PlayerData playerData = _plugin.Data.GetPlayerData(Player.Id);
        playerData?.OnGroupAdded(group);
            
        _plugin.Logger.Info("Adding Player {0} to Server Group '{1}'", PlayerName, group);
        Permission.AddUserGroup(Player.Id, group);
        _groups.Add(group);
        _playerGroups = null;
    }
        
    public void RemoveServerGroup(string group)
    {
        PlayerData playerData = _plugin.Data.GetPlayerData(Player.Id);
        if (playerData != null && !playerData.CanRemoveGroup(group))
        {
            _plugin.Logger.Info("Skipped removing player {0} from Server Group {1}. Conflicting Sync Detected.", PlayerName, group);
            return;
        }
            
        _plugin.Logger.Info("Removing player {0} from Server Group '{1}'", PlayerName, group);
        Permission.RemoveUserGroup(Player.Id, group);
        _groups.Remove(group);
        _playerGroups = null;
        _recentSync.OnGroupRemoved(group);
    }

    public void AddGuildRole(DiscordRole role)
    {
        PlayerData playerData = _plugin.Data.GetPlayerData(Player.Id);
        playerData?.OnRoleAdded(role.Id);
            
        string playerName = PlayerName;
        _plugin.Logger.Info("Adding {0} to Discord Role {1}", playerName, role.Name);
        _roles.Add(role.Id);
        _playerRoles = null;
        DiscordRoles.Instance.Guild.AddMemberRole(DiscordRoles.Instance.Client, Member.User.Id, role.Id).Then(() =>
        {
            _plugin.Logger.Info("Successfully Added {0} to Discord Role {1}", playerName, role.Name);
        }).Catch<ResponseError>(error =>
        {
            if (error.DiscordError is { Code: 50013 })
            {
                _plugin.Logger.Error("An error has occured adding {0} to Discord Role {1}. The Discord Bot {2} does not have permission to add Discord Role {3}.", playerName, role.Name, _plugin.Client.Bot.BotUser.FullUserName, role.Name);
            }
            else
            {
                _plugin.Logger.Error("An error has occured adding {0} to Discord Role {1}.\nCode:{2}\nMessage:{3}", playerName, role.Name, error.HttpStatusCode, error.Message);
            }
        });
    }
        
    public void RemoveGuildRole(DiscordRole role)
    {
        PlayerData playerData = _plugin.Data.GetPlayerData(Player.Id);
        if (playerData != null && !playerData.CanRemoveRole(role.Id))
        {
            _plugin.Logger.Info("Skipped removing player {0} from Discord Role {1}. Conflicting Sync Detected.", PlayerName, role.Name);
            return;
        }
            
        string playerName = PlayerName;
        _roles.Remove(role.Id);
        _playerRoles = null;
        _plugin.Guild.RemoveMemberRole(_plugin.Client, Member.User.Id, role.Id).Then(() =>
        {
            _plugin.Logger.Info("Successfully Removed {0} from Discord Role: {1}", playerName, role.Name);
            _recentSync.OnRoleRemoved(role.Id);
        }).Catch<ResponseError>(error =>
        {
            if (error.DiscordError is { Code: 50013 })
            {
                _plugin.Logger.Error("An error has occured removing {0} from Discord Role {1}. The Discord Bot {2} does not have permission to remove Discord role {3}.", playerName, role.Name, _plugin.Client.Bot.BotUser.FullUserName, role.Name);
                error.SuppressErrorMessage();
            }
            else
            {
                _plugin.Logger.Error("An error has occured removing {0} from Discord Role {1}.\nCode:{2}\nMessage:{3}", playerName, role.Name, error.HttpStatusCode, error.Message);
            }
        });
    }

    public void GetGuildMember()
    {
        _plugin.Guild.GetMember(_plugin.Client, MemberId).Then(member =>
        {
            SetMember(member);
            _plugin.ProcessUser(this);
        }).Catch<ResponseError>(error =>
        {
            if (error.HttpStatusCode == DiscordHttpStatusCode.NotFound)
            {
                error.SuppressErrorMessage();
                IsLeaving = true;
                _plugin.ProcessUser(this);
                return;
            }
                
            _plugin.Logger.Error("An error occured loading Guild Member For: {0}.\nCode:{1}\nMessage:{2}", PlayerName, error.HttpStatusCode, error.Message);
        });
    }
}