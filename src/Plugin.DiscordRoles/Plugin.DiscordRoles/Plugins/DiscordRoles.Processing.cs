using System;
using System.Collections.Generic;
using System.Linq;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Handlers;
using DiscordRolesPlugin.Sync;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Callbacks;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Libraries;
using Oxide.Ext.Discord.Logging;
using Oxide.Ext.Discord.Types;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    private bool _isReadyForProcessing;
    
    public void CheckLinkedPlayers()
    {
        if (!IsDiscordLinkEnabled())
        {
            return;
        }

        // var link = _link.PlayerToDiscordIds.FirstOrDefault();
        // for (int i = 0; i < 100; i++)
        // {
        //     IPlayer player = link.Key.Player;
        //     if (player != null)
        //     {
        //         QueueSync(new PlayerSyncRequest(player, link.Value, SyncEvent.PluginLoaded, false));
        //     }
        // }

        IReadOnlyDictionary<PlayerId, Snowflake> links = _link.PlayerToDiscordIds;
        foreach (KeyValuePair<PlayerId, Snowflake> link in links)
        {
            IPlayer player = link.Key.Player;
            if (player != null)
            {
                QueueSync(new PlayerSyncRequest(player, link.Value, SyncEvent.PluginLoaded, false));
            }
        }

        Logger.Info("Starting sync for {0} linked players", ProcessQueue.Count);
        ProcessNextSync();
        _isReadyForProcessing = true;
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

        if (!sync.MemberId.IsValid())
        {
            Logger.Debug("Skipping Sync: MemberId is invalid. Player ID: {0}({1})", sync.Player.Name, sync.Player.Id);
            return;
        }
            
        if (ProcessQueue.Count <= 3)
        {
            ProcessQueue.Add(sync);
            if (_isReadyForProcessing && ProcessQueue.Count == 1)
            {
                ProcessNextSync();
            }
        }
        else
        {
            ProcessQueue.Insert(3, sync);
        }
    }

    public void ProcessNextSync()
    {
        if (ProcessQueue.Count == 0)
        {
            return;
        }
        
        PlayerSyncRequest request = ProcessQueue[0];
        ProcessQueue.RemoveAt(0);
        Promise promise = Promise.Create();
        promise.Finally(ProcessNextSync);
        ProcessUser(request, promise);
    }
        
    public void ProcessUser(PlayerSyncRequest request, Promise promise)
    {
        ProcessUserCallback callback = ProcessUserCallback.Create(request, this, promise);
        request.GetGuildMember().Finally(callback.RunAction);
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
            Logger.Debug("Skipping Nickname Sync: Member is leaving.");
            return;
        }
            
        if (request.Member.User.Id == Guild.OwnerId)
        {
            Logger.Debug("Skipping Nickname Sync: Member is Discord Server Owner.");
            return;
        }

        string playerName = GetPlayerName(request.Player);
        if (playerName.Equals(request.Member.Nickname, StringComparison.Ordinal))
        {
            Logger.Debug("Skipping Nickname Sync: Member Nickname matches '{0}' expected value", playerName);
            return;
        }
            
        Logger.Debug("Updating {0}'s Discord Nickname {1} -> {2}", request.PlayerName, request.Member.DisplayName, playerName);

        string oldNickname = request.Member.Nickname;
        Guild.EditMemberNick(Client, request.Member.User.Id, playerName).Then(member =>
        {
            Logger.Info("Successfully updated {0}'s Discord Nickname {1} -> {2}. Discord nickname now has the value: {3}", request.PlayerName, oldNickname, playerName, member.Nickname);
        }).Catch<ResponseError>(error =>
        {
            Logger.Error("An error has occured updating {0}'s discord server nickname to {1}.\n{2}", request.Member.DisplayName, playerName);
        });
    }
}