using System;
using System.Collections.Generic;
using DiscordRolesPlugin.Configuration.Notifications;
using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Lang;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Libraries;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    public void RegisterLang()
    {
        HashSet<string> server = new();
        HashSet<string> player = new();
        Dictionary<string, string> loc = new()
        {
            [LangKeys.Chat] = $"[#BEBEBE][[{AccentColor}]{Title}[/#]] {{0}}[/#]",
            [LangKeys.ClanTag] = "[{0}] {1}"
        };

        for (int index = 0; index < _config.SyncSettings.Count; index++)
        {
            SyncSettings settings = _config.SyncSettings[index];
            HandleServerRegistration(settings.ServerNotifications, loc, server);
            HandlePlayerRegistration(settings.PlayerNotifications, loc, player);
        }

        for (int index = 0; index < _config.PriorityGroupSettings.Count; index++)
        {
            PriorityGroupSettings group = _config.PriorityGroupSettings[index];
            HandleServerRegistration(group.ServerNotifications, loc, server);
            HandlePlayerRegistration(group.PlayerNotifications, loc, player);
        }

        lang.RegisterMessages(loc, this);
    }

    public void HandleServerRegistration(ServerNotificationSettings server, Dictionary<string, string> loc, HashSet<string> registered)
    {
        server.Initialize();
        if (registered.Add(server.LocalizationKey))
        {
            server.AddLocalizations(loc);
        }
    }
    
    public void HandlePlayerRegistration(PlayerNotificationSettings player, Dictionary<string, string> loc, HashSet<string> registered)
    {
        player.Initialize();
        if (registered.Add(player.LocalizationKey))
        {
            player.AddLocalizations(loc);
        }
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