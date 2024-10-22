using System;
using System.Collections.Generic;
using DiscordRolesPlugin.Configuration;
using DiscordRolesPlugin.Data;
using DiscordRolesPlugin.Handlers;
using DiscordRolesPlugin.Sync;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Clients;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Ext.Discord.Libraries;
using Oxide.Ext.Discord.Types;
using Oxide.Plugins;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    // ReSharper disable once UnassignedField.Global
    public DiscordClient Client { get; set; }
    public DiscordPluginPool Pool { get; set; }
        
    [PluginReference] 
#pragma warning disable CS0649
    // ReSharper disable InconsistentNaming
    private Plugin AntiSpam, Clans;
    // ReSharper restore InconsistentNaming
#pragma warning restore CS0649

    public PluginConfig _config;
    public PluginData Data;
        
    public DiscordGuild Guild;

    private const string AccentColor = "#de8732";

    private readonly DiscordLink _link = GetLibrary<DiscordLink>();
    private readonly DiscordPlaceholders _placeholders = GetLibrary<DiscordPlaceholders>();
    public readonly DiscordMessageTemplates Templates = GetLibrary<DiscordMessageTemplates>();
    private readonly DiscordCommandLocalizations _localizations = GetLibrary<DiscordCommandLocalizations>();

    public ILogger Logger;

    public readonly List<BaseHandler> SyncHandlers = new();
    public readonly List<Snowflake> ProcessRoles = new();
    public readonly List<PlayerSyncRequest> ProcessQueue = new();
    public readonly Hash<string, RecentSyncData> RecentSync = new();
        
    public static DiscordRoles Instance;
}