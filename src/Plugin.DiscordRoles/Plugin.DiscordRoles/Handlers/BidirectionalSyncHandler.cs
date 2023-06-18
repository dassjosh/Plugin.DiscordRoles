using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Interfaces;
using DiscordRolesPlugin.Sync;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Handlers
{
    public class BidirectionalSyncHandler : BaseHandler
    {
        private readonly SyncSettings _settings;
        private readonly string _syncName;

        public BidirectionalSyncHandler(SyncSettings settings)
        {
            _settings = settings;
            _syncName = settings.GetInfoString(SyncMode.Bidirectional);
        }

        protected override bool CanProcess(PlayerSyncRequest request)
        {
            if (request.IsLeaving)
            {
                Plugin.Logger.Debug("Skipping Skipping Bidirectional Sync: Member Is Leaving Discord Server", request.PlayerName, request.IsLeaving);
                return false;
            }

            return true;
        }

        protected override void LogProcessStart(PlayerSyncRequest request) => Plugin.Logger.Debug("Processing Bidirectional Sync: [{0}] Sync for {1} Is Leaving: {2}", _syncName, request.PlayerName, request.IsLeaving);
        protected override BaseSyncSettings GetMatchingSync(PlayerSyncRequest request) => _settings;
        protected override bool IsInGroup(PlayerSyncRequest request, BaseSyncSettings sync) => request.HasGroup(sync.GroupName);
        protected override bool IsInRole(PlayerSyncRequest request, BaseSyncSettings sync) => request.HasRole(sync.RoleId);
        protected override bool RequiresSync(PlayerSyncRequest request, bool isInGroup, bool isInRole)
        {
            if (isInGroup != isInRole)
            {
                return true;
            }

            if (Plugin.Logger.IsLogging(DiscordLogLevel.Debug))
            {
                string playerName = request.PlayerName;
                Plugin.Logger.Debug("{0} Skipping Bidirectional Sync: {1} -> {2} Reason: {3}", playerName, _settings.GroupName, _settings.Role.Name, isInGroup ? "Already Synced" : "Not in group");
            
                if (!isInGroup)
                {
                    Plugin.Logger.Debug("{0} is in the following Groups: {1} Roles: {2}", playerName, request.PlayerGroups, request.PlayerRoles);
                }

            }
            
            return false;
        }

        protected override bool ShouldAdd(bool isInGroup, bool isInRole) => isInGroup || isInRole;
        protected override bool ShouldRemove(PlayerSyncRequest request) => false;
        protected override void HandleAdd(PlayerSyncRequest request, BaseSyncSettings sync, bool isInGroup, bool isInRole)
        {
            if (!isInGroup)
            {
                request.AddServerGroup(_settings.GroupName);
                SendNotification(request, _settings, NotificationType.GroupAdded);
            }

            if (!isInRole)
            {
                request.AddGuildRole(_settings.Role);
                SendNotification(request, _settings, NotificationType.RoleAdded);
            }
        }

        protected override void HandleRemove(PlayerSyncRequest request, BaseSyncSettings sync) {}
        protected override void SendNotification(PlayerSyncRequest request, BaseSyncSettings sync, NotificationType type) =>  Plugin.SendSyncNotification(request, sync, (INotificationSettings)sync, type);
    }
}