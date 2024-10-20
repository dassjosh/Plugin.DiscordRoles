using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Placeholders;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Libraries;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    public void RegisterPlaceholders()
    {
        _placeholders.RegisterPlaceholder<BaseSyncSettings, string>(this, PlaceholderKeys.Group, PlaceholderDataKeys.Sync, settings => settings.GroupName);
    }

    public PlaceholderData GetDefault(IPlayer player, DiscordUser user, BaseSyncSettings settings)
    {
        return _placeholders.CreateData(this).AddPlayer(player).AddUser(user).Add(PlaceholderDataKeys.Sync, settings).AddRole(settings.Role);
    }

    public PlaceholderData GetDefault(IPlayer player)
    {
        return _placeholders.CreateData(this).AddPlayer(player);
    }
        
    public PlaceholderData GetDefault(DiscordUser user)
    {
        return _placeholders.CreateData(this).AddUser(user);
    }

    public PlaceholderData GetDefault(IPlayer player, DiscordUser user)
    {
        return _placeholders.CreateData(this).AddPlayer(player).AddUser(user);
    }
}