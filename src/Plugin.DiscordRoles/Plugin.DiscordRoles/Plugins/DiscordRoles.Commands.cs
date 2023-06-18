using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Logging;
using Oxide.Plugins;

namespace DiscordRolesPlugin.Plugins
{
    public partial class DiscordRoles
    {
        [Command("dcr.forcecheck")]
        private void HandleCommand(IPlayer player, string cmd, string[] args)
        {
            Logger.Info("Begin checking all players");
            CheckLinkedPlayers();
        }
    }
}