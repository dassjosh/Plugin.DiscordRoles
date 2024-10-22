using DiscordRolesPlugin.Data;
using DiscordRolesPlugin.Lang;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    public string GetPlayerName(IPlayer player)
    {
        string playerName = player.Name;
        if (_config.Nickname.UseAntiSpam && AntiSpam is { IsLoaded: true })
        {
            playerName = AntiSpam.Call<string>("GetClearName", player);
            if (string.IsNullOrEmpty(playerName))
            {
                Logger.Warning("AntiSpam returned an empty string for '{0}'", player.Name);
                playerName = player.Name;
            }
            else if (!playerName.Equals(player.Name))
            {
                Logger.Debug("Nickname '{0}' was filtered by AntiSpam to '{1}'", player.Name, playerName);
            }
        }
            
        if (_config.Nickname.SyncClanTag && Clans is { IsLoaded: true })
        {
            string tag = Clans.Call<string>("GetClanOf", player.Id);
            if (!string.IsNullOrEmpty(tag))
            {
                playerName = Lang(LangKeys.ClanTag, player, tag, playerName);
            }
        }

        if (playerName.Length > 32)
        {
            playerName = playerName.Substring(0, 32);
        }
            
        return playerName;
    }

    public RecentSyncData GetRecentSync(string playerId)
    {
        RecentSyncData data = RecentSync[playerId];
        if (data == null)
        {
            data = new RecentSyncData(_config.ConflictSettings, playerId);
            RecentSync[playerId] = data;
        }

        return data;
    }

    public void SaveData(bool force = false)
    {
        if (Data == null)
        {
            return;
        }
            
        if(!Data.HasChanged || !force)
        {
            return;
        }
            
        Interface.Oxide.DataFileSystem.WriteObject(Name, Data);
        Data.OnSaved();
    }
}