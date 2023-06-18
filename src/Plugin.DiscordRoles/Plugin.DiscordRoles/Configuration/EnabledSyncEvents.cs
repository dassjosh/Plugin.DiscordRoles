using System;
using DiscordRolesPlugin.Enums;
using Newtonsoft.Json;

namespace DiscordRolesPlugin.Configuration
{
    public class EnabledSyncEvents
    {
        [JsonProperty("Sync On Plugin Load")]
        public bool SyncOnPluginLoad { get; set; }

        [JsonProperty("Sync On Player Connected")]
        public bool SyncOnPlayerConnected { get; set; }

        [JsonProperty("Sync On Server Group Changed")]
        public bool SyncOnServerGroupChanged { get; set; }

        [JsonProperty("Sync On Discord Role Changed")]
        public bool SyncOnDiscordRoleChanged { get; set; }

        [JsonProperty("Sync On Discord Nickname Changed")]
        public bool SyncOnDiscordNicknameChanged { get; set; }

        [JsonProperty("Sync On Player Linked / Unlinked")]
        public bool SyncOnLinkedChanged { get; set; }

        [JsonProperty("Sync On User Join / Leave Discord Server")]
        public bool SyncOnDiscordServerJoinLeave { get; set; }

        public bool IsEnabled(SyncEvent syncEvent)
        {
            switch (syncEvent)
            {
                case SyncEvent.None:
                    return false;
                case SyncEvent.PluginLoaded:
                    return SyncOnPluginLoad;
                case SyncEvent.PlayerConnected:
                    return SyncOnPlayerConnected;
                case SyncEvent.ServerGroupChanged:
                    return SyncOnServerGroupChanged;
                case SyncEvent.DiscordRoleChanged:
                    return SyncOnDiscordRoleChanged;
                case SyncEvent.DiscordNicknameChanged:
                    return SyncOnDiscordNicknameChanged;
                case SyncEvent.PlayerLinkedChanged:
                    return SyncOnLinkedChanged;
                case SyncEvent.DiscordServerJoinLeave:
                    return SyncOnDiscordServerJoinLeave;
                default:
                    throw new ArgumentOutOfRangeException(nameof(syncEvent), syncEvent, null);
            }
        }
    }
}