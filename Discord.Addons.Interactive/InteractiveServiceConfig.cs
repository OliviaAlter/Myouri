using System;

namespace DiscordBot.Discord.Addons.Interactive
{
    public class InteractiveServiceConfig
    {
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(15);
    }
}