using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Utilities;

namespace DiscordBot.Modules
{
    [Summary(":b:")]
    public class ServerCommand : InteractiveBase<SocketCommandContext>
    {
        /*
        [Command("slowmode", RunMode = RunMode.Async)]
        [Summary("Apply slow mode to channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SlowMode(int interval, SocketGuildChannel channel)
        {
            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            } 
            channel ??= (SocketGuildChannel)Context.Channel;
            await ((SocketTextChannel) channel).ModifyAsync(x => x.SlowModeInterval = interval);
            await Context.Channel.SendErrorAsync($"Slow mode activated",
                $"{channel.Name}'s slow made has been set to {interval} seconds");
        }
        */
        [Command("create", RunMode = RunMode.Async)]
        [Summary("Create a new role")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task CreateRole([Remainder] string role)
        {
            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            if (Context.IsPrivate || role == null) return;
            await Context.Guild.CreateRoleAsync(role, GuildPermissions.None,
                new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()), false, false);
        }

        [Command("revoke", RunMode = RunMode.Async)]
        [Description("Revoke a role from someone")]
        [Summary("Revoke someone role. Need admin perm & bot manage role perm")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task RevokeRole(SocketUser user, SocketRole role)
        {
            if (Context.IsPrivate || role == null) return;

            var builder = new EmbedBuilder();

            builder.WithTitle("Logged Information")
                .AddField("User", $"{user.Mention}")
                .AddField("Moderator", $"{Context.User.Mention}")
                .WithDescription(
                    $"{role} does not exist from {user}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));

            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractRole(userSend, role))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            if (((SocketGuildUser) user).Roles.Contains(role))
            {
                await ((SocketGuildUser) user).RemoveRoleAsync(role);
                builder.WithDescription(
                    $"{role.Mention} has been revoke from {user.Mention} by {Context.User.Mention}");
            }

            await ReplyAsync(embed: builder.Build());
        }

        [Command("give", RunMode = RunMode.Async)]
        [Description("Give a role to someone")]
        [Summary("Grant someone role. Need admin perm & bot manage role perm")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddRole(SocketUser user, SocketRole role)
        {
            if (Context.IsPrivate || role == null) return;
            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractRole(userSend, role))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var builder = new EmbedBuilder();

            builder.WithTitle("Logged Information")
                .AddField("User", $"{user.Mention}")
                .AddField("Moderator", $"{Context.User.Mention}")
                .WithDescription(
                    $"{user.Mention} already has the role {role.Mention}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));

            if (!((SocketGuildUser) user).Roles.Contains(role))
            {
                await ((SocketGuildUser) user).AddRoleAsync(role);
                builder.WithDescription($"{user.Mention} has been granted {role.Mention} by {Context.User.Mention}");
            }

            await ReplyAsync(embed: builder.Build());
        }

        [Command("unlock", true, RunMode = RunMode.Async)]
        [Summary("unlock channel")]
        [Alias("rul")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task UnlockChannelRemote(SocketGuildChannel channel = null)
        {
            channel = channel ?? (SocketGuildChannel) Context.Channel;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var role = Context.Guild.EveryoneRole;
            await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.InheritAll);
            var builder = new EmbedBuilder()
                .WithDescription("`Channel unlocked`");
            if (!(Context.Client.GetChannel(channel.Id) is SocketTextChannel channelId)) return;
            await channelId.SendMessageAsync(null, false, builder.Build());
        }

        [Command("lock", true, RunMode = RunMode.Async)]
        [Summary("lock channel")]
        [Alias("rl")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task LockChannelRemote(SocketGuildChannel channel = null)
        {
            channel = channel ?? (SocketGuildChannel) Context.Channel;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var role = Context.Guild.EveryoneRole;
            await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.DenyAll(channel)
                .Modify(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow));
            var builder = new EmbedBuilder()
                .WithDescription("`Channel locked`");
            if (!(Context.Client.GetChannel(channel.Id) is SocketTextChannel channelId)) return;
            await channelId.SendMessageAsync(null, false, builder.Build());
        }

        [Command("purge", RunMode = RunMode.Async)]
        [Summary("Purge message from channel")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            if (amount <= 0)
            {
                await Context.Channel.SendMessageAsync("No input");
                return;
            }

            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var messages = (await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync()).ToList();
            await ((SocketTextChannel) Context.Channel).DeleteMessagesAsync(messages);

            var message =
                await ReplyAsync($"{messages.Count - 1} messages deleted successfully!");

            await Task.Delay(2500).ConfigureAwait(false);

            await message.DeleteAsync();
        }

        [Command("nomention", RunMode = RunMode.Async)]
        [Summary("Disable mention everyone / here on roles")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task DisableMention()
        {
            if (Context.User is SocketGuildUser)
            {
                foreach (var roles in Context.Guild.Roles)
                    try
                    {
                        if (roles.ToString().ToLower().Equals("muted")) continue;
                        await roles.ModifyAsync(r => r.Permissions = Utils.MemPermissions);
                    }
                    catch
                    {
                        //
                    }

                await ReplyAsync("Mention disabled on all non-powered roles");
            }
            else
            {
                await ReplyAsync("Permission denied!");
            }
        }

        [Command("noinvite", RunMode = RunMode.Async)]
        [Summary("Disable mention everyone / here on roles")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task DisableInvite()
        {
            if (Context.User is SocketGuildUser)
            {
                foreach (var roles in Context.Guild.Roles)
                    try
                    {
                        await roles.ModifyAsync(r => r.Permissions = Utils.NoInvite);
                    }
                    catch
                    {
                        //
                    }

                await ReplyAsync("Invite disabled on all non-powered roles");
            }
            else
            {
                await ReplyAsync("Permission denied!");
            }
        }

        [Command("Nuke", RunMode = RunMode.Async)]
        [Description("Clone channel and create new one")]
        [Summary("Nuke a channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task NukeChannel(SocketGuildChannel channel = null)
        {
            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            if (userSend.GuildPermissions.ManageChannels)
            {
                channel = channel ?? (SocketGuildChannel) Context.Channel;
                await ReplyAsync($"{Context.User.Mention}, do you want drop the nuke in <#{channel.Id}>?");
                if (channel == null) return;
                var oldChannel = (ITextChannel) channel;
                var guild = Context.Guild;
                var response = await NextMessageAsync(timeout: TimeSpan.FromSeconds(10));
                if (response != null)
                {
                    var yesRep = new[]
                    {
                        "y",
                        "yes"
                    };
                    var noRep = new[]
                    {
                        "n",
                        "no"
                    };
                    //if (response.ToString().ToLower().Equals("yes")) 
                    if (yesRep.Any(response.ToString().ToLower().Equals))
                    {
                        await ReplyAsync($"Nuke inbound at <#{channel.Id}> in 10s");
                        await Task.Delay(10000).ConfigureAwait(false);
                        await guild.CreateTextChannelAsync($"{channel.Name}", newChannel =>
                        {
                            newChannel.CategoryId = oldChannel.CategoryId;
                            newChannel.Topic = oldChannel.Topic;
                            newChannel.Position = oldChannel.Position;
                            newChannel.SlowModeInterval = oldChannel.SlowModeInterval;
                            newChannel.IsNsfw = oldChannel.IsNsfw;
                        });
                        await oldChannel.DeleteAsync();
                    }
                    else if (noRep.Any(response.ToString().ToLower().Equals))
                    {
                        await ReplyAsync($"Nuke aborted in <#{channel.Id}>");
                    }
                    else
                    {
                        await ReplyAsync($"{Context.User.Mention}, command timed out...");
                    }
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention}, command timed out...");
                }
            }
        }
    }
}