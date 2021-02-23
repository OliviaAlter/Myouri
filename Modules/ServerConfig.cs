using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseEntity;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Extension;
using DiscordBot.Utilities;

namespace DiscordBot.Modules
{
    [Summary(":b:")]
    public class ServerConfig : ModuleBase<SocketCommandContext>
    {
        private readonly Autoroles _autoRoles;
        private readonly AutoRolesHelperClass _autoRolesHelperClass;
        private readonly Ranks _ranks;
        private readonly RanksHelperClass _ranksHelperClass;
        private readonly Servers _servers;

        public ServerConfig(Ranks ranks, RanksHelperClass ranksHelperClass,
            AutoRolesHelperClass rolesHelperClassHelper, Servers servers,
            Autoroles autoRoles)
        {
            _autoRoles = autoRoles;
            _ranks = ranks;
            _ranksHelperClass = ranksHelperClass;
            _autoRolesHelperClass = rolesHelperClassHelper;
            _servers = servers;
        }

        [Command("roles", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task AutoRoles()
        {
            var autoRoles = await _autoRolesHelperClass.GetAutoRolesAsync(Context.Guild);
            if (autoRoles.Count == 0)
            {
                await ReplyAsync("This Server does not yet have any auto roles!!!");
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            var description = autoRoles.Aggregate("This message lists all auto roles. \n"
                                                  + "In order to remove auto roles, you can use the name or ID of Rank!",
                (current, autoRole) => current + $"\n{autoRole.Mention} ({autoRole.Id})");

            await ReplyAsync(description);
        }

        [Command("arole", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task AddAutoRoles(SocketRole name)
        {
            await Context.Channel.TriggerTypingAsync();
            var autoRoles = await _autoRolesHelperClass.GetAutoRolesAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Id == name.Id);
            if (role == null)
            {
                await ReplyAsync("Role does not exist!!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("Role is higher than bot's!!!!");
                return;
            }

            if (autoRoles.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("Role is already set as auto roles!");
                return;
            }

            await _autoRoles.AddAutoRolesAsync(Context.Guild.Id, role.Id);
            await RoleExtension.AddAutoRoleEmbed(Context.Channel, role);
        }

        [Command("drole", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task DelAutoRoles(SocketRole name)
        {
            await Context.Channel.TriggerTypingAsync();
            var autoRoles = await _autoRolesHelperClass.GetAutoRolesAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Id == name.Id);
            if (role == null)
            {
                await ReplyAsync("Role does not exist!!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("Role is higher than bot's!!!!");
                return;
            }

            if (autoRoles.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("That role isn't set as auto role yet!");
                return;
            }

            await _autoRoles.RemoveAutoRolesAsync(Context.Guild.Id, role.Id);
            await RoleExtension.RemoveAutoRoleEmbed(Context.Channel, role);
        }

        [Command("Rank", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Rank([Remainder] string identifier)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _ranksHelperClass.GetRankAsync(Context.Guild);

            IRole role;

            if (ulong.TryParse(identifier, out var roleId))
            {
                var roleById = Context.Guild.Roles.FirstOrDefault(x => x.Id == roleId);
                if (roleById == null)
                {
                    await ReplyAsync("That role does not exist!");
                    return;
                }

                role = roleById;
            }
            else
            {
                var roleByName = Context.Guild.Roles.FirstOrDefault(x =>
                    string.Equals(x.Name, identifier, StringComparison.CurrentCultureIgnoreCase));
                if (roleByName == null)
                {
                    await ReplyAsync("That role does not exist!");
                    return;
                }

                role = roleByName;
            }

            if (ranks.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("That Rank does not exist!");
                return;
            }

            if (((SocketGuildUser) Context.User).Roles.Any(x => x.Id == role.Id))
            {
                await ((SocketGuildUser) Context.User).RemoveRoleAsync(role);
                await ReplyAsync($"Successfully removed the Rank {role.Mention} from you.");
                return;
            }

            await ((SocketGuildUser) Context.User).AddRoleAsync(role);
            await ReplyAsync($"Successfully added the Rank {role.Mention} to you.");
        }

        [Command("Ranks", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task Ranks()
        {
            var ranks = await _ranksHelperClass.GetRankAsync(Context.Guild);
            if (ranks.Count == 0)
            {
                await ReplyAsync("This Server does not yet have any role in the roles list!!!");
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            var description = ranks.Aggregate("This message lists all available roles to choose. \n"
                                              + "In order to add a role to this list, you can use the name or ID of its!",
                (current, rank) => current + $"\n{rank.Mention} ({rank.Id})");

            await ReplyAsync(description);
        }

        [Command("arank", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task AddRank(SocketRole name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _ranksHelperClass.GetRankAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Id == name.Id);
            if (role == null)
            {
                await ReplyAsync("Role does not exist!!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("Role is higher than bot's!!!!");
                return;
            }

            if (ranks.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("Role is already in the role list!");
                return;
            }

            await _ranks.AddRankAsync(Context.Guild.Id, role.Id);
            await RoleExtension.AddRankRoleEmbed(Context.Channel, role);
        }

        [Command("drank", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task DelRank(SocketRole name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _ranksHelperClass.GetRankAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x =>
                x.Id == name.Id);

            if (role == null)
            {
                await ReplyAsync("Role does not exist!!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("Role is higher than bot's!!!!");
                return;
            }

            if (ranks.Any(roleRank => roleRank.Id == role.Id))
            {
                await _ranks.RemoveRankAsync(Context.Guild.Id, role.Id);
                await RoleExtension.RevokeRankRoleEmbed(Context.Channel, role);
                return;
            }

            await ReplyAsync("Role is not set in roles list!");
        }

        [Command("prefix")]
        [Summary("change Server prefix")]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task PrefixChange(string prefix = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            if (prefix == null)
            {
                var guildPrefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? "*";
                await ReplyAsync($"The current prefix of the bot is [`{guildPrefix}`]!");
                return;
            }

            if (prefix.Length > 4)
            {
                await ReplyAsync("The length of prefix is longer than 4 characters!!");
                return;
            }

            await _servers.ModifyGuildPrefix(Context.Guild.Id, prefix);
            await ReplyAsync($"The prefix has been adjusted to [`{prefix}`]!");
        }

        [Command("lchannel")]
        [Summary("set message log channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetMessageLogChannel(SocketGuildChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel ??=  (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetLogMessageChannel(Context.Guild.Id);
            if (channelLog == 0)
                await _servers.SetLogMessageChannel(Context.Guild.Id, channel.Id);
            else
                await _servers.ModifyMessageLogChannel(Context.Guild.Id, channel.Id);
            await ReplyAsync($"Channel <#{channel.Id}> has been set as message log");
        }

        [Command("rmlog")]
        [Summary("remove message log channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task RemoveMessageLogChannel(SocketGuildChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel ??= (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetLogMessageChannel(Context.Guild.Id);
            if (channelLog == 0)
            {
                await ReplyAsync("This Server doesn't have any message log channel!");
                return;
            }

            if (channel.Id != channelLog)
            {
                await ReplyAsync("That channel is not set as message log channel");
            }
            else
            {
                await _servers.RemoveLogMessageChannel(Context.Guild.Id);
                await ReplyAsync($"Removed channel <#{channel.Id}> as message log channel!");
            }
        }

        [Command("echannel")]
        [Summary("set event log channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetEventLogChannel(SocketGuildChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel ??= (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetEventLogChannel(Context.Guild.Id);
            if (channelLog == 0)
                await _servers.SetEventLogChannel(Context.Guild.Id, channel.Id);
            else
                await _servers.ModifyEventLogChannel(Context.Guild.Id, channel.Id);
            await ReplyAsync($"Channel <#{channel.Id}> has been set as event log");
        }

        [Command("relog")]
        [Summary("remove event log channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task RemoveEventLogChannel(SocketGuildChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel ??= (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetEventLogChannel(Context.Guild.Id);
            if (channelLog == 0)
            {
                await ReplyAsync("This Server doesn't have any event log channel!");
                return;
            }

            if (channel.Id != channelLog)
            {
                await ReplyAsync("That channel is not set as event log channel");
            }
            else
            {
                await _servers.RemoveEventLogChannel(Context.Guild.Id);
                await ReplyAsync($"Removed channel <#{channel.Id}> as event log channel!");
            }
        }

        [Command("ulog")]
        [Summary("set User log channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task SetUserLogChannel(SocketGuildChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel ??= (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetUserLogChannel(Context.Guild.Id);
            if (channelLog == 0)
                await _servers.SetUserLogChannel(Context.Guild.Id, channel.Id);
            else
                await _servers.ModifyUserLogChannel(Context.Guild.Id, channel.Id);
            await ReplyAsync($"Channel <#{channel.Id}> has been set as User log");
        }

        [Command("rulog")]
        [Summary("remove User log channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task RemoveUserLogChannel(SocketGuildChannel channel = null)
        {
            if (!(Context.Channel is SocketGuildChannel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            channel ??= (SocketGuildChannel) Context.Channel;
            var channelLog = await _servers.GetEventLogChannel(Context.Guild.Id);
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
                await _servers.RemoveUserLogChannel(Context.Guild.Id);
                await ReplyAsync($"Removed channel <#{channel.Id}> as User log channel!");
            }
        }
    }
}