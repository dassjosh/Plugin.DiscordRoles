using System.Collections.Generic;
using DiscordRolesPlugin.Configuration;
using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Enums;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities;

namespace DiscordRolesPlugin.Plugins
{
    public partial class DiscordRoles
    {
        protected override void LoadDefaultConfig()
        {
            PrintWarning("Loading Default Config");
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            Config.Settings.DefaultValueHandling = DefaultValueHandling.Populate;
            _config = AdditionalConfig(Config.ReadObject<PluginConfig>());
            Config.WriteObject(_config);
        }

        private PluginConfig AdditionalConfig(PluginConfig config)
        {
            config.EventSettings = new EventSettings(config.EventSettings);
            config.LogSettings = new LogSettings(config.LogSettings);
            config.ConflictSettings = new ConflictSettings(config.ConflictSettings);

            config.SyncSettings = config.SyncSettings ?? new List<SyncSettings>
            {
                new SyncSettings("Default", default(Snowflake), SyncMode.Server),
                new SyncSettings("VIP", default(Snowflake), SyncMode.Discord)
            };

            config.PriorityGroupSettings = config.PriorityGroupSettings ?? new List<PriorityGroupSettings>
            {
                new PriorityGroupSettings(null)
            };
            
            for (int index = 0; index < config.SyncSettings.Count; index++)
            {
                config.SyncSettings[index] = new SyncSettings(config.SyncSettings[index]);
            }
            
            for (int index = 0; index < config.PriorityGroupSettings.Count; index++)
            {
                config.PriorityGroupSettings[index] = new PriorityGroupSettings(config.PriorityGroupSettings[index]);
            }

            return config;
        }
    }
}