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
using Oxide.Plugins;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    // ReSharper disable once UnassignedField.Global
    public DiscordClient Client { get; set; }
        
    [PluginReference] 
#pragma warning disable CS0649
    // ReSharper disable InconsistentNaming
    private Plugin AntiSpam, Clans;
    // ReSharper restore InconsistentNaming
#pragma warning restore CS0649

    public PluginConfig _config;
    public PluginData Data;
        
    public DiscordGuild Guild;
        
    private Timer _syncTimer;

    private const string AccentColor = "#de8732";

    private readonly DiscordLink _link = GetLibrary<DiscordLink>();
    private readonly DiscordPlaceholders _placeholders = GetLibrary<DiscordPlaceholders>();
    public readonly DiscordMessageTemplates Templates = GetLibrary<DiscordMessageTemplates>();
    private readonly DiscordCommandLocalizations _localizations = GetLibrary<DiscordCommandLocalizations>();

    public ILogger Logger;

    private readonly List<BaseHandler> _syncHandlers = new List<BaseHandler>();
    private readonly List<Snowflake> _processRoles = new List<Snowflake>();
    private readonly List<PlayerSyncRequest> _processQueue = new List<PlayerSyncRequest>();
    private readonly Hash<string, RecentSyncData> _recentSync = new Hash<string, RecentSyncData>();

    private Action _processNextCallback;
        
    public static DiscordRoles Instance;
}