using System.Linq;
using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Handlers;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    public void ValidateSyncs()
    {
        GuildMember botMember = Guild.Members[Client.Bot.BotUser.Id];
        DiscordRole botMaxRole = botMember.Roles.Select(r => Guild.Roles[r]).OrderByDescending(r => r.Position).FirstOrDefault();

        for (int index = _config.SyncSettings.Count - 1; index >= 0; index--)
        {
            if (!ProcessSyncSetting(_config.SyncSettings[index], botMaxRole))
            {
                _config.SyncSettings.RemoveAt(index);
            }
        }

        for (int index = _config.PriorityGroupSettings.Count - 1; index >= 0; index--)
        {
            PriorityGroupSettings group = _config.PriorityGroupSettings[index];
            for (int i = group.SyncSettings.Count - 1; i >= 0; i--)
            {
                if (!ProcessSyncSetting(group.SyncSettings[i], botMaxRole))
                {
                    group.SyncSettings.RemoveAt(index);
                }
            }

            if (group.SyncSettings.Count == 0)
            {
                _config.PriorityGroupSettings.RemoveAt(index);
            }
        }
    }
        
    public void ValidateNickname()
    {
        if (_config.Nickname.SyncNicknames)
        {
            PermissionFlags botPerms = Guild.GetUserPermissions(Client.Bot.BotUser.Id);
            if ((botPerms & PermissionFlags.ManageNicknames) == 0)
            {
                Logger.Warning("Sync nicknames is enabled but Discord Bot {0} does not have permission to change {1} Discord Server nicknames. Please grant the {0} bot permission to Manage Nicknames or disable sync nicknames.", Client.Bot.BotUser.FullUserName, Guild.Name);
            }
        }
    }
        
    public void RegisterSyncs()
    {
        SyncHandlers.AddRange(_config.SyncSettings
            .Where(s => s.SyncMode == SyncMode.Bidirectional)
            .Select(s => new BidirectionalSyncHandler(s)));

        SyncHandlers.AddRange(_config.SyncSettings
            .Where(s => s.SyncMode == SyncMode.Server)
            .GroupBy(s => s.RoleId)
            .Select(s => new ServerSyncHandler(s.ToList())));

        SyncHandlers.AddRange(_config.SyncSettings
            .Where(s => s.SyncMode == SyncMode.Discord)
            .GroupBy(s => s.GroupName)
            .Select(s => new DiscordSyncHandler(s.Key, s.ToList())));

        SyncHandlers.AddRange(_config.PriorityGroupSettings
            .Select(p => new PrioritySyncHandler(p)));
    }
}