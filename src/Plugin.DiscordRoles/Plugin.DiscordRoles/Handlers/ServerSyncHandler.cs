using System.Collections.Generic;
using System.Linq;
using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Interfaces;
using DiscordRolesPlugin.Sync;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Handlers;

public class ServerSyncHandler : BaseHandler
{
    private readonly DiscordRole _role;
    private readonly List<SyncSettings> _syncs;
    private readonly string _groupNames;

    public ServerSyncHandler(List<SyncSettings> syncs)
    {
        _role = syncs[0].Role;
        _syncs = syncs;
        _groupNames = string.Join(", ", syncs.Select(d => d.GroupName));
    }

    protected override bool CanProcess(PlayerSyncRequest request) => !request.IsLeaving && Plugin._config.EventSettings.IsServerEnabled(request.Event);
    protected override void LogProcessStart(PlayerSyncRequest request) => Plugin.Logger.Debug("Processing Server Sync: [{0}] -> {1} Sync for {2} Is Leaving: {3}", _groupNames, _role.Name, request.PlayerName, request.IsLeaving);
    protected override BaseSyncSettings GetMatchingSync(PlayerSyncRequest request)
    {
        for (int index = 0; index < _syncs.Count; index++)
        {
            SyncSettings sync = _syncs[index];
            if (request.HasGroup(sync.GroupName))
            {
                return sync;
            }
        }

        return null;
    }
    protected override bool IsInGroup(PlayerSyncRequest request, BaseSyncSettings sync) => sync != null;
    protected override bool IsInRole(PlayerSyncRequest request, BaseSyncSettings sync) => request.HasRole(_role.Id);
    protected override bool RequiresSync(PlayerSyncRequest request, bool isInGroup, bool isInRole)
    {
        if (isInGroup != isInRole)
        {
            return true;
        }

        if (Plugin.Logger.IsLogging(DiscordLogLevel.Debug))
        {
            string playerName = request.PlayerName;
            Plugin.Logger.Debug("{0} Skipping Server Sync: [{1}] -> {2} Reason: {3}", playerName, _groupNames, _role.Name, isInGroup ? "Already Synced" : "Not in group");
            
            if (!isInGroup)
            {
                Plugin.Logger.Debug("{0} is in the following Groups: {1}", playerName, request.PlayerGroups);
            }
        }
            
        return false;
    }
    protected override bool ShouldAdd(bool isInGroup, bool isInRole) => isInGroup;
    protected override bool ShouldRemove(PlayerSyncRequest request)
    {
        for (int index = 0; index < _syncs.Count; index++)
        {
            SyncSettings sync = _syncs[index];
            if (sync.RemoveMode == RemoveMode.Keep && request.HasGroup(sync.GroupName))
            {
                Plugin.Logger.Debug("Skipped Removing {0} from Discord Role '{1}' because RemoveIfNotInSource is false", request.PlayerName, sync.Role.Name);
                return false;
            }
        }

        return true;
    }
    protected override void HandleAdd(PlayerSyncRequest request, BaseSyncSettings sync, bool isInGroup, bool isInRole)
    {
        request.AddGuildRole(_role);
        SendNotification(request, sync ?? _syncs[0], NotificationType.RoleAdded);
    }

    protected override void HandleRemove(PlayerSyncRequest request, BaseSyncSettings sync)
    {
        request.RemoveGuildRole(_role);
        SendNotification(request, sync ?? _syncs[0], NotificationType.RoleRemoved);
    }

    protected override void SendNotification(PlayerSyncRequest request, BaseSyncSettings sync, NotificationType type) =>  Plugin.SendSyncNotification(request, sync, (INotificationSettings)sync, type);
}