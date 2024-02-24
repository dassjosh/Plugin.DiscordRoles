using System.Collections.Generic;
using DiscordRolesPlugin.Lang;
using DiscordRolesPlugin.Plugins;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Libraries;

namespace DiscordRolesPlugin.Configuration.Notifications;

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
            
            
        GroupAddedTemplate = new TemplateKey(GroupAddedKey);
        GroupRemoveTemplate = new TemplateKey(GroupRemoveKey);
        RoleAddedTemplate = new TemplateKey(RoleAddedKey);
        RoleRemoveTemplate = new TemplateKey(RoleRemoveKey);
            
        loc[GroupAddedKey] = "{player.name} has been added to server group {group.name}";
        loc[GroupRemoveKey] = "{player.name} has been removed to server group {group.name}";
        loc[RoleAddedKey] = "{player.name} has been added to discord role {role.name}";
        loc[RoleRemoveKey] = "{player.name} has been removed to discord role {role.name}";

        DiscordMessageTemplates templates = DiscordRoles.Instance.Templates;
        templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, GroupAddedTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[GroupAddedKey], DiscordColor.Success.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, GroupRemoveTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[GroupRemoveKey], DiscordColor.Danger.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, RoleAddedTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[RoleAddedKey], DiscordColor.Success.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, RoleRemoveTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[RoleRemoveKey], DiscordColor.Danger.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
    }
}