using System;
using System.Collections.Generic;
using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Lang;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Libraries;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    public void RegisterLang()
    {
        Dictionary<string, string> loc = new()
        {
            [LangKeys.Chat] = $"[#BEBEBE][[{AccentColor}]{Title}[/#]] {{0}}[/#]",
            [LangKeys.ClanTag] = "[{0}] {1}"
        };

        for (int index = 0; index < _config.SyncSettings.Count; index++)
        {
            SyncSettings settings = _config.SyncSettings[index];
            settings.ServerNotifications.AddLocalizations(loc);
            settings.PlayerNotifications.AddLocalizations(loc);
        }

        for (int index = 0; index < _config.PriorityGroupSettings.Count; index++)
        {
            PriorityGroupSettings group = _config.PriorityGroupSettings[index];
            group.ServerNotifications.AddLocalizations(loc);
            group.PlayerNotifications.AddLocalizations(loc);
        }

        lang.RegisterMessages(loc, this);
    }

    public void Chat(string message)
    {
        server.Broadcast(Lang(LangKeys.Chat, null, message));
    }
        
    public void Chat(IPlayer player, string message)
    {
        server.Broadcast(Lang(LangKeys.Chat, player, message));
    }

    public string Lang(string key, IPlayer player = null) => lang.GetMessage(key, this, player?.Id);
        
    public string Lang(string key, IPlayer player = null, params object[] args)
    {
        try
        {
            return string.Format(lang.GetMessage(key, this, player?.Id), args);
        }
        catch (Exception ex)
        {
            PrintError($"Lang Key '{key}' threw exception:\n{ex}");
            throw;
        }
    }
        
    public string Lang(string key, PlaceholderData data, IPlayer player = null)
    {
        string message = Lang(key, player);
        if (data != null)
        {
            message = _placeholders.ProcessPlaceholders(message, data);
        }

        return message;
    }
}