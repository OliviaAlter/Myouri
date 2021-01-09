using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DatabaseEntity;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Extension;
using DiscordBot.Utilities;

namespace DiscordBot.Services
{
    public class CommandHandler : InitializedService
    {
        //private readonly LavaNode _lavaNode;
        public static List<MuteExtension> Mutes = new List<MuteExtension>();
        private readonly AutoRolesHelperClass _autoRolesHelperClass;
        private readonly DiscordSocketClient _client;
        private readonly ImageService _imageService;
        private readonly IServiceProvider _provider;
        private readonly Servers _servers;
        private readonly CommandService _service;

        //,LavaNode lavaNode)

        public CommandHandler(IServiceProvider provider,
            DiscordSocketClient client, CommandService service, Servers servers,
            AutoRolesHelperClass autoRolesHelperClass, ImageService imageService)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _servers = servers;
            _autoRolesHelperClass = autoRolesHelperClass;
            _imageService = imageService;
            //_lavaNode = lavaNode;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.JoinedGuild += OnJoinedGuild;

            _client.MessageReceived += OnMessageReceived;

            _client.MessageDeleted += OnMessageDeleted;
            _client.MessageUpdated += OnMessageUpdated;
            //_client.GuildMemberUpdated += OnGuildMemberUpdated;

            _client.UserJoined += OnUserJoined;
            _client.UserLeft += OnUserLeft;

            var muteTask = new Task(async () => await MuteHandler());
            muteTask.Start();

            _client.UserIsTyping += OnUserIsTyping;
            _client.UserBanned += OnUserBanned;
            _client.UserUnbanned += OnUserUnbanned;

            _client.RoleCreated += OnRoleCreated;
            _client.RoleUpdated += OnRoleUpdated;
            _client.RoleDeleted += OnRoleDeleted;

            _client.ChannelCreated += OnChannelCreated;
            _client.ChannelUpdated += OnChannelUpdated;
            _client.ChannelDestroyed += OnChannelDestroyed;

            _client.UserVoiceStateUpdated += OnUserVoiceStateChanged;

            _service.CommandExecuted += OnCommandExecuted;

            _client.Ready += OnReady;
            //await _client.GetRecommendedShardCountAsync();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

      

        private static async Task OnUserIsTyping(SocketUser u, ISocketMessageChannel m)
        {
            var random = new Random().Next(0, 10);
            Console.WriteLine(random);
            var username = u.Username;
            if (random == 5) await m.SendMessageAsync($"Watcha typing there, {username}?");
        }

        protected async Task MuteHandler()
        {
            var remove = new List<MuteExtension>();
            foreach (var mute in Mutes)
            {
                if (DateTime.Now < mute.EndTime) continue;

                var guild = _client.GetGuild(mute.Guild.Id);
                var role = guild.GetRole(mute.Role.Id);
                var user = guild.GetUser(mute.User.Id);

                if (guild.GetRole(mute.Role.Id) == null)
                {
                    remove.Add(mute);
                    continue;
                }

                if (guild.GetUser(mute.User.Id) == null)
                {
                    remove.Add(mute);
                    continue;
                }

                if (role.Position > guild.CurrentUser.Hierarchy)
                {
                    remove.Add(mute);
                    continue;
                }

                await user.RemoveRoleAsync(mute.Role);
                remove.Add(mute);
            }

            Mutes = Mutes.Except(remove).ToList();
            await Task.Delay(1 * 60 * 1000);
            await MuteHandler();
        }

        private async Task OnGuildMemberUpdated(SocketGuildUser userInitial, SocketGuildUser userUpdated)
        {
            var guildId = await _servers.GetUserLogChannel(userInitial.Guild.Id);
            if (_client.GetChannel(guildId) is ISocketMessageChannel channelLogged)
            {
                if (!Equals(userInitial.Nickname, userUpdated.Nickname))
                {
                    Console.WriteLine($"Nickname : {userInitial.Nickname} -> {userUpdated.Nickname}");
                    await channelLogged.SendMessageAsync("Nick");
                    //await EventReplyUtils.UserNameUpdatedEmbed(userInitial, userUpdated, channelLogged);
                }

                if (!Equals(userInitial.Discriminator, userUpdated.Discriminator))
                {
                    Console.WriteLine($"Discriminator : {userInitial.Discriminator} -> {userUpdated.Discriminator}");
                    await channelLogged.SendMessageAsync("Discriminator");
                    //await EventReplyUtils.UserNameUpdatedEmbed(userInitial, userUpdated, channelLogged);
                }

                if (!Equals(userInitial.Username, userUpdated.Username))
                {
                    Console.WriteLine($"Username : {userInitial.Username} -> {userUpdated.Username}");
                    await channelLogged.SendMessageAsync("Username");
                    //await EventReplyUtils.UserNameUpdatedEmbed(userInitial, userUpdated, channelLogged);
                }

                if (!Equals(userInitial.GetAvatarUrl(), userUpdated.GetAvatarUrl()))
                    Console.WriteLine($"{userInitial} -> {userUpdated}");
                //await channelLogged.SendMessageAsync("e");
                //await EventReplyUtils.UserAvatarUpdatedEmbed(userInitial, userUpdated, channelLogged);
            }

            ;
        }

        private async Task OnUserVoiceStateChanged(SocketUser user, SocketVoiceState voiceStateBefore,
            SocketVoiceState voiceStateAfter)
        {
            var guildId = await _servers.GetUserLogChannel(((SocketGuildUser) user).Guild.Id);
            if (!(_client.GetChannel(guildId) is ISocketMessageChannel messageChannel)) return;
            if (!Equals(voiceStateBefore.VoiceChannel, voiceStateAfter.VoiceChannel) &&
                voiceStateAfter.VoiceChannel != null && voiceStateBefore.VoiceChannel == null)
                await EventExtension.UserVoiceJoined(user, voiceStateAfter.VoiceChannel, messageChannel);
            else if (!Equals(voiceStateAfter.VoiceChannel, voiceStateBefore.VoiceChannel) &&
                     voiceStateBefore.VoiceChannel != null && voiceStateAfter.VoiceChannel == null)
                await EventExtension.UserVoiceLeft(user, voiceStateBefore.VoiceChannel, messageChannel);
            else if (!Equals(voiceStateAfter.VoiceChannel, voiceStateBefore.VoiceChannel) &&
                     voiceStateBefore.VoiceChannel != null && voiceStateAfter.VoiceChannel != null)
                await EventExtension.UserVoicejumped(user, voiceStateBefore.VoiceChannel, voiceStateAfter.VoiceChannel, messageChannel);
        }

        private async Task OnChannelCreated(SocketChannel channel)
        {
            var guildId = await _servers.GetEventLogChannel(((SocketGuildChannel) channel).Guild.Id);
            if (_client.GetChannel(guildId) is ISocketMessageChannel logChannel)
                await EventExtension.ChannelCreatedEmbed(channel, logChannel);
        }

        /*
        private async Task OnChannelUpdated(SocketChannel channelBefore, SocketChannel channelAfter)
        {
            var guildId = await _servers.GetEventLogChannel(((SocketGuildChannel)channelBefore).Guild.Id);
            if (!(_client.GetChannel(guildId) is IMessageChannel logChannel)) return;
            if (((SocketGuildChannel)channelBefore).Name 
                != ((SocketGuildChannel)channelAfter).Name)
                await EventReplyUtils.ChannelNameUpdatedEmbed(channelBefore, channelAfter, logChannel);
            if (!Equals(((SocketGuildChannel)channelBefore).PermissionOverwrites, 
                ((SocketGuildChannel)channelAfter).PermissionOverwrites))
                await EventReplyUtils.ChannelPermUpdatedEmbed(channelBefore, channelAfter, logChannel);
        }
        */
        private async Task OnChannelUpdated(SocketChannel channelBefore, SocketChannel channelAfter)
        {
            var guildId = await _servers.GetEventLogChannel(((SocketGuildChannel) channelBefore).Guild.Id);
            if (!(_client.GetChannel(guildId) is ISocketMessageChannel logChannel)) return;
            // First, we want both SocketChannels to be converted to SocketGuildChannels
            var after = channelAfter as SocketGuildChannel;

            if (!(channelBefore is SocketGuildChannel before)) return;
            var differences =
                after?.PermissionOverwrites.Except(before
                    .PermissionOverwrites); // Go through the PermissionOverwrites to extract only the differences
            if (differences != null)
                foreach (var difference in differences) // Loop over every difference
                {
                    Console.WriteLine($"\n{difference.Permissions} test\n");
                    Console.WriteLine(
                        $"{difference.TargetType}: {difference.TargetId}"); // Let's you know what the TargetType is (a User or role) followed by their ID
                    Console.WriteLine("Allowed: " +
                                      string.Join(" ",
                                          difference.Permissions
                                              .ToAllowList())); // Get all the permissions that were changed to allow
                    Console.WriteLine("Denied: " +
                                      string.Join(" ",
                                          difference.Permissions
                                              .ToDenyList())); // Get all the permissions that were changed to denied
                }

            if (((SocketGuildChannel) channelBefore).Name != ((SocketGuildChannel) channelAfter).Name)
                await EventExtension.ChannelNameUpdatedEmbed(channelBefore, channelAfter, logChannel);
        }

        private async Task OnChannelDestroyed(SocketChannel channel)
        {
            var guildId = await _servers.GetEventLogChannel(((SocketGuildChannel) channel).Guild.Id);
            if (_client.GetChannel(guildId) is ISocketMessageChannel logChannel)
                await EventExtension.ChannelDeletedEmbed(channel, logChannel);
        }

        private async Task OnRoleCreated(SocketRole role)
        {
            var guildId = await _servers.GetEventLogChannel(role.Guild.Id);
            if (_client.GetChannel(guildId) is ISocketMessageChannel logChannel)
                await EventExtension.RoleCreatedEmbed(role, logChannel);
        }

        private async Task OnRoleUpdated(SocketRole roleBefore, SocketRole roleAfter)
        {
            var guildId = await _servers.GetEventLogChannel(roleBefore.Guild.Id);
            if (!(_client.GetChannel(guildId) is ISocketMessageChannel logChannel)) return;
            if (!Equals(roleBefore.Permissions, roleAfter.Permissions))
                await EventExtension.RolePermUpdatedEmbed(roleBefore, roleAfter, logChannel);
            if (!Equals(roleBefore.Name, roleAfter.Name))
                await EventExtension.RoleNameUpdatedEmbed(roleBefore, roleAfter, logChannel);
            if (!Equals(roleBefore.Color, roleAfter.Color))
                await EventExtension.RoleColorUpdatedEmbed(roleBefore, roleAfter, logChannel);
        }

        private async Task OnRoleDeleted(SocketRole role)
        {
            var guildId = await _servers.GetEventLogChannel(role.Guild.Id);
            if (_client.GetChannel(guildId) is ISocketMessageChannel logChannel)
                await EventExtension.RoleRemovedEmbed(role, logChannel);
        }

        private async Task OnUserJoined(SocketGuildUser joinUser)
        {
            var newTask = new Task(async () => await UserJoinedHandler(joinUser));
            newTask.Start();
        }

        private async Task UserJoinedHandler(SocketGuildUser joinUser)
        {
            var guildId = await _servers.GetWelcomeChannel(joinUser.Guild.Id);
            var path = await _imageService.CreateImageAsync(joinUser);

            if (_client.GetChannel(guildId) is ISocketMessageChannel welcomeChannel && welcomeChannel.Id != 0)
            {
                await welcomeChannel.SendFileAsync(path);
                File.Delete(path);
            }

            var roles = await _autoRolesHelperClass.GetAutoRolesAsync(joinUser.Guild);
            if (roles.Count < 1) return;
            await joinUser.AddRolesAsync(roles);
        }

        private async Task OnUserLeft(SocketGuildUser leftUser)
        {
            var guildId = await _servers.GetLeftChannel(leftUser.Guild.Id);
            if (_client.GetChannel(guildId) is ISocketMessageChannel leftChannel)
            {
                await EventExtension.UserLeftEmbed(leftUser, leftChannel);
                await leftChannel.SendMessageAsync($"Good bye {leftUser.Mention}, we will miss you!!");
            }
        }

        private async Task OnUserUnbanned(SocketUser userBanned, SocketGuild guild)
        {
            var guildId = await _servers.GetEventLogChannel(guild.Id);
            if (_client.GetChannel(guildId) is ISocketMessageChannel logChannel)
                await logChannel.SendMessageAsync($"{userBanned.Mention} has been unbanned from {guild.Name} " +
                                                  $"at {DateTime.Now}!");
        }

        private async Task OnUserBanned(SocketUser userBanned, SocketGuild guild)
        {
            var guildId = await _servers.GetEventLogChannel(guild.Id);
            var banAsync = await guild.GetBanAsync(userBanned);
            var reason = banAsync.Reason;
            if (_client.GetChannel(guildId) is ISocketMessageChannel logChannel)
                await logChannel.SendMessageAsync($"{userBanned.Mention} has been banned from {guild.Name} " +
                                                  $"at {DateTime.Now} with reason : {reason}!");
        }

        private async Task OnMessageUpdated(Cacheable<IMessage, ulong> messageCache, SocketMessage messageAfter,
            ISocketMessageChannel messageChannel)
        {
            if (!(messageAfter is SocketUserMessage message)) return;

            if (message.Source != MessageSource.User) return;
            var guildId = await _servers.GetLogMessageChannel(((SocketGuildChannel) messageChannel).Guild.Id);
            var user = messageAfter.Author;
            var content = messageCache.Value.Content;
            if (_client.GetChannel(guildId) is ISocketMessageChannel logChannel
                && messageCache.HasValue
                && !messageCache.Value.IsPinned
                && !messageCache.Value.IsTTS)
                await EventExtension.MessageUpdatedEmbed(user, logChannel, messageChannel, content, messageAfter);
        }

        private async Task OnMessageDeleted(Cacheable<IMessage, ulong> messageCache,
            ISocketMessageChannel messageChannel)
        {
            if (!messageCache.HasValue) return;
            var guildId = await _servers.GetLogMessageChannel(((SocketGuildChannel) messageChannel).Guild.Id);
            var message = (SocketUserMessage) messageCache.Value;
            if (message.Source != MessageSource.User) return;
            var user = ((SocketMessage) messageCache.Value).Author;
            var content = messageCache.Value.Content;
            if (_client.GetChannel(guildId) is IMessageChannel logChannel)
                await EventExtension.MessageDeletedEmbed(user, logChannel, messageChannel, content);
        }

        private static async Task OnJoinedGuild(SocketGuild arg)
        {
            await arg.DefaultChannel.SendMessageAsync(
                "Hello, Ｍｙｏｕｒｉ is here. Thanks for inviting me to your Server!");
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;

            //if (message.Source != MessageSource.User) return;
            //string[] filterBadWords = {"gay", "noob"};

            if (!((SocketGuildChannel) message.Channel).Guild.GetUser(message.Author.Id)
                .GuildPermissions.Administrator)
            {
                if (message.Content.ToLower().Contains("https://discord.gg/"))
                {
                    await message.DeleteAsync();
                    await message.Channel.SendErrorModerationAsync("Invalid invite link",
                        $"{message.Author.Mention} has posted another discord invite link without permission",
                        message.Author);
                }
                if (message.MentionedEveryone)
                {
                    await message.DeleteAsync();
                    await message.Channel.SendErrorAsync("Warning",
                        $"{message.Author.Mention} has mentioned everyone in channel <#{message.Channel.Id}>, shame on them!");
                }

                if (message.MentionedRoles.Count >= 4)
                {
                    await message.DeleteAsync();
                    await message.Channel.SendErrorAsync("Warning",
                        $"{message.Author.Mention} has mentioned more than 4 roles in channel <#{message.Channel.Id}>, shame on them!");

                }

                if (message.MentionedUsers.Count >= 4)
                {
                    await message.DeleteAsync();
                    await message.Channel.SendErrorAsync("Warning",
                        $"{message.Author.Mention} has mentioned more than 4 people in channel <#{message.Channel.Id}>, shame on them!");
                }

                /*
                if (message.Content.Split(" ").Intersect(filterBadWords).Any())
                {
                    await message.DeleteAsync(new RequestOptions
                    {
                        AuditLogReason = "Message deleted by bot. Contains bad word"
                    });
                    await message.Channel.SendErrorModerationAsync("Bad word!",
                        $"{message.Author.Mention} has written bad word", message.Author);
                }
                */
            }

            var argPos = 0;
            var prefix = await _servers.GetGuildPrefix(((SocketGuildChannel) message.Channel).Guild.Id) ?? "*";
            if (!message.HasStringPrefix(prefix, ref argPos) &&
                !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _service.ExecuteAsync(context, argPos, _provider);
        }

        private static async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context,
            IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess)
            {
                if (!(context.Channel is ISocketMessageChannel errorChannel)) return;
                switch (result.Error)
                {
                    case CommandError.UnknownCommand:
                        await errorChannel.SendErrorAsync("Error", "Sorry I don't know that command!");
                        break;
                    case CommandError.BadArgCount:
                        await errorChannel.SendErrorAsync("Error", "You need to provide arguments!");
                        break;
                    case CommandError.Exception:
                        await errorChannel.SendErrorAsync("Error",
                            "An exception happened while executing your command!");
                        break;
                    case CommandError.MultipleMatches:
                        await errorChannel.SendErrorAsync("Error",
                            "Look like there is something has the same name as your wish");
                        break;
                    case CommandError.ObjectNotFound:
                        await errorChannel.SendErrorAsync("Error",
                            "There was an issue finding the information provided! Please make sure it is correct!");
                        break;
                    case CommandError.ParseFailed:
                        await errorChannel.SendErrorAsync("Error",
                            "Failed to convert the data type you input!");
                        break;
                    case CommandError.UnmetPrecondition:
                        await errorChannel.SendErrorAsync("Error",
                            "Look like you or the bot don't have the permission to do that!");
                        break;
                    case CommandError.Unsuccessful:
                        await errorChannel.SendErrorAsync("Error", "The command failed to execute!");
                        break;
                    case null:
                        await errorChannel.SendErrorAsync("Error",
                            "Null exception happened! This shouldn't be happening though!");
                        break;
                    default:
                        await errorChannel.SendErrorAsync("Error",
                            "Big error happened while executing your command!");
                        break;
                }
            }
        }

        private async Task OnReady()
        {
            /*
            if (!_lavaNode.IsConnected)
            {
                await _lavaNode.ConnectAsync();
            }
            */
            var gameActivity = new[]
            {
                "with Olivia, type *help for cmd",
                "with knife, type *help for cmd",
                "watching anime, type *help for cmd",
                "reading manga, type *help for cmd",
                "nothing, type *help for cmd",
                "surfing the net, type *help for cmd",
                "reading message, type *help for cmd"
            };
            await _client.SetStatusAsync(UserStatus.Idle);
            await _client.SetGameAsync($"{gameActivity[new Random().Next(gameActivity.Length - 1)]}");
        }
    }
}