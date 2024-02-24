using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Plugins;
using DiscordRolesPlugin.Sync;

namespace DiscordRolesPlugin.Handlers;

public abstract class BaseHandler
{
    protected readonly DiscordRoles Plugin = DiscordRoles.Instance;

    protected abstract bool CanProcess(PlayerSyncRequest request);
    protected abstract void LogProcessStart(PlayerSyncRequest request);
    protected abstract BaseSyncSettings GetMatchingSync(PlayerSyncRequest request);
    protected abstract bool IsInGroup(PlayerSyncRequest request, BaseSyncSettings sync);
    protected abstract bool IsInRole(PlayerSyncRequest request, BaseSyncSettings sync);
    protected abstract bool RequiresSync(PlayerSyncRequest request, bool isInGroup, bool isInRole);
    protected abstract bool ShouldAdd(bool isInGroup, bool isInRole);
    protected abstract bool ShouldRemove(PlayerSyncRequest request);
    protected abstract void HandleAdd(PlayerSyncRequest request, BaseSyncSettings sync, bool isInGroup, bool isInRole);
    protected abstract void HandleRemove(PlayerSyncRequest request, BaseSyncSettings sync);
    protected abstract void SendNotification(PlayerSyncRequest request, BaseSyncSettings sync, NotificationType type);

    protected virtual void ProcessAddRemove(PlayerSyncRequest request, BaseSyncSettings sync, bool isInGroup, bool isInRole)
    {
        if (ShouldAdd(isInGroup, isInRole))
        {
            HandleAdd(request, sync, isInGroup, isInRole);
        }
        else if (ShouldRemove(request))
        {
            HandleRemove(request, sync);
        }
    }

    public void Process(PlayerSyncRequest request)
    {
        if (!CanProcess(request))
        {
            return;
        }

        LogProcessStart(request);

        BaseSyncSettings sync = GetMatchingSync(request);
        bool isInGroup = IsInGroup(request, sync);
        bool isInRole = IsInRole(request, sync);
        if (RequiresSync(request, isInGroup, isInRole))
        {
            ProcessAddRemove(request, sync, isInGroup, isInRole);
        }
    }
}