using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive.Callbacks;
using DiscordBot.Discord.Addons.Interactive.Criteria;

namespace DiscordBot.Discord.Addons.Interactive.Paginator
{
    public class PaginatedMessageCallback : IReactionCallback
    {
        private readonly PaginatedMessage _pager;

        private readonly int _pages;

        private int _currentPage = 1;

        public PaginatedMessageCallback(
            InteractiveService interactive,
            SocketCommandContext sourceContext,
            PaginatedMessage pager,
            ICriterion<SocketReaction> criterion = null)
        {
            Interactive = interactive;
            Context = sourceContext;
            Criterion = criterion ?? new EmptyCriterion<SocketReaction>();
            _pager = pager;
            _pages = _pager.Pages.Count();
        }

        private PaginatedAppearanceOptions Options => _pager.Options;

        public InteractiveService Interactive { get; }

        public IUserMessage Message { get; private set; }
        public RunMode RunMode => RunMode.Sync;

        public TimeSpan? Timeout => Options.Timeout;

        public SocketCommandContext Context { get; }

        public ICriterion<SocketReaction> Criterion { get; }

        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            var emote = reaction.Emote;

            if (emote.Equals(Options.First))
            {
                _currentPage = 1;
            }
            else if (emote.Equals(Options.Next))
            {
                if (_currentPage >= _pages) return false;
                ++_currentPage;
            }
            else if (emote.Equals(Options.Back))
            {
                if (_currentPage <= 1) return false;
                --_currentPage;
            }
            else if (emote.Equals(Options.Last))
            {
                _currentPage = _pages;
            }
            else if (emote.Equals(Options.Stop))
            {
                await Message.RemoveAllReactionsAsync().ConfigureAwait(false);
                return true;
            }
            else if (emote.Equals(Options.Jump))
            {
                _ = Task.Run(async () =>
                {
                    var criteria = new Criteria<SocketMessage>()
                        .AddCriterion(new EnsureSourceChannelCriterion())
                        .AddCriterion(new EnsureFromUserCriterion(reaction.UserId))
                        .AddCriterion(new EnsureIsIntegerCriterion());

                    var response = await Interactive.NextMessageAsync(Context, criteria, TimeSpan.FromSeconds(15));

                    var request = int.Parse(response.Content);
                    if (request < 1 || request > _pages)
                    {
                        _ = response.DeleteAsync().ConfigureAwait(false);
                        await Interactive.ReplyAndDeleteAsync(Context, Options.Stop.Name);
                        return;
                    }

                    _currentPage = request;
                    _ = response.DeleteAsync().ConfigureAwait(false);
                    await RenderAsync().ConfigureAwait(false);
                });
            }
            else if (emote.Equals(Options.Info))
            {
                await Interactive.ReplyAndDeleteAsync(Context, null, embed:
                    new EmbedBuilder()
                        .WithTitle(Options.InformationTitle)
                        .WithDescription(Options.InformationText)
                        .WithColor(Options.InformationColor)
                        .Build(),
                    timeout: Options.InfoTimeout);

                return false;
            }

            _ = Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            await RenderAsync().ConfigureAwait(false);
            return false;
        }

        public async Task DisplayAsync(ReactionList reactionList)
        {
            var embed = BuildEmbed();
            var message = await Context.Channel.SendMessageAsync(_pager.Content, embed: embed).ConfigureAwait(false);

            Message = message;
            Interactive.AddReactionCallback(message, this);

            _ = Task.Run(async () =>
            {
                if (reactionList.First) await message.AddReactionAsync(Options.First);

                if (reactionList.Backward) await message.AddReactionAsync(Options.Back);

                if (reactionList.Forward) await message.AddReactionAsync(Options.Next);

                if (reactionList.Last) await message.AddReactionAsync(Options.Last);

                var manageMessages = Context.Channel is IGuildChannel guildChannel &&
                                     ((IGuildUser) Context.User).GetPermissions(guildChannel).ManageMessages;

                if (reactionList.Jump
                    && Options.JumpDisplayOptions == JumpDisplayOptions.Always
                    || Options.JumpDisplayOptions == JumpDisplayOptions.WithManageMessages && manageMessages)
                    await message.AddReactionAsync(Options.Jump);
                if (reactionList.Trash) await message.AddReactionAsync(Options.Stop);
                if (reactionList.Info && Options.DisplayInformationIcon) await message.AddReactionAsync(Options.Info);
            });
            if (Timeout.HasValue)
                DisplayTimeout(message, Message);
        }

        public void DisplayTimeout(RestUserMessage m1, IUserMessage m2)
        {
            if (Timeout.HasValue)
                _ = Task.Delay(Timeout.Value).ContinueWith(_ =>
                {
                    Interactive.RemoveReactionCallback(m1);
                    m2.RemoveAllReactionsAsync();
                });
        }

        protected Embed BuildEmbed()
        {
            var current = _pager.Pages.ElementAt(_currentPage - 1);
            var builder = new EmbedBuilder
            {
                Title = current.Title ?? _pager.Title,
                Url = current.Url ?? _pager.Url,
                Description = current.Description ?? _pager.Description,
                ImageUrl = current.ImageUrl ?? _pager.ImageUrl,
                Color = current.Color ?? _pager.Color,
                Fields = current.Fields ?? _pager.Fields,
                Footer = current.FooterOverride ?? _pager.FooterOverride ?? new EmbedFooterBuilder
                {
                    Text = string.Format(Options.FooterFormat, _currentPage, _pages)
                },
                ThumbnailUrl = current.ThumbnailUrl ?? _pager.ThumbnailUrl,
                Timestamp = current.TimeStamp ?? _pager.TimeStamp
            };

            if (current.DisplayTotalFieldsCount)
                builder
                    .WithAuthor(author =>
                    {
                        author.Name =
                            $"{current.AlternateAuthorTitle}\nPage {_currentPage}/{_pages} ({Math.Round(_pager.Pages.Sum(x => x.Fields.Count) * current.TotalFieldsCountConstant)} {current.TotalFieldsMessage})";
                        author.IconUrl = current.AlternateAuthorIcon;
                    });
            else
                builder
                    .WithAuthor(author =>
                    {
                        author.Name = $"{current.AlternateAuthorTitle}\nPage {_currentPage}/{_pages}";
                        author.IconUrl = current.AlternateAuthorIcon;
                    });

            return builder.Build();
        }

        private Task RenderAsync()
        {
            var embed = BuildEmbed();
            return Message.ModifyAsync(m => m.Embed = embed);
        }
    }
}