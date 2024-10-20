using System.Collections.Generic;
using DiscordRolesPlugin.Lang;
using DiscordRolesPlugin.Placeholders;
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

    public override void Initialize()
    {
        GroupAddedKey = LocalizationKey + LangKeys.Announcements.GroupAdded;
        GroupRemoveKey = LocalizationKey + LangKeys.Announcements.GroupRemoved;
        RoleAddedKey = LocalizationKey + LangKeys.Announcements.RoleAdded;
        RoleRemoveKey = LocalizationKey + LangKeys.Announcements.RoleRemoved;
            
        GroupAddedTemplate = new TemplateKey(GroupAddedKey);
        GroupRemoveTemplate = new TemplateKey(GroupRemoveKey);
        RoleAddedTemplate = new TemplateKey(RoleAddedKey);
        RoleRemoveTemplate = new TemplateKey(RoleRemoveKey);
    }

    public override void AddLocalizations(Dictionary<string, string> loc)
    {
        loc[GroupAddedKey] = $"{DefaultKeys.Player.Name} has been added to server group {PlaceholderKeys.Group}";
        loc[GroupRemoveKey] = $"{DefaultKeys.Player.Name} has been removed from server group {PlaceholderKeys.Group}";
        loc[RoleAddedKey] = $"{DefaultKeys.Player.Name} has been added to discord role {PlaceholderKeys.Group}";
        loc[RoleRemoveKey] = $"{DefaultKeys.Player.Name} has been removed from discord role {PlaceholderKeys.Group}";

        DiscordMessageTemplates templates = DiscordRoles.Instance.Templates;
        templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, GroupAddedTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[GroupAddedKey], DiscordColor.Success), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, GroupRemoveTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[GroupRemoveKey], DiscordColor.Danger), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, RoleAddedTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[RoleAddedKey], DiscordColor.Success), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        templates.RegisterGlobalTemplateAsync(DiscordRoles.Instance, RoleRemoveTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[RoleRemoveKey], DiscordColor.Danger), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
    }
}