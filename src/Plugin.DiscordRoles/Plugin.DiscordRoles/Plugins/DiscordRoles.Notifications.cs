using DiscordRolesPlugin.Configuration.Notifications;
using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Interfaces;
using DiscordRolesPlugin.Sync;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Cache;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Extensions;
using Oxide.Ext.Discord.Libraries;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    public void SendSyncNotification(PlayerSyncRequest request, BaseSyncSettings sync, INotificationSettings settings, NotificationType type)
    {
        using (PlaceholderData data = GetDefault(request.Player, request.Member?.User ?? EntityCache<DiscordUser>.Instance.Get(request.MemberId), sync))
        {
            data.ManualPool();
            if (settings.ServerNotifications.CanSendNotification(type))
            {
                SendServerMessage(settings.ServerNotifications, data, type);
                SendDiscordMessage(settings.ServerNotifications, data, type);
            }
                
            if (settings.PlayerNotifications.CanSendNotification(type))
            {
                SendPlayerMessage(request.Player, settings.PlayerNotifications, data, type);
                if (!request.IsLeaving)
                {
                    SendDiscordPmMessage(request.Player, settings.PlayerNotifications, data, type);
                }
            }
        }
    }

    public void SendServerMessage(ServerNotificationSettings settings, PlaceholderData data, NotificationType type)
    {
        if (settings.SendMessageToServer)
        {
            Chat(Lang(settings.GetLocalizationKey(type), data));
        }
    }

    public void SendDiscordMessage(ServerNotificationSettings settings, PlaceholderData data, NotificationType type)
    {
        DiscordChannel channel = Guild.Channels[settings.DiscordMessageChannelId];
        channel?.CreateGlobalTemplateMessage(Client, settings.GetLocalizationTemplate(type), null, data);
    }

    public void SendPlayerMessage(IPlayer player, PlayerNotificationSettings settings, PlaceholderData data, NotificationType type)
    {
        if (settings.SendMessageToPlayer && player.IsConnected)
        {
            Chat(player, Lang(settings.GetLocalizationKey(type), data, player));
        }
    }

    public void SendDiscordPmMessage(IPlayer player, PlayerNotificationSettings settings, PlaceholderData data, NotificationType type)
    {
        if (settings.SendDiscordPm)
        {
            player.SendDiscordTemplateMessage(Client, settings.GetLocalizationTemplate(type), null, data);
        }
    }
}