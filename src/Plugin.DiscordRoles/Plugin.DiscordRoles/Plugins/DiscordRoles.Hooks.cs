﻿using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Sync;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    private void OnUserConnected(IPlayer player)
    {
        Logger.Debug($"{nameof(OnUserConnected)} Added Player: {{0}}({{1}}) to be processed", player.Name, player.Id);
        ProcessChange(player, SyncEvent.PlayerConnected);
    }

    private void OnUserGroupAdded(string id, string group)
    {
        IPlayer player = players.FindPlayerById(id);
        Logger.Debug($"{nameof(OnUserGroupAdded)} Added Player: {{0}}({{1}}) to be processed because added to group {{2}}", player.Name, player.Id, group);
        ProcessChange(player, SyncEvent.ServerGroupChanged);
    }

    private void OnUserGroupRemoved(string id, string group)
    {
        IPlayer player = players.FindPlayerById(id);
        Logger.Debug($"{nameof(OnUserGroupRemoved)} Added Player: {{0}}({{1}}) to be processed because removed from group {{2}}", player.Name, player.Id, group);
        ProcessChange(player, SyncEvent.ServerGroupChanged);
    }

    public void ProcessChange(IPlayer player, SyncEvent syncEvent)
    {
        if (player == null)
        {
            return;
        }

        GuildMember member = _link.GetLinkedMember(player, Guild);
        if (member == null)
        {
            Logger.Debug("Skipping processing {0}. Player does not have a valid Discord id.", player.Id);
            return;
        }
            
        _processQueue.RemoveAll(p => p.Player.Id == player.Id && !p.IsLeaving);
        QueueSync(new PlayerSyncRequest(player, member, syncEvent));
    }

    public void ProcessLeaving(string playerId, Snowflake discordId, SyncEvent syncEvent)
    {
        _processQueue.RemoveAll(p => p.Player.Id == playerId);

        IPlayer player = players.FindPlayerById(playerId);
        if (player != null && discordId.IsValid())
        {
            QueueSync(new PlayerSyncRequest(player, discordId, syncEvent, true));
        }
    }
}