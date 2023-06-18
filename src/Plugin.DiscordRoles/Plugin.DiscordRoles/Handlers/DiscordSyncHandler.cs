using System.Collections.Generic;
using System.Linq;
using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Interfaces;
using DiscordRolesPlugin.Plugins;
using DiscordRolesPlugin.Sync;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Handlers
{
    public class DiscordSyncHandler : BaseHandler
    {
        private readonly string _group;
        private readonly List<SyncSettings> _syncs;
        private readonly string _roleNameList;

        public DiscordSyncHandler(string group, List<SyncSettings> syncs)
        {
            _group = group;
            _syncs = syncs;
            _roleNameList = string.Join(", ", syncs.Select(d => DiscordRoles.Instance.Guild.Roles[d.RoleId].Name));
        }

        protected override bool CanProcess(PlayerSyncRequest request)
        {
            if (request.IsLeaving)
            {
                Plugin.Logger.Debug("Skipping Skipping Bidirectional Sync: Member Is Leaving Discord Server", request.PlayerName, request.IsLeaving);
                return false;
            }

            if (!Plugin._config.EventSettings.IsDiscordEnabled(request.Event))
            {
                Plugin.Logger.Debug("Skipping server sync for event {0}", request.Event);
                return false;
            }
            
            return true;
        }

        protected override void LogProcessStart(PlayerSyncRequest request) => Plugin.Logger.Debug("Processing Discord Sync: [{0}] -> {1} Sync for {2} Is Leaving: {3}", _roleNameList, _group, request.PlayerName, request.IsLeaving);

        protected override BaseSyncSettings GetMatchingSync(PlayerSyncRequest request)
        {
            if (request.IsLeaving || request.Member == null)
            {
                return null;
            }
            
            for (int index = 0; index < _syncs.Count; index++)
            {
                SyncSettings sync = _syncs[index];
                if (request.HasRole(sync.RoleId))
                {
                    return sync;
                }
            }

            return null;
        }

        protected override bool IsInGroup(PlayerSyncRequest request, BaseSyncSettings sync) => request.HasGroup(_group);
        protected override bool IsInRole(PlayerSyncRequest request, BaseSyncSettings sync) => sync != null;
        protected override bool RequiresSync(PlayerSyncRequest request, bool isInGroup, bool isInRole)
        {
            if (isInGroup != isInRole)
            {
                return true;
            }

            if (Plugin.Logger.IsLogging(DiscordLogLevel.Debug))
            {
                string playerName = request.PlayerName;
                Plugin.Logger.Debug("{0} Skipping Discord Sync: [{1}] -> {2} {3}", playerName, _roleNameList, _group, isInRole ? "Already Synced" : "Not in role");
            
                if (!isInGroup)
                {
                    Plugin.Logger.Debug("{0} has the following roles ({1})", playerName, request.PlayerRoles);
                }
            }
            
            return false;
        }
        protected override bool ShouldAdd(bool isInGroup, bool isInRole) => isInRole;
        protected override bool ShouldRemove(PlayerSyncRequest request)
        {
            for (int index = 0; index < _syncs.Count; index++)
            {
                SyncSettings sync = _syncs[index];
                if (sync.RemoveMode == RemoveMode.Keep && request.HasRole(sync.RoleId))
                {
                    Plugin.Logger.Debug("Skipped Removing {0} from Server Group '{1}' because {2} -> {1} Remove Mode is {3}", request.PlayerName, _group, sync.Role.Name, sync.RemoveMode);
                    return false;
                }
            }

            return true;
        }
        protected override void HandleAdd(PlayerSyncRequest request, BaseSyncSettings sync, bool isInGroup, bool isInRole)
        {
            request.AddServerGroup(_group);
            SendNotification(request, sync ?? _syncs[0], NotificationType.GroupAdded);
        }

        protected override void HandleRemove(PlayerSyncRequest request, BaseSyncSettings sync)
        {
            request.RemoveServerGroup(_group);
            SendNotification(request, sync ?? _syncs[0], NotificationType.GroupRemoved);
        }

        protected override void SendNotification(PlayerSyncRequest request, BaseSyncSettings sync, NotificationType type) =>  Plugin.SendSyncNotification(request, sync, (INotificationSettings)sync, type);
    }
}