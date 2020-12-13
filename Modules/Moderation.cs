using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Extension;
using DiscordBot.Services;
using DiscordBot.Utilities;

namespace DiscordBot.Modules
{
    [Summary(":shield:")]
    public class Moderation : InteractiveBase<SocketCommandContext>
    {
        [Command("mv", RunMode = RunMode.Async)]
        [Description("toggle mute state")]
        [Summary("toggle mute state")]
        [Alias("vc")]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task Voice(SocketGuildUser user)
        {
            var voiceChannel = Context.Guild.VoiceChannels;
            if (voiceChannel == null) return;
            foreach (var channels in voiceChannel)
            foreach (var toggleUser in channels.Users)
            {
                if (toggleUser != user) continue;
                if (user.IsMuted)
                    await user.ModifyAsync(r => r.Mute = false);
                else
                    await user.ModifyAsync(r => r.Mute = true);
            }
        }

        [Command("dv", RunMode = RunMode.Async)]
        [Description("toggle deafen state")]
        [Summary("toggle deafen state")]
        [Alias("df")]
        [RequireBotPermission(GuildPermission.DeafenMembers)]
        public async Task Deafen(SocketGuildUser user)
        {
            var voiceChannel = Context.Guild.VoiceChannels;
            if (voiceChannel == null) return;
            foreach (var channels in voiceChannel)
            foreach (var toggleUser in channels.Users)
            {
                if (toggleUser != user) continue;
                if (user.IsDeafened)
                    await user.ModifyAsync(r => r.Deaf = false);
                else
                    await user.ModifyAsync(r => r.Deaf = true);
            }
        }

        [Command("mute", RunMode = RunMode.Async)]
        [Description("Give someone muted roles")]
        [Summary("Mute someone.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Mute(SocketGuildUser user, string duration, [Remainder] string reason = null)
        {
            if (!(Context.User is SocketGuildUser userSend)
                || !(userSend.GuildPermissions.ManageRoles
                     || user.GuildPermissions.ManageRoles
                     || Utils.CanInteractUser(userSend, user)))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var mutedRole = Context.Guild.Roles.FirstOrDefault(t => t.Name.ToLower().Equals("muted"));
            if (mutedRole == null)
            {
                var roleCreation =
                    await Context.Guild.CreateRoleAsync("Muted", Utils.MutedPermissions, null, false, true);
                mutedRole = Context.Guild.Roles.FirstOrDefault(x => x.Id == roleCreation.Id);
            }

            if (mutedRole != null)
                foreach (var channel in Context.Guild.Channels) // Loop over all channels
                    if (!channel.GetPermissionOverwrite(mutedRole).HasValue ||
                        channel.GetPermissionOverwrite(mutedRole).Value.SendMessages == PermValue.Allow ||
                        channel.GetPermissionOverwrite(mutedRole).Value.AddReactions == PermValue.Allow ||
                        channel.GetPermissionOverwrite(mutedRole).Value.Connect == PermValue.Allow ||
                        channel.GetPermissionOverwrite(mutedRole).Value.Speak == PermValue.Allow)
                        await channel.AddPermissionOverwriteAsync(mutedRole,
                            new OverwritePermissions(sendMessages: PermValue.Deny, addReactions: PermValue.Deny,
                                connect: PermValue.Deny,
                                speak: PermValue.Deny));

            if (mutedRole != null && mutedRole.Position > Context.Guild.CurrentUser.Hierarchy
            ) // Return an error when the role has a lower position than the bot
            {
                await Context.Channel.SendErrorAsync("Invalid permission",
                    $"Role is higher than {Context.Client.CurrentUser.Mention}!!");
                return;
            }

            var minute = new[]
            {
                "m",
                "min",
                "minute",
                "minutes"
            };

            var hour = new[]
            {
                "h",
                "hour",
                "hours"
            };

            var day = new[]
            {
                "d",
                "day",
                "days"
            };

            if (user.Roles.Contains(mutedRole))
            {
                await Context.Channel.SendErrorAsync("Already muted",
                    $"The user {user.Mention} is already muted within the server.");
                return;
            }

            var muteTimer = new string(duration.Where(char.IsDigit).ToArray());
            if (minute.Any(duration.Contains) && day.Any(duration.Contains) && hour.Any(duration.Contains))
            {
                await Context.Channel.SendErrorAsync("Multiple argument found!", "For example : *mute user 1d");
                return;
            }

            if (muteTimer.Length == 0) return;
            var timer = Convert.ToInt32(muteTimer);


            if (minute.Any(duration.ToLower().Equals))
                //For minute(s) mute
                CommandHandler.Mutes.Add(new MuteExtension
                {
                    Guild = Context.Guild,
                    User = user,
                    EndTime = DateTime.Now + TimeSpan.FromMinutes(timer),
                    Role = mutedRole
                });

            if (hour.Any(duration.ToLower().Equals))
                //For hour(s) mute
                CommandHandler.Mutes.Add(new MuteExtension
                {
                    Guild = Context.Guild,
                    User = user,
                    EndTime = DateTime.Now + TimeSpan.FromHours(timer),
                    Role = mutedRole
                });

            if (day.Any(duration.ToLower().Equals))
                //For day(s) mute
                CommandHandler.Mutes.Add(new MuteExtension
                {
                    Guild = Context.Guild,
                    User = user,
                    EndTime = DateTime.Now + TimeSpan.FromDays(timer),
                    Role = mutedRole
                });
            reason ??= "No reason provided";
            await user.AddRoleAsync(mutedRole);
            await Context.Channel.SendSuccessAsync("Muted", $"User : {user.Mention} has been muted" +
                                                            $"\nModerator : {userSend.Mention}" +
                                                            $"\nReason : {reason}" +
                                                            $"\nDuration : {duration}",
                new RequestOptions
                {
                    AuditLogReason = "Muted by moderator"
                });
        }

        [Command("unmute", RunMode = RunMode.Async)]
        [Description("Takeaway someone muted roles")]
        [Summary("Unmute someone.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnMute(SocketGuildUser user)
        {
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals("muted"));
            if (role == null)
            {
                await Context.Channel.SendErrorAsync("Role error", "This role simply does not exist here!");
                return;
            }

            if (!(Context.User is SocketGuildUser userSend)
                || !(userSend.GuildPermissions.KickMembers
                     || user.GuildPermissions.BanMembers
                     || user.GuildPermissions.ManageRoles))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await Context.Channel.SendErrorAsync("Hierarchy error", $"{role.Mention} is higher than bot's role!");
                return;
            }

            if (!user.Roles.Contains(role))
            {
                await Context.Channel.SendErrorAsync("Already muted", "This user already muted!");
                return;
            }

            try
            {
                await user.RemoveRoleAsync(role);
                await Context.Channel.SendSuccessAsync("Unmuted", $"User : {user.Mention}" +
                                                                  $"\nModerator : {userSend.Mention}" +
                                                                  "\nDescription : User unmuted");
            }
            catch
            {
                await Context.Channel.SendErrorAsync("Error", $"Error happened while removing {role.Mention}!");
            }
        }

        [Command("kick", RunMode = RunMode.Async)]
        [Description("Kick someone's ass")]
        [Summary("Kick someone.")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Kick(SocketUser userAccount, [Remainder] string reason = "")
        {
            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractUser(userSend, (SocketGuildUser) userAccount))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var builder = new EmbedBuilder();

            builder.WithTitle("Logged Information")
                .AddField("User", $"{userAccount.Mention}")
                .AddField("Command issued by", $"{userSend.Mention}")
                .AddField("Reason", reason ?? "No reason provided")
                .WithDescription(
                    $"You can't kick this {userAccount} from {Context.Guild.Name}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));

            if (userSend.GuildPermissions.KickMembers)
            {
                await ((SocketGuildUser) userAccount).KickAsync(reason);
                builder.WithDescription(
                        $"This User has been kicked from {Context.Guild.Name} by {Context.User.Username}!")
                    .AddField("Reason", $"{reason}");
            }

            await ReplyAsync(embed: builder.Build());
        }

        [Command("ban", RunMode = RunMode.Async)]
        [Description("Ban someone's ass")]
        [Summary("Ban someone")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketUser userAccount, [Remainder] string reason = "")
        {
            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractUser(userSend, (SocketGuildUser) userAccount))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var builder = new EmbedBuilder();

            builder.WithTitle("Logged Information")
                .AddField("User", $"{userAccount.Mention}")
                .AddField("Command issued by", $"{userSend.Mention}")
                .AddField("Reason", reason ?? "No reason provided")
                .WithDescription(
                    $"You can't ban this {userAccount} from {Context.Guild.Name}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));

            if (userSend.GuildPermissions.BanMembers)
            {
                await ((SocketGuildUser) userAccount).BanAsync(0, reason);

                builder
                    .AddField("Reason", $"{reason}")
                    .WithDescription(
                        $"This User has been banned from {Context.Guild.Name} by {Context.User.Username}!");
            }

            await ReplyAsync(embed: builder.Build());
        }

        [Command("set", RunMode = RunMode.Async)]
        [Description("Change someone nickname")]
        [Summary("Change User nickname")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task SetName(SocketUser user, [Remainder] string nickName)
        {
            if (nickName == null) return;

            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageNicknames)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            await ((SocketGuildUser) user).ModifyAsync(c => c.Nickname = nickName);
            var builder = new EmbedBuilder()
                .WithTitle("Name changed")
                .WithDescription(
                    $"{user}'s name has been changed to {nickName}!")
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));
            await ReplyAsync(embed: builder.Build());
        }
    }
}