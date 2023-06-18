using System;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Lang;
using DiscordRolesPlugin.Sync;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Attributes.ApplicationCommands;
using Oxide.Ext.Discord.Builders.ApplicationCommands;
using Oxide.Ext.Discord.Builders.Interactions;
using Oxide.Ext.Discord.Builders.Interactions.AutoComplete;
using Oxide.Ext.Discord.Entities.Interactions;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Extensions;
using Oxide.Ext.Discord.Libraries.Templates;
using Oxide.Ext.Discord.Libraries.Templates.Commands;

namespace DiscordRolesPlugin.Plugins
{
    public partial class DiscordRoles
    {
        public void RegisterCommands()
        {
            ApplicationCommandBuilder builder = new ApplicationCommandBuilder("roles", "Discord Roles Command", ApplicationCommandType.ChatInput)
                .AddDefaultPermissions(PermissionFlags.Administrator);

            AddPlayerSyncCommand(builder);
            AddUserSyncCommand(builder);

            CommandCreate cmd = builder.Build();
            DiscordCommandLocalization loc = builder.BuildCommandLocalization();
            
            _localizations.RegisterCommandLocalizationAsync(this, "Roles", loc, new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0)).Then(() =>
            {
                _localizations.ApplyCommandLocalizationsAsync(this, cmd, "Roles").Then(() =>
                {
                    Client.Bot.Application.CreateGlobalCommand(Client, builder.Build());
                });
            });
        }

        public void AddPlayerSyncCommand(ApplicationCommandBuilder builder)
        {
            builder.AddSubCommand("player", "Sync Oxide Player")
                   .AddOption(CommandOptionType.String, "player", "Player to Sync")
                   .Required()
                   .AutoComplete()
                   .Build();
        }
        
        public void AddUserSyncCommand(ApplicationCommandBuilder builder)
        {
            builder.AddSubCommand("user", "Sync Discord User")
                     .AddOption(CommandOptionType.User, "user", "User to Sync")
                     .Required()
                     .Build()
                     .Build();
        }
        
        [DiscordApplicationCommand("roles", "player")]
        private void HandlePlayerCommand(DiscordInteraction interaction, InteractionDataParsed parsed)
        {
            string playerId = parsed.Args.GetString("player");
            IPlayer player = players.FindPlayerById(playerId);
            DiscordUser user = player?.GetDiscordUser();
            if (user == null)
            {
                interaction.CreateTemplateResponse(Client, this, InteractionResponseType.ChannelMessageWithSource, TemplateKeys.Commands.Player.Errors.NotLinked, null, GetDefault(player));
                return;
            }
            
            QueueSync(new PlayerSyncRequest(player, user.Id, SyncEvent.None, false));
            interaction.CreateTemplateResponse(Client, this, InteractionResponseType.ChannelMessageWithSource, TemplateKeys.Commands.Player.Queued, null, GetDefault(player, user));
        }
        
        [DiscordApplicationCommand("roles", "user")]
        private void HandleUserCommand(DiscordInteraction interaction, InteractionDataParsed parsed)
        {
            DiscordUser user = parsed.Args.GetUser("user");
            IPlayer player = user?.Player;
            if (player == null)
            {
                interaction.CreateTemplateResponse(Client, this, InteractionResponseType.ChannelMessageWithSource, TemplateKeys.Commands.User.Errors.NotLinked, null, GetDefault(user));
                return;
            }
            
            QueueSync(new PlayerSyncRequest(player, user.Id, SyncEvent.None, false));
            interaction.CreateTemplateResponse(Client, this, InteractionResponseType.ChannelMessageWithSource, TemplateKeys.Commands.User.Queued, null, GetDefault(player, user));
        }
        
        [DiscordAutoCompleteCommand("roles", "player", "player")]
        private void HandleNameAutoComplete(DiscordInteraction interaction, InteractionDataOption focused)
        {
            string search = focused.GetValue<string>();
            InteractionAutoCompleteBuilder response = interaction.GetAutoCompleteBuilder();
            response.AddAllOnlineFirstPlayers(search, PlayerNameFormatter.ClanName);
            interaction.CreateResponse(Client, response);
        }
    }
}