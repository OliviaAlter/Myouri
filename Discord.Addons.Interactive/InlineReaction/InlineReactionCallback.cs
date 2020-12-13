// The foundation of this code came from PassiveModding's fork of the original repo
// https://github.com/PassiveModding/Discord.Addons.Interactive

using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive.Callbacks;
using DiscordBot.Discord.Addons.Interactive.Criteria;

namespace DiscordBot.Discord.Addons.Interactive.InlineReaction
{
    /// <summary>
    ///     The inline reaction callback.
    /// </summary>
    public class InlineReactionCallback : IReactionCallback
    {
        /// <summary>
        ///     The data.
        /// </summary>
        private readonly ReactionCallbackData _data;

        /// <summary>
        ///     The interactive.
        /// </summary>
        private readonly InteractiveService _interactive;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InlineReactionCallback" /> class.
        /// </summary>
        /// <param name="interactive">
        ///     The interactive.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        /// <param name="data">
        ///     The data.
        /// </param>
        /// <param name="criterion">
        ///     The criterion.
        /// </param>
        public InlineReactionCallback(
            InteractiveService interactive,
            SocketCommandContext context,
            ReactionCallbackData data,
            ICriterion<SocketReaction> criterion = null)
        {
            _interactive = interactive;
            Context = context;
            _data = data;
            Criterion = criterion ?? new EmptyCriterion<SocketReaction>();
            Timeout = data.Timeout ?? TimeSpan.FromSeconds(30);
        }

        /// <summary>
        ///     Gets the message.
        /// </summary>
        public IUserMessage Message { get; private set; }

        /// <summary>
        ///     The run mode.
        /// </summary>
        public RunMode RunMode => RunMode.Sync;

        /// <summary>
        ///     Gets the criterion.
        /// </summary>
        public ICriterion<SocketReaction> Criterion { get; }

        /// <summary>
        ///     Gets the timeout.
        /// </summary>
        public TimeSpan? Timeout { get; }

        /// <summary>
        ///     Gets the context.
        /// </summary>
        public SocketCommandContext Context { get; }

        /// <summary>
        ///     handles callbacks (reactions from users)
        /// </summary>
        /// <param name="reaction">
        ///     The reaction.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            // If reaction is not specified in our Callback List, ignore
            var reactionCallbackItem = _data.Callbacks.FirstOrDefault(t => t.Reaction.Equals(reaction.Emote));
            if (reactionCallbackItem == null) return false;

            if (_data.SingleUsePerUser)
            {
                // Ensure that we only allow users to react a single time.
                if (_data.ReactorIDs.Contains(reaction.UserId))
                    return _data.ExpiresAfterUse;

                await reactionCallbackItem.Callback(Context, reaction);
                _data.ReactorIDs.Add(reaction.UserId);
            }
            else
            {
                await reactionCallbackItem.Callback(Context, reaction);
            }

            return _data.ExpiresAfterUse;
        }

        /// <summary>
        ///     The display async.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        public async Task DisplayAsync()
        {
            var message = await Context.Channel.SendMessageAsync(_data.Text, embed: _data.Embed).ConfigureAwait(false);
            Message = message;
            _interactive.AddReactionCallback(message, this);

            _ = Task.Run(async () =>
            {
                foreach (var item in _data.Callbacks) await message.AddReactionAsync(item.Reaction);
            });

            if (Timeout.HasValue)
                _ = Task.Delay(Timeout.Value)
                    .ContinueWith(_ =>
                    {
                        _interactive.RemoveReactionCallback(message);
                        _data.TimeoutCallback?.Invoke(Context);
                    });
        }
    }
}