using System;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Extension
{
    public class MuteExtension
    {
        public DateTime EndTime;
        public SocketGuild Guild;
        public IRole Role;
        public SocketGuildUser User;
    }
}