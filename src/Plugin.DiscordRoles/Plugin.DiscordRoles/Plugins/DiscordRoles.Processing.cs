using System;
using System.Collections.Generic;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Handlers;
using DiscordRolesPlugin.Sync;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Libraries;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    public void CheckLinkedPlayers()
    {
        if (!IsDiscordLinkEnabled())
        {
            return;
        }

        IReadOnlyDictionary<PlayerId, Snowflake> links = _link.PlayerToDiscordIds;
        foreach (KeyValuePair<PlayerId, Snowflake> link in links)
        {
            IPlayer player = link.Key.Player;
            if (player == null)
            {
                continue;
            }
                
            GuildMember member = Guild.GetMember(link.Value, true);
            if (member != null)
            {
                QueueSync(new PlayerSyncRequest(player, member, SyncEvent.PluginLoaded));
            }
            else
            {
                QueueSync(new PlayerSyncRequest(player, link.Value, SyncEvent.PluginLoaded, false));
            }
        }

        Logger.Info("Starting sync for {0} linked players", _processQueue.Count);
    }

    private bool IsDiscordLinkEnabled()
    {
        if (!_link.IsEnabled)
        {
            PrintWarning("No Discord Link plugin registered! This plugin will not work until one is added. Please add a Discord Link plugin and reload this plugin.");
            return false;
        }

        return true;
    }

    public void QueueSync(PlayerSyncRequest sync)
    {
        if (!IsDiscordLinkEnabled())
        {
            return;
        }
            
        if (_processQueue.Count <= 3)
        {
            _processQueue.Add(sync);
        }
        else
        {
            _processQueue.Insert(3, sync);
        }

        if (_syncTimer == null || _syncTimer.Destroyed)
        {
            _syncTimer = timer.Every(_config.UpdateRate, _processNextCallback);
        }
    }

    public void ProcessNextStartupId()
    {
        if (_processQueue.Count == 0)
        {
            _syncTimer?.Destroy();
            _syncTimer = null;
            return;
        }

        PlayerSyncRequest request = _processQueue[0];
        _processQueue.RemoveAt(0);

        ProcessUser(request);
    }
        
    public void ProcessUser(PlayerSyncRequest request)
    {
        if (request.Member == null)
        {
            request.GetGuildMember();
            return;
        }
            
        Logger.Debug("Start processing: {0} Is Leaving: {1} Server Groups: {2} Discord Roles: {3}", request.PlayerName, request.IsLeaving, request.PlayerGroups, request. PlayerRoles);
            
        for (int index = 0; index < _syncHandlers.Count; index++)
        {
            BaseHandler handler = _syncHandlers[index];
            handler.Process(request);
        }

        HandleUserNick(request);
    }

    public void HandleUserNick(PlayerSyncRequest request)
    {
        if (!_config.Nickname.SyncNicknames)
        {
            Logger.Debug( "Skipping Nickname Sync: Nickname sync is disabled.");
            return;
        }
            
        if (!_config.EventSettings.IsNicknameEnabled(request.Event))
        {
            Logger.Debug( "Skipping Nickname Sync: Nickname sync is disabled for event: {0}.", request.Event);
            return;
        }

        if (request.IsLeaving)
        {
            Logger.Debug( "Skipping Nickname Sync: Member is leaving.");
            return;
        }
            
        if (request.Member.User.Id == Guild.OwnerId)
        {
            Logger.Debug( "Skipping Nickname Sync: Member is Discord Server Owner.");
            return;
        }

        string playerName = GetPlayerName(request.Player);
        if (playerName.Equals(request.Member.Nickname, StringComparison.Ordinal))
        {
            Logger.Debug( "Skipping Nickname Sync: Member Nickname matches '{0}' expected value", playerName);
            return;
        }
            
        Logger.Debug( "Updating {0}'s Discord Nickname {1} -> {2}", request.PlayerName, request.Member.DisplayName, playerName);

        string oldNickname = request.Member.Nickname;
        Guild.EditMemberNick(Client, request.Member.User.Id, playerName).Then(member =>
        {
            Logger.Info( "Successfully updated {0}'s Discord Nickname {1} -> {2}. Discord nickname now has the value: {3}", request.PlayerName, oldNickname, playerName, member.Nickname);
        }).Catch<ResponseError>(error =>
        {
            Logger.Error( "An error has occured updating {0}'s discord server nickname to {1}", request.Member.DisplayName, playerName);
        });
    }
}