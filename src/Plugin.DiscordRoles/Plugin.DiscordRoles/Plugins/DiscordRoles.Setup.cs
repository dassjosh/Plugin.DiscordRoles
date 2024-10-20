using DiscordRolesPlugin.Data;
using Oxide.Core;
using Oxide.Ext.Discord.Connections;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    private void Init()
    {
        Instance = this;
        Logger = DiscordLoggerFactory.Instance.CreateLogger(this, _config.LogSettings.PluginLogLevel, _config.LogSettings);
        Data = Interface.Oxide.DataFileSystem.ReadObject<PluginData>(Name);
        _processNextCallback = ProcessNextStartupId;
        UnsubscribeAll();
        RegisterLang();
    }
        
    private void OnServerInitialized()
    {
        if (string.IsNullOrEmpty(_config.DiscordApiKey))
        {
            PrintWarning("Please enter your bot token in the config and reload the plugin.");
            return;
        }
            
        if (_config.Nickname.UseAntiSpam)
        {
            if (AntiSpam is not { IsLoaded: true })
            {
                PrintWarning("AntiSpam is enabled in the config but is not loaded. " +
                             "Please disable the setting in the config or load AntiSpam: https://umod.org/plugins/anti-spam");
            }
            else if (AntiSpam.Version < new VersionNumber(2, 1, 3))
            {
                PrintWarning("AntiSpam requires version >= 2.1.3. " +
                             "Please update the plugin @ https://umod.org/plugins/anti-spam");
            }
        }

        RegisterPlaceholders();

        Client.Connect(new BotConnection
        {
            ApiToken = _config.DiscordApiKey,
            LogLevel = _config.ExtensionDebugging,
            Intents = GatewayIntents.Guilds | GatewayIntents.GuildMembers
        });
    }

    private void OnServerSave() => SaveData();

    private void Unload()
    {
        SaveData(true);
        Instance = null;
    }
}