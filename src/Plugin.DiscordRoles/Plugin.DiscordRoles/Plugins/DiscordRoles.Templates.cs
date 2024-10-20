using System.Collections.Generic;
using DiscordRolesPlugin.Lang;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Libraries;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    public void RegisterTemplates()
    {
        DiscordMessageTemplate queued = CreatePrefixedTemplateEmbed($"Player: {DefaultKeys.Player.Name}({DefaultKeys.Player.Id}) User: {DefaultKeys.User.Mention} has been queued to be synced.", DiscordColor.Success);
        Templates.RegisterLocalizedTemplateAsync(this, TemplateKeys.Commands.Player.Queued, queued, new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        Templates.RegisterLocalizedTemplateAsync(this, TemplateKeys.Commands.User.Queued, queued, new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            
        DiscordMessageTemplate playerNotLinked = CreatePrefixedTemplateEmbed($"Player: {DefaultKeys.Player.Name}({DefaultKeys.Player.Id}) is not linked and cannot be queued to be synced", DiscordColor.Danger);
        Templates.RegisterLocalizedTemplateAsync(this, TemplateKeys.Commands.Player.Errors.NotLinked, playerNotLinked, new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            
        DiscordMessageTemplate userNotLinked = CreatePrefixedTemplateEmbed($"User: {DefaultKeys.User.Mention} is not linked and cannot be queued to be synced", DiscordColor.Danger);
        Templates.RegisterLocalizedTemplateAsync(this, TemplateKeys.Commands.User.Errors.NotLinked, userNotLinked, new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
    }
        
    public DiscordMessageTemplate CreatePrefixedTemplateEmbed(string description, DiscordColor color)
    {
        return new DiscordMessageTemplate
        {
            Embeds = new List<DiscordEmbedTemplate>
            {
                new()
                {
                    Description = $"[{DefaultKeys.Plugin.Title}] {description}",
                    Color = color.ToHex()
                }
            }
        };
    }
}