using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Utilities;

namespace DiscordBot.Extension
{
    public static class JikanExtension
    {
        public static async Task<IMessage> SendSuccessAnimeAsync(this ISocketMessageChannel channel,
            string topic, string title, string rated, string score, string episode,
            string description, string url, string image, IUser user, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithTitle(title)
                .AddField("**Rated**", rated, true)
                .AddField("**MAL Score**", score, true)
                .AddField("**Episode**", episode, true)
                .AddField("**MAL link**", $"[Click here]({url})")
                .WithAuthor(author =>
                {
                    author.WithIconUrl("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png")
                        .WithName(topic);
                })
                .WithDescription(description)
                .WithThumbnailUrl(image)
                .WithFooter($"Requested by {user.Username}", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithCurrentTimestamp()
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendSuccessMangaAsync(this ISocketMessageChannel channel,
            string topic, string title, string type, string score,
            string description, string url, string image, IUser user, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithTitle(title)
                .AddField("**Type**", type, true)
                .AddField("**MAL Score**", score, true)
                .AddField("**MAL link**", $"[Click here]({url})")
                .WithAuthor(author =>
                {
                    author.WithIconUrl("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png")
                        .WithName(topic);
                })
                .WithDescription(description)
                .WithThumbnailUrl(image)
                .WithFooter($"Requested by {user.Username}", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithCurrentTimestamp()
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendErrorJikanAsync(this ISocketMessageChannel channel, string title,
            string description, IUser user, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author.WithIconUrl("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png")
                        .WithName(title);
                })
                .WithFooter($"{user.Username}", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithCurrentTimestamp()
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }
    }
}