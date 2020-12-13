using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseEntity;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Utilities;

namespace DiscordBot.Modules
{
    [Summary(":sa:")]
    public class ChannelSetting : InteractiveBase<SocketCommandContext>
    {
        private readonly Servers _servers;

        public ChannelSetting(Servers servers)
        {
            _servers = servers;
        }

        [Command("wch", RunMode = RunMode.Async)]
        [Summary("set welcome channel")]
        [Alias("wc")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetWelcomeChannel(SocketChannel channel, string option = null, string value = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel = channel ?? (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetWelcomeChannel(Context.Guild.Id);
            if (channelLog == 0)
                await _servers.SetWelcomeChannel(Context.Guild.Id, channel.Id);
            else
                await _servers.ModifyWelcomeChannel(Context.Guild.Id, channel.Id);
            await ReplyAsync($"Channel <#{channel.Id}> has been set as welcome channel");
        }

        [Command("rwch", RunMode = RunMode.Async)]
        [Summary("remove welcome channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task RemoveWelcomeChannel(SocketChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel = channel ?? (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetWelcomeChannel(Context.Guild.Id);
            if (channelLog == 0)
            {
                await ReplyAsync("This server doesn't have any welcome channel!");
                return;
            }

            if (channel.Id == channelLog)
            {
                await _servers.RemoveWelcomeChannel(Context.Guild.Id, channelLog);
                await ReplyAsync($"Removed channel <#{channelLog}> as welcome channel!");
            }

            if (channel.Id != channelLog)
            {
                var channelFind = Context.Guild.TextChannels
                    .FirstOrDefault(t => t.Id.Equals(channelLog));
                if (channelFind == null)
                {
                    await ReplyAsync("This server doesn't have any welcome channel!");
                    return;
                }

                await ReplyAsync($"Do you want to remove <#{channelLog}> as welcome channel ? Yes/No (Y/N)");
                var response = await NextMessageAsync(timeout: TimeSpan.FromSeconds(10));
                if (response != null)
                {
                    var answer = response.ToString().ToLower();
                    if (answer.Equals("yes") || answer.Equals("y"))
                    {
                        await _servers.RemoveWelcomeChannel(Context.Guild.Id, channelLog);
                        await ReplyAsync($"Removed channel <#{channelLog}> as welcome channel!");
                    }
                    else if (answer.Equals("no") || answer.Equals("n"))
                    {
                        await ReplyAsync("Stopped the remove process");
                    }
                    else
                    {
                        await ReplyAsync("Invalid response !");
                    }
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention} command timed out...");
                }
            }
        }

        [Command("lch", RunMode = RunMode.Async)]
        [Summary("set leave channel")]
        [Alias("lc")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetLeftChannel(SocketGuildChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel = channel ?? (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetLeftChannel(Context.Guild.Id);
            if (channelLog == 0)
                await _servers.SetLeftChannel(Context.Guild.Id, channel.Id);
            else
                await _servers.ModifyLeftChannel(Context.Guild.Id, channel.Id);
            await ReplyAsync($"Channel <#{channel.Id}> has been set as leave channel");
        }

        [Command("rlch", RunMode = RunMode.Async)]
        [Summary("remove leave channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task RemoveLeftChannel(SocketChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel = channel ?? (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetLeftChannel(Context.Guild.Id);
            if (channelLog == 0)
            {
                await ReplyAsync("This Server doesn't have any leave channel!");
                return;
            }

            if (channel.Id != channelLog)
            {
                await ReplyAsync("That channel is not set as leave channel");
            }
            else
            {
                await _servers.RemoveLeftChannel(Context.Guild.Id, channelLog);
                await ReplyAsync($"Removed channel <#{channel.Id}> as leave channel!");
            }
        }

        [Command("ulch", RunMode = RunMode.Async)]
        [Summary("set User log channel")]
        [Alias("lc")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetUserLogChannel(SocketChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel = channel ?? (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetLeftChannel(Context.Guild.Id);
            if (channelLog == 0)
                await _servers.SetLeftChannel(Context.Guild.Id, channel.Id);
            else
                await _servers.ModifyLeftChannel(Context.Guild.Id, channel.Id);
            await ReplyAsync($"Channel <#{channel.Id}> has been set as User log channel");
        }

        [Command("rulch", RunMode = RunMode.Async)]
        [Summary("remove User log channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task RemoveUserLogChannel(SocketChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel = channel ?? (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetLeftChannel(Context.Guild.Id);
            if (channelLog == 0)
            {
                await ReplyAsync("This Server doesn't have any User log channel!");
                return;
            }

            if (channel.Id != channelLog)
            {
                await ReplyAsync("That channel is not set as User log channel");
            }
            else
            {
                await _servers.RemoveLeftChannel(Context.Guild.Id, channelLog);
                await ReplyAsync($"Removed channel <#{channel.Id}> as User log channel!");
            }
        }
    }
}