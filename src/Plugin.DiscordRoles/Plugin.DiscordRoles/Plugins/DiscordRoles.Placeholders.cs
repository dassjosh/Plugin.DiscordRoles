using System.Text;
using DiscordRolesPlugin.Configuration.SyncConfig;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Libraries.Placeholders;

namespace DiscordRolesPlugin.Plugins
{
    public partial class DiscordRoles
    {
        public void RegisterPlaceholders()
        {
            _placeholders.RegisterPlaceholder<BaseSyncSettings>(this, "discordroles.sync.group", "sync", GroupName);
        }

        public void GroupName(StringBuilder builder, PlaceholderState state, BaseSyncSettings settings)
        {
            PlaceholderFormatting.Replace(builder, state, settings.GroupName);
        }

        public PlaceholderData GetDefault(IPlayer player, DiscordUser user, BaseSyncSettings settings)
        {
            return _placeholders.CreateData(this).AddPlayer(player).AddUser(user).Add("sync", settings).AddRole(settings.Role);
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
}