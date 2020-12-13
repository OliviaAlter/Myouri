using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Discord.Addons.Interactive.Paginator;
using DiscordBot.Extension;
using DiscordBot.Utilities;
using NHentai.NET.Client;
using NHentaiAPI;
using static System.DateTime;

namespace DiscordBot.Modules
{
    public class nHentai : InteractiveBase<SocketCommandContext>
    {
        //This is client for getting pages 
        private readonly HentaiClient _hentai = new HentaiClient();

        //This is client for searching for tags, etc
        private readonly NHentaiClient _nHentai = new NHentaiClient();

        [Command("read", RunMode = RunMode.Async)]
        [Alias("rhen")]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task ReadingHen(int henId)
        {
            try
            {
                var book = await _hentai.SearchBookAsync(henId);
                var pages = book.GetPages().ToList();

                var paging = pages
                    .Select(result => new EmbedPage
                    {
                        Title = $"**{book.Titles.Japanese}**",
                        AlternateAuthorTitle = "Nhentai",
                        AlternateAuthorIcon = "https://i.4cdn.org/h/1605807858643.png",
                        ImageUrl = result,
                        TimeStamp = Now,
                        Url = $"https://nhentai.net/g/" + $"{book.Id}/"
                    }).ToList();

                var options = new PaginatedAppearanceOptions
                {
                    Next = new Emoji("▶️"),
                    Back = new Emoji("◀️"),
                    Last = new Emoji("↪️"),
                    First = new Emoji("↩️"),
                    Timeout = TimeSpan.FromMinutes(15)
                };
                var paginatedMessage = new PaginatedMessage
                {
                    Pages = paging,
                    Content = "**Happy reading, this embed will expire in 15 minutes**",
                    Options = options,
                    FooterOverride = new EmbedFooterBuilder().WithText($"Requested by {Context.User.Username}")
                        .WithIconUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl()),
                    TimeStamp = Now,
                    Color = new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                };
                await PagedReplyAsync(paginatedMessage, new ReactionList());
            }
            catch
            {
                await Context.Channel.SendErrorAsync("Not found",
                    "Our engine can't find anything using your provided id");
            }
        }

        [Command("l", RunMode = RunMode.Async)]
        [Alias("lh")]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task LookUpHen()
        {
            var messageCache = Context.Channel.CachedMessages.Reverse();
            foreach (var messageCheck in messageCache)
            {
                if (!int.TryParse(messageCheck.ToString(), out var bookId)) continue;
                try
                {
                    StringBuilder sb = new StringBuilder();
                    var book = await _nHentai.GetBookAsync(bookId);
                    var imageUrl = _nHentai.GetBookThumbUrl(book);
                    var url = "https://nhentai.net/g/" + $"{book.Id}/";
                    foreach (var tag in book.Tags)
                    {
                        sb.Append($"{tag.Name}, ");
                    }

                    await Context.Channel.SendSuccessNhentaiAsync(
                        $"{book.Title.Japanese}",
                        $"{book.Title.English}",
                        $"{sb}",
                        Convert.ToDateTime($"{book.UploadDate}"),
                        $"{book.NumPages}",
                        $"{url}", imageUrl);
                    return;
                }
                catch
                {
                    //
                }
            }
        }


        [Command("s", RunMode = RunMode.Async)]
        [Alias("sh")]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task SearchHen(int henId)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                var book = await _nHentai.GetBookAsync(henId);
                var imageUrl = _nHentai.GetBookThumbUrl(book);
                var url = "https://nhentai.net/g/" + $"{book.Id}/";
                foreach (var tag in book.Tags)
                {
                    sb.Append($"{tag.Name}, ");
                }

                await Context.Channel.SendSuccessNhentaiAsync(
                    $"{book.Title.Japanese}",
                    $"{book.Title.English}",
                    $"{sb}",
                    Convert.ToDateTime($"{book.UploadDate}"),
                    $"{book.NumPages}",
                    $"{url}", imageUrl);
            }
            catch
            {
                await Context.Channel.SendErrorNhentaiAsync("Invalid ID",
                   "Our engine can't find using your provided id");
            }
        }
    }
}