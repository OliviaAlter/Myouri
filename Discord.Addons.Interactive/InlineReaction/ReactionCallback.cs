// The foundation of this code came from PassiveModding's fork of the original repo
// https://github.com/PassiveModding/Discord.Addons.Interactive

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Discord.Addons.Interactive.InlineReaction
{
    public class ReactionCallbackData
    {
        private ICollection<ReactionCallbackItem> _items;

        public ReactionCallbackData(string text, Embed embed = null, bool expiresAfterUse = true,
            bool singleUsePerUser = true, TimeSpan? timeout = null,
            Func<SocketCommandContext, Task> timeoutCallback = null)
        {
            if (text == null && embed == null) throw new Exception("Inline reaction must have message data");

            SingleUsePerUser = singleUsePerUser;
            ExpiresAfterUse = expiresAfterUse;
            ReactorIDs = new List<ulong>();
            Text = text ?? "";
            Embed = embed;
            Timeout = timeout;
            TimeoutCallback = timeoutCallback;
            _items = new List<ReactionCallbackItem>();
        }

        public IEnumerable<ReactionCallbackItem> Callbacks => _items;
        public bool ExpiresAfterUse { get; }
        public bool SingleUsePerUser { get; }
        public List<ulong> ReactorIDs { get; }
        public string Text { get; }
        public Embed Embed { get; }
        public TimeSpan? Timeout { get; }
        public Func<SocketCommandContext, Task> TimeoutCallback { get; }

        public ReactionCallbackData AddCallbacks(
            IEnumerable<(IEmote, Func<SocketCommandContext, SocketReaction, Task>)> callbacks)
        {
            foreach (var (emote, item2) in callbacks) _items.Add(new ReactionCallbackItem(emote, item2));

            return this;
        }

        public ReactionCallbackData SetCallbacks(
            IEnumerable<(IEmote, Func<SocketCommandContext, SocketReaction, Task>)> callbacks)
        {
            _items = callbacks.Select(x => new ReactionCallbackItem(x.Item1, x.Item2)).ToList();
            return this;
        }

        public ReactionCallbackData AddCallBack(IEmote reaction,
            Func<SocketCommandContext, SocketReaction, Task> callback)
        {
            _items.Add(new ReactionCallbackItem(reaction, callback));
            return this;
        }

        public ReactionCallbackData WithCallback(IEmote reaction,
            Func<SocketCommandContext, SocketReaction, Task> callback)
        {
            var item = new ReactionCallbackItem(reaction, callback);
            _items.Add(item);
            return this;
        }
    }
}