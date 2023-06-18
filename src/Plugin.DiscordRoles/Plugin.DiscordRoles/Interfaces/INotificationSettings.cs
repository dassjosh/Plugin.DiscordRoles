using DiscordRolesPlugin.Configuration.Notifications;

namespace DiscordRolesPlugin.Interfaces
{
    public interface INotificationSettings
    {
        ServerNotificationSettings ServerNotifications { get; }
        
        PlayerNotificationSettings PlayerNotifications { get; }
    }
}