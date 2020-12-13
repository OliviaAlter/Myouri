using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Utilities;

namespace DiscordBot.Extension
{
    public static class DotaExtension
    {
        public static async Task<IMessage> SendErrorDotaProfileAsync(this ISocketMessageChannel channel, string title,
            string description, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author.WithIconUrl("https://pbs.twimg.com/profile_images/1148484652358746112/UdJALHjZ_400x400.png")
                        .WithName(title);
                })
                .WithCurrentTimestamp()
                .WithFooter("Powered by OpenDota API")
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendDotaProfile(this ISocketMessageChannel channel, string title,
            string description, string url, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithThumbnailUrl(url ?? "No image found")
                .WithAuthor(author =>
                {
                    author.WithIconUrl("https://pbs.twimg.com/profile_images/1148484652358746112/UdJALHjZ_400x400.png")
                        .WithName(title);
                })
                .WithCurrentTimestamp()
                .WithFooter("Powered by OpenDota API")
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendDotaSuccessAsync(this ISocketMessageChannel channel, string title,
            string description, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author.WithIconUrl("https://pbs.twimg.com/profile_images/1148484652358746112/UdJALHjZ_400x400.png")
                        .WithName(title);
                })
                .WithCurrentTimestamp()
                .WithFooter("Powered by OpenDota API")
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendDotaErrorAsync(this ISocketMessageChannel channel, string title,
            string description, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author.WithIconUrl("https://pbs.twimg.com/profile_images/1148484652358746112/UdJALHjZ_400x400.png")
                        .WithName(title);
                })
                .WithCurrentTimestamp()
                .WithFooter("Powered by OpenDota API")
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }
    }
}