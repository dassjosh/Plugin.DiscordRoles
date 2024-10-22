using System;
using System.Linq;
using DiscordRolesPlugin.Configuration.SyncConfig;
using DiscordRolesPlugin.Enums;
using DiscordRolesPlugin.Sync;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Constants;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Libraries;
using Oxide.Ext.Discord.Logging;

namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    [HookMethod(DiscordExtHooks.OnDiscordGatewayReady)]
    private void OnDiscordGatewayReady(GatewayReadyEvent ready)
    {
        if (ready.Guilds.Count == 0)
        {
            PrintError("Your bot was not found in any discord servers. Please invite it to a server and reload the plugin.");
            return;
        }

        Guild = null;
        if (ready.Guilds.Count == 1 && !_config.GuildId.IsValid())
        {
            Guild = ready.Guilds.Values.FirstOrDefault();
        }
        
        if (Guild == null)
        {
            Guild = ready.Guilds[_config.GuildId];
            if (Guild == null)
            {
                PrintError("Failed to find a matching guild for the Discord Server Id. " +
                           "Please make sure your guild Id is correct and the bot is in the discord server.");
            }
        }

        DiscordApplication app = Client.Bot.Application;
        if (!app.HasApplicationFlag(ApplicationFlags.GatewayGuildMembersLimited) && !app.HasApplicationFlag(ApplicationFlags.GatewayGuildMembers))
        {
            PrintError($"You need to enable \"Server Members Intent\" for {Client.Bot.BotUser.Username} @ https://discord.com/developers/applications\n" +
                       $"{Name} will not function correctly until that is fixed. Once updated please reload {Name}.");
            return;
        }
            
        SubscribeAll();
        RegisterTemplates();
        RegisterCommands();
    }

    [HookMethod(DiscordExtHooks.OnDiscordGuildCreated)]
    private void OnDiscordGuildCreated(DiscordGuild guild)
    {
        if (guild.Id != Guild?.Id)
        {
            return;
        }
            
        ValidateSyncs();
        ValidateNickname();
        RegisterSyncs();
    }

    public bool ProcessSyncSetting(BaseSyncSettings data, DiscordRole botMaxRole)
    {
        if (!permission.GroupExists(data.GroupName))
        {
            Logger.Warning("Server group does not exist: '{0}'. Please create the group or correct the name in the config and reload the plugin.", data.GroupName);
            return false;
        }

        DiscordRole role = Guild.Roles[data.RoleId];
        if (role == null)
        {
            Logger.Warning("Discord Role ID does not exist: '{0}'. Please fix the role ID in the config and reload the plugin", data.RoleId);
            return false;
        }

        data.Role = role;
        if (botMaxRole != null && role.Position > botMaxRole.Position)
        {
            Logger.Warning("Discord Role '{0}' has a role position of {1} which is higher than the highest bot role {2} with position {3}. The bot will not be able to grant this role until this is fixed.", role.Name, role.Position, botMaxRole.Name, botMaxRole.Position);
        }

        if (!ProcessRoles.Contains(data.RoleId))
        {
            ProcessRoles.Add(data.RoleId);
        }

        return true;
    }

    [HookMethod(DiscordExtHooks.OnDiscordGuildMembersLoaded)]
    private void OnDiscordGuildMembersLoaded(DiscordGuild guild)
    {
        if (guild.Id != Guild?.Id)
        {
            return;
        }

        if (_config.EventSettings.IsAnyEnabled(SyncEvent.PluginLoaded))
        {
            timer.In(5f, CheckLinkedPlayers);
            Logger.Debug("{0} Members have been loaded. Starting processing in 5 seconds.", Guild.Name);
        }
            
        Puts($"{Title} Ready");
    }

    [HookMethod(DiscordExtHooks.OnDiscordPlayerLinked)]
    private void OnDiscordPlayerLinked(IPlayer player, DiscordUser user)
    {
        Logger.Debug($"{nameof(OnDiscordPlayerLinked)} Added Player: {{0}}({{1}}) Discord: {{2}}({{3}}) to be processed", player.Name, player.Id, user.FullUserName, user.Id);
        ProcessChange(player, SyncEvent.PlayerLinkedChanged);
    }

    [HookMethod(DiscordExtHooks.OnDiscordPlayerUnlinked)]
    private void OnDiscordPlayerUnlinked(IPlayer player, DiscordUser user)
    {
        Logger.Debug($"{nameof(OnDiscordPlayerUnlinked)} Added Player: {{0}}({{1}}) Discord: {{2}}({{3}}) to be processed", player.Name, player.Id, user.FullUserName, user.Id);
        ProcessLeaving(player.Id, user.Id, SyncEvent.PlayerLinkedChanged);
    }

    [HookMethod(DiscordExtHooks.OnDiscordGuildMemberAdded)]
    private void OnDiscordGuildMemberAdded(GuildMemberAddedEvent member)
    {
        if (member.GuildId != Guild.Id)
        {
            return;
        }

        Logger.Debug($"{nameof(OnDiscordGuildMemberAdded)} Added {{0}}({{1}}) to be processed", member.User.FullUserName, member.Id);
        HandleDiscordChange(member, SyncEvent.DiscordServerJoinLeave);
    }

    [HookMethod(DiscordExtHooks.OnDiscordGuildMemberRemoved)]
    private void OnDiscordGuildMemberRemoved(GuildMember member, DiscordGuild guild)
    {
        if (guild.Id != Guild.Id)
        {
            return;
        }

        Logger.Debug($"{nameof(OnDiscordGuildMemberRemoved)} Added {{0}}({{1}}) to be processed", member.User.FullUserName, member.Id);
        HandleDiscordChange(Guild.GetMember(member.User.Id, true), SyncEvent.DiscordServerJoinLeave);
    }

    [HookMethod(DiscordExtHooks.OnDiscordGuildMemberNicknameUpdated)]
    private void OnDiscordGuildMemberNicknameUpdated(GuildMember member, string oldNickname, string newNickname, DateTime? lastNicknameUpdate)
    {
        if(lastNicknameUpdate.HasValue && DateTime.UtcNow - lastNicknameUpdate.Value < TimeSpan.FromMinutes(_config.Nickname.TimeBetweenNicknameSync))
        {
            Logger.Debug($"{nameof(OnDiscordGuildMemberNicknameUpdated)} Skipped processing {{0}}({{1}}) to be processed because nickname was changed {{2:0.00}} minutes ago.", member.User.FullUserName, member.Id, (DateTime.UtcNow - lastNicknameUpdate.Value).TotalMinutes);
            return;
        }
            
        Logger.Debug($"{nameof(OnDiscordGuildMemberRemoved)} Added {{0}}({{1}}) to be processed because nickname changed: {{2}} -> {{3}}", member.User.FullUserName, member.Id, oldNickname, newNickname);
        HandleDiscordChange(member, SyncEvent.DiscordNicknameChanged);
    }
        
    [HookMethod(DiscordExtHooks.OnDiscordGuildMemberRoleAdded)]
    private void OnDiscordGuildMemberRoleAdded(GuildMember member, Snowflake roleId, DiscordGuild guild)
    {
        if (ProcessRoles.Contains(roleId))
        {
            Logger.Debug($"{nameof(OnDiscordGuildMemberRemoved)} Added {{0}}({{1}}) to be processed because {{2}} role added.", member.User.FullUserName, member.Id, guild.Roles[roleId]?.Name ?? "Unknown Role");
            HandleDiscordChange(member, SyncEvent.DiscordRoleChanged);
        }
    }
        
    [HookMethod(DiscordExtHooks.OnDiscordGuildMemberRoleRemoved)]
    private void OnDiscordGuildMemberRoleRemoved(GuildMember member, Snowflake roleId, DiscordGuild guild)
    {
        if (ProcessRoles.Contains(roleId))
        {
            Logger.Debug($"{nameof(OnDiscordGuildMemberRemoved)} Added {{0}}({{1}}) to be processed because {{2}} role removed.", member.User.FullUserName, member.Id, guild.Roles[roleId]?.Name ?? "Unknown Role");
            HandleDiscordChange(member, SyncEvent.DiscordRoleChanged);
        }
    }
        
    public void HandleDiscordChange(GuildMember member, SyncEvent syncEvent)
    {
        DiscordUser user = member.User;
        Snowflake userId = user.Id;
        PlayerId playerId = _link.GetPlayerId(member.Id);
        if (!playerId.IsValid)
        {
            Logger.Debug("Skipping {0} Sync for {1}({2}). Player is not linked", syncEvent, user.FullUserName, userId);
            return;
        }

        IPlayer player = playerId.Player;
        if (player == null)
        {
            Logger.Debug("Skipping {0} Sync for {1}({2}). No IPlayer found", syncEvent, user.FullUserName, userId);
            return;
        }
            
        ProcessQueue.RemoveAll(p => p.MemberId == userId && !p.IsLeaving);
        QueueSync(new PlayerSyncRequest(player, member, syncEvent, false));
    }
}