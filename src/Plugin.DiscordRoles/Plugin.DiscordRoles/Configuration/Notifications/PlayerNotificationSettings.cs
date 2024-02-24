using System.Collections.Generic;
using DiscordRolesPlugin.Lang;
using DiscordRolesPlugin.Placeholders;
using DiscordRolesPlugin.Plugins;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Libraries;

namespace DiscordRolesPlugin.Configuration.Notifications;

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

        GroupAddedTemplate = new TemplateKey(GroupAddedKey);
        GroupRemoveTemplate = new TemplateKey(GroupRemoveKey);
        RoleAddedTemplate = new TemplateKey(RoleAddedKey);
        RoleRemoveTemplate = new TemplateKey(RoleRemoveKey);
            
        loc[GroupAddedKey] = $"You have been added to group {PlaceholderKeys.Group}";
        loc[GroupRemoveKey] =  $"You have been removed from group {PlaceholderKeys.Group}";
        loc[RoleAddedKey] = $"You have been added to Discord Role {DefaultKeys.Role.Name}";
        loc[RoleRemoveKey] = $"You have been removed from Discord Role {DefaultKeys.Role.Name}";

        DiscordMessageTemplates templates = DiscordRoles.Instance.Templates;
        templates.RegisterLocalizedTemplateAsync(DiscordRoles.Instance, GroupAddedTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[GroupAddedKey], DiscordColor.Success.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        templates.RegisterLocalizedTemplateAsync(DiscordRoles.Instance, GroupRemoveTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[GroupRemoveKey], DiscordColor.Danger.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        templates.RegisterLocalizedTemplateAsync(DiscordRoles.Instance, RoleAddedTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[RoleAddedKey], DiscordColor.Success.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        templates.RegisterLocalizedTemplateAsync(DiscordRoles.Instance, RoleRemoveTemplate, DiscordRoles.Instance.CreatePrefixedTemplateEmbed(loc[RoleRemoveKey], DiscordColor.Danger.ToHex()), new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
    }
}