using Newtonsoft.Json;

namespace DiscordRolesPlugin.Configuration;

public class ConflictSettings
{
    public int GroupConflictLimit { get; set; }
    public int RoleConflictLimit { get; set; }

    [JsonConstructor]
    public ConflictSettings() { }

    public ConflictSettings(ConflictSettings settings)
    {
            GroupConflictLimit = settings?.GroupConflictLimit ?? 3;
            RoleConflictLimit = settings?.RoleConflictLimit ?? 3;
        }
}