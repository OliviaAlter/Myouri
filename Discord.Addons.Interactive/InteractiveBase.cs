using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive.Criteria;
using DiscordBot.Discord.Addons.Interactive.InlineReaction;
using DiscordBot.Discord.Addons.Interactive.Paginator;
using DiscordBot.Discord.Addons.Interactive.Results;

namespace DiscordBot.Discord.Addons.Interactive
{
    public abstract class InteractiveBase : InteractiveBase<SocketCommandContext>
    {
    }

    public abstract class InteractiveBase<T> : ModuleBase<T> where T : SocketCommandContext
    {
        public InteractiveService Interactive { get; set; }

        public Task<SocketMessage> NextMessageAsync(ICriterion<SocketMessage> criterion, TimeSpan? timeout = null,
            CancellationToken token = default)
        {
            return Interactive.NextMessageAsync(Context, criterion, timeout, token);
        }

        public Task<SocketMessage> NextMessageAsync(bool fromSourceUser = true, bool inSourceChannel = true,
            TimeSpan? timeout = null, CancellationToken token = default)
        {
            return Interactive.NextMessageAsync(Context, fromSourceUser, inSourceChannel, timeout, token);
        }

        public Task<IUserMessage> ReplyAndDeleteAsync(string content, bool isTTS = false, Embed embed = null,
            TimeSpan? timeout = null, RequestOptions options = null)
        {
            return Interactive.ReplyAndDeleteAsync(Context, content, isTTS, embed, timeout, options);
        }

        public Task<IUserMessage> InlineReactionReplyAsync(ReactionCallbackData data, bool fromSourceUser = true)
        {
            return Interactive.SendMessageWithReactionCallbacksAsync(Context, data, fromSourceUser);
        }

        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, ReactionList reactions,
            bool fromSourceUser = true)
        {
            var criterion = new Criteria<SocketReaction>();
            if (fromSourceUser)
                criterion.AddCriterion(new EnsureReactionFromSourceUserCriterion());
            return PagedReplyAsync(pager, reactions, criterion);
        }

        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, ReactionList reactions,
            ICriterion<SocketReaction> criterion)
        {
            return Interactive.SendPaginatedMessageAsync(Context, pager, reactions, criterion);
        }

        public RuntimeResult Ok(string reason = null)
        {
            return new OkResult(reason);
        }
    }
}