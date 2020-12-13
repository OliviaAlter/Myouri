using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Utilities;

namespace DiscordBot.Extension
{
    public static class SteamExtension
    {
        public static async Task<IMessage> SendSteamProfile(this ISocketMessageChannel channel, string title,
            string description, string url, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithThumbnailUrl(url ?? "No image found")
                .WithAuthor(author =>
                {
                    author.WithIconUrl(
                            "https://upload.wikimedia.org/wikipedia/commons/thumb/8/83/Steam_icon_logo.svg/768px-Steam_icon_logo.svg.png")
                        .WithName(title);
                })
                .WithCurrentTimestamp()
                .WithFooter("Powered by Steam API")
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendErrorSteamProfileAsync(this ISocketMessageChannel channel, string title,
            string description, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author.WithIconUrl(
                            "https://upload.wikimedia.org/wikipedia/commons/thumb/8/83/Steam_icon_logo.svg/768px-Steam_icon_logo.svg.png")
                        .WithName(title);
                })
                .WithCurrentTimestamp()
                .WithFooter("Powered by Steam API")
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }
    }
}