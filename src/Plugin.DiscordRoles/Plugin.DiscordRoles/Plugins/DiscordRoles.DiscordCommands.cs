using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Lang;
using DiscordRolesPlugin.Sync;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Attributes;
using Oxide.Ext.Discord.Builders;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Extensions;
using Oxide.Ext.Discord.Libraries;

namespace DiscordRolesPlugin.Plugins;

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

        TemplateKey command = new("Roles");

        _localizations.RegisterCommandLocalizationAsync(this, command, loc, new TemplateVersion(1, 0, 0), new TemplateVersion(1, 0, 0)).Then(_ => { _localizations.ApplyCommandLocalizationsAsync(this, cmd, command).Then(() => { Client.Bot.Application.CreateGlobalCommand(Client, builder.Build()); }); });
    }

    public void AddPlayerSyncCommand(ApplicationCommandBuilder builder)
    {
        builder.AddSubCommand("player", "Sync Oxide Player", sub =>
        {
            sub.AddOption(CommandOptionType.String, "player", "Player to Sync", options => options.Required().AutoComplete());
        });
    }

    public void AddUserSyncCommand(ApplicationCommandBuilder builder)
    {
        builder.AddSubCommand("user", "Sync Discord User", sub =>
        {
            sub.AddOption(CommandOptionType.User, "user", "User to Sync",
                options => options.Required());
        });
    }

    [DiscordApplicationCommand("roles", "player")]
    private void HandlePlayerCommand(DiscordInteraction interaction, InteractionDataParsed parsed)
    {
        string playerId = parsed.Args.GetString("player");
        IPlayer player = players.FindPlayerById(playerId);
        DiscordUser user = player?.GetDiscordUser();
        if (user == null)
        {
            interaction.CreateTemplateResponse(Client, InteractionResponseType.ChannelMessageWithSource, TemplateKeys.Commands.Player.Errors.NotLinked, null, GetDefault(player));
            return;
        }

        QueueSync(new PlayerSyncRequest(player, user.Id, SyncEvent.None, false));
        interaction.CreateTemplateResponse(Client, InteractionResponseType.ChannelMessageWithSource, TemplateKeys.Commands.Player.Queued, null, GetDefault(player, user));
    }

    [DiscordApplicationCommand("roles", "user")]
    private void HandleUserCommand(DiscordInteraction interaction, InteractionDataParsed parsed)
    {
        DiscordUser user = parsed.Args.GetUser("user");
        IPlayer player = user?.Player;
        if (player == null)
        {
            interaction.CreateTemplateResponse(Client, InteractionResponseType.ChannelMessageWithSource, TemplateKeys.Commands.User.Errors.NotLinked, null, GetDefault(user));
            return;
        }

        QueueSync(new PlayerSyncRequest(player, user.Id, SyncEvent.None, false));
        interaction.CreateTemplateResponse(Client, InteractionResponseType.ChannelMessageWithSource, TemplateKeys.Commands.User.Queued, null, GetDefault(player, user));
    }

    [DiscordAutoCompleteCommand("roles", "player", "player")]
    private void HandleNameAutoComplete(DiscordInteraction interaction, InteractionDataOption focused)
    {
        string search = focused.GetString();
        InteractionAutoCompleteBuilder response = interaction.GetAutoCompleteBuilder();
        response.AddAllOnlineFirstPlayers(search, PlayerNameFormatter.ClanName);
        interaction.CreateResponse(Client, response);
    }
}