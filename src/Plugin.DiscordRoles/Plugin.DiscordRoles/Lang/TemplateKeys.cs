using Oxide.Ext.Discord.Libraries;

namespace DiscordRolesPlugin.Lang;

public class TemplateKeys
{
    public static class Commands
    {
        private const string Base = nameof(Commands) + ".";
            
        public static class User
        {
            private const string Base = Commands.Base + nameof(User) + ".";

            public static readonly TemplateKey Queued = new(Base + nameof(Queued));

            public static class Errors
            {
                private const string Base = User.Base + nameof(Errors) + ".";
                    
                public static readonly TemplateKey NotLinked = new(Base + nameof(NotLinked));
            }
        }
            
        public static class Player
        {
            private const string Base = Commands.Base + nameof(Player) + ".";

            public static readonly TemplateKey Queued = new(Base + nameof(Queued));

            public static class Errors
            {
                private const string Base = Player.Base + nameof(Errors) + ".";
                    
                public static readonly TemplateKey NotLinked = new(Base + nameof(NotLinked));
            }
        }
    }
}