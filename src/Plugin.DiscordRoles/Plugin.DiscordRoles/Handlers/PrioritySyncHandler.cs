using System.Collections.Generic;
using System.Linq;
using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Sync;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Handlers;

public class PrioritySyncHandler : BaseHandler
{
    private readonly PriorityGroupSettings _group;
    private readonly List<PrioritySyncSettings> _syncs;
    private readonly string _syncName;

    public PrioritySyncHandler(PriorityGroupSettings settings)
    {
        _group = settings;
        _syncs = settings.SyncSettings;
        _syncs.Sort((l, r) => l.Priority.CompareTo(r.Priority));
        _syncName = string.Join(", ", _syncs.Select(s => s.GetInfoString(_group.SyncMode)));
    }

    protected override bool CanProcess(PlayerSyncRequest request)
    {
        if (_group.SyncMode == SyncMode.Server && request.IsLeaving)
        {
            Plugin.Logger.Debug("Skipping Skipping Priority Sync for Sync Mode: {0}. Member Is Leaving Discord Server", _group.SyncMode);
            return false;
        }

        return true;
    }
    protected override void LogProcessStart(PlayerSyncRequest request) => Plugin.Logger.Debug("Processing Priority Sync: [{0}] Sync for {1} Is Leaving: {2}", _syncName, request.PlayerName, request.IsLeaving);

    protected override BaseSyncSettings GetMatchingSync(PlayerSyncRequest request)
    {
        SyncMode mode = _group.SyncMode;
        for (int index = 0; index < _syncs.Count; index++)
        {
            PrioritySyncSettings settings = _syncs[index];
            if ((mode == SyncMode.Bidirectional || mode == SyncMode.Server) && request.HasGroup(settings.GroupName))
            {
                return settings;
            }
                
            if ((mode == SyncMode.Bidirectional || mode == SyncMode.Discord) && request.HasRole(settings.RoleId))
            {
                return settings;
            }
        }

        return null;
    }

    protected override bool IsInGroup(PlayerSyncRequest request, BaseSyncSettings sync) => sync != null && request.HasGroup(sync.GroupName);
    protected override bool IsInRole(PlayerSyncRequest request, BaseSyncSettings sync) => sync != null && request.HasRole(sync.RoleId);
    protected override bool RequiresSync(PlayerSyncRequest request, bool isInGroup, bool isInRole) => true;
    protected override void ProcessAddRemove(PlayerSyncRequest request, BaseSyncSettings baseSync, bool isInGroup, bool isInRole)
    {
        SyncMode mode = _group.SyncMode;
        PriorityMode priorityMode = _group.PriorityMode;
        PrioritySyncSettings sync = baseSync as PrioritySyncSettings;
        for (int index = 0; index < _syncs.Count; index++)
        {
            PrioritySyncSettings settings = _syncs[index];
                
            bool shouldAdd = false;
            if (priorityMode == PriorityMode.Highest)
            {
                shouldAdd = sync != null && settings == sync;
            }
            else if(priorityMode == PriorityMode.HighestAndBelow)
            {
                shouldAdd = sync != null && settings.Priority <= sync.Priority;
            }

            if (mode == SyncMode.Bidirectional || mode == SyncMode.Server)
            {
                if (shouldAdd)
                {
                    if (!request.HasRole(settings.RoleId))
                    {
                        SendNotification(request, settings, NotificationType.RoleAdded);
                    }
                    request.AddGuildRole(settings.Role);
                }
                else if(request.HasRole(settings.RoleId))
                {
                    request.RemoveGuildRole(settings.Role);
                    SendNotification(request, settings, NotificationType.RoleRemoved);
                }
            }
                
            if (mode == SyncMode.Bidirectional || mode == SyncMode.Discord)
            {
                if (shouldAdd)
                {
                    if (!request.HasGroup(settings.GroupName))
                    {
                        SendNotification(request, settings, NotificationType.GroupAdded);
                    }
                    request.AddServerGroup(settings.GroupName);
                }
                else if(request.HasGroup(settings.GroupName))
                {
                    request.RemoveServerGroup(settings.GroupName);
                    SendNotification(request, settings, NotificationType.GroupRemoved);
                }
            }
        }
    }

    protected override bool ShouldAdd(bool isInGroup, bool isInRole) => false;
    protected override bool ShouldRemove(PlayerSyncRequest request) => false;
    protected override void HandleAdd(PlayerSyncRequest request, BaseSyncSettings sync, bool isInGroup, bool isInRole){}
    protected override void HandleRemove(PlayerSyncRequest request, BaseSyncSettings sync) {}
    protected override void SendNotification(PlayerSyncRequest request, BaseSyncSettings sync, NotificationType type) =>  Plugin.SendSyncNotification(request, sync, _group, type);
}