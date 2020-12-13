using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Utilities;

namespace DiscordBot.Extension
{
    public static class ReplyExtension
    {
        public static async Task<IMessage> SendModerationEmbedAsync(this ISocketMessageChannel channel, string title,
            string description, IUser user, IUser moderator, string reason, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .AddField("User", user)
                .AddField("Command issued by", moderator)
                .AddField("Reason", reason)
                .WithFooter($"{moderator.Username}", moderator.GetAvatarUrl() ?? moderator.GetDefaultAvatarUrl())
                .WithCurrentTimestamp()
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendSuccessAsync(this ISocketMessageChannel channel, string title,
            string description, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author.WithIconUrl(
                            "https://icons-for-free.com/iconfiles/png/512/complete+done+green+success+valid+icon-1320183462969251652.png")
                        .WithName(title);
                })
                .WithCurrentTimestamp()
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendSuccessStringBuilderAsync(this ISocketMessageChannel channel,
            string title,
            StringBuilder description, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description.ToString())
                .WithAuthor(author =>
                {
                    author.WithIconUrl(
                            "https://icons-for-free.com/iconfiles/png/512/complete+done+green+success+valid+icon-1320183462969251652.png")
                        .WithName(title);
                })
                .WithCurrentTimestamp()
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendSuccessModerationAsync(this ISocketMessageChannel channel, string title,
            string description, IUser user, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author.WithIconUrl(
                            "https://icons-for-free.com/iconfiles/png/512/complete+done+green+success+valid+icon-1320183462969251652.png")
                        .WithName(title);
                })
                .WithFooter($"{user.Username}", user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithCurrentTimestamp()
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendErrorAsync(this ISocketMessageChannel channel, string title,
            string description, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author.WithIconUrl("https://cdn.icon-icons.com/icons2/1380/PNG/512/vcsconflicting_93497.png")
                        .WithName(title);
                })
                .WithCurrentTimestamp()
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendErrorModerationAsync(this ISocketMessageChannel channel, string title,
            string description, IUser user, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author.WithIconUrl("https://cdn.icon-icons.com/icons2/1380/PNG/512/vcsconflicting_93497.png")
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