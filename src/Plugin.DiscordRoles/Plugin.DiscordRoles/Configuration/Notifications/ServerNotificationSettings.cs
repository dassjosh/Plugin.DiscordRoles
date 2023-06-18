using System.Collections.Generic;
using DiscordRolesPlugin.Lang;
using DiscordRolesPlugin.Plugins;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Extensions;
using Oxide.Ext.Discord.Libraries.Templates;
using Oxide.Ext.Discord.Libraries.Templates.Messages;

namespace DiscordRolesPlugin.Configuration.Notifications
{
    public class ServerNotificationSettings : BaseNotifications
    {
        [JsonProperty(PropertyName = "Send Message To Game Server")]
        public bool SendMessageToServer { get; set; }

        [JsonProperty(PropertyName = "Discord Message Channel ID")]
        public Snowflake DiscordMessageChannelId { get; set; }

        [JsonConstructor]
        public ServerNotificationSettings() { }

        public ServerNotificationSettings(ServerNotificationSettings settings) : base(settings)
        {
            SendMessageToServer = settings?.SendMessageToServer ?? false;
            DiscordMessageChannelId = settings?.DiscordMessageChannelId ?? default(Snowflake);
        }

        public override void AddLocalizations(Dictionary<string, string> loc)
        {
            GroupAddedKey = LocalizationKey + LangKeys.Announcements.GroupAdded;
            GroupRemoveKey = LocalizationKey + LangKeys.Announcements.GroupRemoved;
            RoleAddedKey = LocalizationKey + LangKeys.Announcements.RoleAdded;
            RoleRemoveKey = LocalizationKey + LangKeys.Announcements.RoleRemoved;
            
            loc[GroupAddedKey] = "{player.name} has been added to server group {group.name}";
            loc[GroupRemoveKey] = "{player.name} has been removed to server group {group.name}";
            loc[RoleAddedKey] = "{player.name} has been added to discord role {role.name}";
            loc[RoleRemoveKey] = "{player.name} has been removed to discord role {role.name}";

            DiscordMessageTemplates templates = DiscordRoles.Instance.Templates;
            templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, GroupAddedKey, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[GroupAddedKey], DiscordColor.Success.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, GroupRemoveKey, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[GroupRemoveKey], DiscordColor.Danger.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, RoleAddedKey, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[RoleAddedKey], DiscordColor.Success.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, RoleRemoveKey, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[RoleRemoveKey], DiscordColor.Danger.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        }
    }
}