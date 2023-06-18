using Newtonsoft.Json;

namespace DiscordRolesPlugin.Configuration.SyncConfig
{
    public class PrioritySyncSettings : BaseSyncSettings
    {
        [JsonProperty("Sync Priority", Order = 5)]
        public int Priority { get; set; }

        [JsonConstructor]
        public PrioritySyncSettings() { }

        public PrioritySyncSettings(PrioritySyncSettings settings) : base(settings)
        {
            Priority = settings?.Priority ?? 1;
        }
    }
}