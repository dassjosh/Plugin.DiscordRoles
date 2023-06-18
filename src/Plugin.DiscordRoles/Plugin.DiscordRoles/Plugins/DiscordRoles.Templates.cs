using System.Collections.Generic;
using DiscordRolesPlugin.Lang;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Extensions;
using Oxide.Ext.Discord.Libraries.Templates;
using Oxide.Ext.Discord.Libraries.Templates.Embeds;
using Oxide.Ext.Discord.Libraries.Templates.Messages;

namespace DiscordRolesPlugin.Plugins
{
    public partial class DiscordRoles
    {
        public void RegisterTemplates()
        {
            DiscordMessageTemplate queued = CreatePrefixedTemplateEmbed("Player: {player.name}({player.id}) User: {user.mention} has been queued to be synced.", DiscordColor.Success.ToHex());
            Templates.RegisterLocalizedTemplateAsync(this, TemplateKeys.Commands.Player.Queued, queued, new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            Templates.RegisterLocalizedTemplateAsync(this, TemplateKeys.Commands.User.Queued, queued, new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            
            DiscordMessageTemplate playerNotLinked = CreatePrefixedTemplateEmbed("Player: {player.name}({player.id}) is not linked and cannot be queued to be synced", DiscordColor.Danger.ToHex());
            Templates.RegisterLocalizedTemplateAsync(this, TemplateKeys.Commands.Player.Errors.NotLinked, playerNotLinked, new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
            
            DiscordMessageTemplate userNotLinked = CreatePrefixedTemplateEmbed("User: {user.mention} is not linked and cannot be queued to be synced", DiscordColor.Danger.ToHex());
            Templates.RegisterLocalizedTemplateAsync(this, TemplateKeys.Commands.User.Errors.NotLinked, userNotLinked, new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0));
        }
        
        public DiscordMessageTemplate CreatePrefixedTemplateEmbed(string description, string color)
        {
            return new DiscordMessageTemplate
            {
                Embeds = new List<DiscordEmbedTemplate>
                {
                    new DiscordEmbedTemplate
                    {
                        Description = $"[{{plugin.title}}] {description}",
                        Color = $"#{color}"
                    }
                }
            };
        }
    }
}