using System.Collections.Generic;
using DiscordRolesPlugin.Lang;
using DiscordRolesPlugin.Plugins;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Extensions;
using Oxide.Ext.Discord.Libraries.Templates;
using Oxide.Ext.Discord.Libraries.Templates.Messages;

namespace DiscordRolesPlugin.Configuration.Notifications
{
    public class PlayerNotificationSettings : BaseNotifications
    {
        [JsonProperty(PropertyName = "Send Server Message To Player")]
        public bool SendMessageToPlayer { get; set; }

        [JsonProperty(PropertyName = "Discord PM To Player")]
        public bool SendDiscordPm { get; set; }

        [JsonConstructor]
        public PlayerNotificationSettings() { }

        public PlayerNotificationSettings(PlayerNotificationSettings settings) : base(settings)
        {
            SendMessageToPlayer = settings?.SendMessageToPlayer ?? false;
            SendDiscordPm = settings?.SendDiscordPm ?? false;
        }

        public override void AddLocalizations(Dictionary<string, string> loc)
        {
            GroupAddedKey = LocalizationKey + LangKeys.Player.GroupAdded;
            GroupRemoveKey = LocalizationKey + LangKeys.Player.GroupRemoved;
            RoleAddedKey = LocalizationKey + LangKeys.Player.RoleAdded;
            RoleRemoveKey = LocalizationKey + LangKeys.Player.RoleRemoved;
            
            loc[GroupAddedKey] = "You have been added to group {discordroles.sync.group}";
            loc[GroupRemoveKey] =  "You have been removed from group {discordroles.sync.group}";
            loc[RoleAddedKey] = "You have been added to Discord Role {role.name}";
            loc[RoleRemoveKey] = "You have been removed from Discord Role {role.name}";

            DiscordMessageTemplates templates = DiscordRoles.Instance.Templates;
            templates.RegisterLocalizedTemplateAsync(DiscordRoles.Instance, GroupAddedKey, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[GroupAddedKey], DiscordColor.Success.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            templates.RegisterLocalizedTemplateAsync(DiscordRoles.Instance, GroupRemoveKey, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[GroupRemoveKey], DiscordColor.Danger.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            templates.RegisterLocalizedTemplateAsync(DiscordRoles.Instance, RoleAddedKey, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[RoleAddedKey], DiscordColor.Success.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            templates.RegisterLocalizedTemplateAsync(DiscordRoles.Instance, RoleRemoveKey, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[RoleRemoveKey], DiscordColor.Danger.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        }
    }
}