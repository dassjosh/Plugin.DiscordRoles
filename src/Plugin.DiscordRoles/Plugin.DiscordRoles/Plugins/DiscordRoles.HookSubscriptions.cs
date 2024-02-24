namespace DiscordRolesPlugin.Plugins;

public partial class DiscordRoles
{
    public void SubscribeAll()
    {
        SubscribeOxideAll();
        SubscribeDiscordAll();
    }
        
    public void UnsubscribeAll()
    {
        UnsubscribeOxideAll();
        UnsubscribeDiscordAll();
    }

    public void SubscribeOxideAll()
    {
        try
        {
            Subscribe(nameof(OnUserConnected));
            Subscribe(nameof(OnUserGroupAdded));
            Subscribe(nameof(OnUserGroupRemoved));
            Subscribe(nameof(OnDiscordPlayerLinked));
            Subscribe(nameof(OnDiscordPlayerUnlinked));
        }
        catch
        {
                
        }
    }

    public void SubscribeDiscordAll()
    {
        if (Client.Bot != null)
        {
            Subscribe(nameof(OnDiscordGuildMemberNicknameUpdated));
            Subscribe(nameof(OnDiscordGuildMemberRoleAdded));
            Subscribe(nameof(OnDiscordGuildMemberRoleRemoved));
            Subscribe(nameof(OnDiscordGuildMemberAdded));
            Subscribe(nameof(OnDiscordGuildMemberRemoved));
        }
    }
        
    public void UnsubscribeOxideAll()
    {
        try
        {
            Unsubscribe(nameof(OnUserConnected));
            Unsubscribe(nameof(OnUserGroupAdded));
            Unsubscribe(nameof(OnUserGroupRemoved));
            Unsubscribe(nameof(OnDiscordPlayerLinked));
            Unsubscribe(nameof(OnDiscordPlayerUnlinked));
        }
        catch
        {
                
        }
    }

    public void UnsubscribeDiscordAll()
    {
        if (Client.Bot != null)
        {
            Unsubscribe(nameof(OnDiscordGuildMemberNicknameUpdated));
            Unsubscribe(nameof(OnDiscordGuildMemberRoleAdded));
            Unsubscribe(nameof(OnDiscordGuildMemberRoleRemoved));
            Unsubscribe(nameof(OnDiscordGuildMemberAdded));
            Unsubscribe(nameof(OnDiscordGuildMemberRemoved));
        }
    }
}