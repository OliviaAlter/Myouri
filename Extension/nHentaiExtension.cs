using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Utilities;

namespace DiscordBot.Extension
{
    public static class NHentaiExtension
    {
        public static async Task<IMessage> SendSuccessNhentaiAsync(this ISocketMessageChannel channel, string japTitle,
             string eng, string tags, DateTime date, string pages,string site, string url, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithAuthor(author =>
                {
                    author.WithIconUrl(
                            "https://i.4cdn.org/h/1605807858643.png")
                        .WithName(japTitle);
                })
                .WithTitle("Result found!")
                .AddField("English title", eng)
                .AddField("Tags", tags)
                .AddField("Publish date", date)
                .AddField("Num of pages", pages)
                .AddField("Direct URL",$"[Click here]({site})" )
                .WithThumbnailUrl(url)
                .WithCurrentTimestamp()
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> SendErrorNhentaiAsync(this ISocketMessageChannel channel, string title,
            string description, RequestOptions options = null)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithDescription(description)
                .WithAuthor(author =>
                {
                    author.WithIconUrl(
                            "https://i.4cdn.org/h/1605807858643.png")
                        .WithName(title);
                })
                .WithCurrentTimestamp()
                .Build();
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

    }
}
