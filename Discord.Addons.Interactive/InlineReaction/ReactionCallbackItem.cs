// The foundation of this code came from PassiveModding's fork of the original repo
// https://github.com/PassiveModding/Discord.Addons.Interactive

using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Discord.Addons.Interactive.InlineReaction
{
    /// <summary>
    ///     The reaction callback item.
    /// </summary>
    public class ReactionCallbackItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReactionCallbackItem" /> class.
        /// </summary>
        /// <param name="reaction">
        ///     The reaction.
        /// </param>
        /// <param name="callback">
        ///     The callback.
        /// </param>
        public ReactionCallbackItem(IEmote reaction, Func<SocketCommandContext, SocketReaction, Task> callback)
        {
            Reaction = reaction;
            Callback = callback;
        }

        /// <summary>
        ///     Gets the reaction.
        /// </summary>
        public IEmote Reaction { get; }

        /// <summary>
        ///     Gets the callback.
        /// </summary>
        public Func<SocketCommandContext, SocketReaction, Task> Callback { get; }
    }
}