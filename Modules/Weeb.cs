using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Discord.Addons.Interactive.Paginator;
using DiscordBot.Extension;
using DiscordBot.Services;
using DiscordBot.Utilities;
using JikanDotNet;
using static System.DateTime;

namespace DiscordBot.Modules
{
    [Summary(":b:")]
    public class Weeb : InteractiveBase<SocketCommandContext>
    {
        private readonly IJikan _jikan = new Jikan(true);

        private static AnimeSearchEntry EvaluateAnime(string query, ICollection<AnimeSearchEntry> animeSearchEntries)
        {
            return (from anime in animeSearchEntries
                let percentage = (double) query.Length / (double) anime.Title.Length * 100
                where percentage <= 100
                select anime).FirstOrDefault();
        }

        private static MangaSearchEntry EvaluateManga(string query, ICollection<MangaSearchEntry> mangaSearchEntries)
        {
            return (from manga in mangaSearchEntries
                let percentage = (double) query.Length / (double) manga.Title.Length * 100
                where percentage <= 100
                select manga).FirstOrDefault();
        }
        

        [Command("e", RunMode = RunMode.Async)]
        [Alias("ep")]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task Recommend([Remainder] string query)
        {
            if (query.Length < 3)
            {
                await Context.Channel.SendErrorAsync("Your input must have at least 3 characters !",
                    "Please try again!");
                return;
            }

            var animeSearchConfig = new AnimeSearchConfig
            {
                Type = AnimeType.EveryType
            };
            var animeSearchResult = await _jikan.SearchAnime($"{query}", animeSearchConfig);
            var animeSearch = animeSearchResult.Results;
            if (animeSearch == null)
            {
                await Context.Channel.SendErrorJikanAsync($"Not found any anime for {query}",
                    "Error ! Please recheck your spelling", Context.User);
                return;
            }

            var animeSearchEntry = EvaluateAnime(query, animeSearch);
            //Computing misses
            //var computing = LevenshteinDistanceComputingService.Evaluation(query.ToLower(), animeSearchEntry.Title.ToLower());
            var percentage = query.Length / (double)animeSearchEntry.Title.Length * 100;

            if (percentage >= 33)
            {
                var episodes = await _jikan.GetAnimeEpisodes(animeSearchEntry.MalId).ConfigureAwait(false);
                var pages = episodes.EpisodeCollection.Select(result => new EmbedPage
                    {
                        Title = $"**{result.Title}**",
                        Description = $"Have anything to discuss with other viewers ? " +
                                      $"\nClick here to enter the MAL forum! " +
                                      $"\n**[Forum]({result.ForumUrl})**",
                        AlternateAuthorTitle = "MyAnimeList",
                        AlternateAuthorIcon = "https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png",
                        Url = result.VideoUrl,
                        TimeStamp = Now,
                        ImageUrl = $"{animeSearchEntry.ImageURL}"
                    })
                    .ToList();
                var options = new PaginatedAppearanceOptions
                {
                    InformationText = $"**Episodes list for [{animeSearchEntry.Title}]**",
                    Next = new Emoji("▶️"),
                    Back = new Emoji("◀️"),
                    Last = new Emoji("↪️"),
                    First = new Emoji("↩️"),
                    Timeout = TimeSpan.FromSeconds(60)
                };
                var paginatedMessage = new PaginatedMessage
                {
                    Pages = pages,
                    Content =
                        $"**Showing episodes details for [{animeSearchEntry.Title}]**",
                    Options = options,
                    FooterOverride = new EmbedFooterBuilder().WithText($"Requested by {Context.User.Username}")
                        .WithIconUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl()),
                    TimeStamp = Now,
                    Color = new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                };
                await PagedReplyAsync(paginatedMessage, new ReactionList());
            }
            else
            {
                await Context.Channel.SendErrorJikanAsync("Match percentage is below 33%",
                    $"We can't not find any suitable anime and its episodes with input : [{query}]", Context.User);
            }
        }

        [Command("m", RunMode = RunMode.Async)]
        [Alias("manga")]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task FetchManga([Remainder] string query)
        {
            if (query.Length < 3)
            {
                await Context.Channel.SendErrorAsync("Your input must have at least 3 characters !",
                    "Please try again!");
                return;
            }

            var mangaSearchConfig = new MangaSearchConfig
            {
                Type = MangaType.EveryType
            };
            var mangaSearchResult = await _jikan.SearchManga($"{query}", mangaSearchConfig);
            var mangaSearch = mangaSearchResult.Results;
            if (mangaSearch == null)
            {
                await Context.Channel.SendErrorJikanAsync($"Not found any manga for {query}",
                    "Error ! Please recheck your spelling", Context.User);
                return;
            }

            var mangaSearchEntry = EvaluateManga(query, mangaSearch);
            var percentage = query.Length / (double) mangaSearchEntry.Title.Length * 100;
            if (percentage >= 33)
            {
                await Context.Channel.SendSuccessMangaAsync($"Found manga for [{query}]",
                    $"{mangaSearchEntry.Title}",
                    $"{mangaSearchEntry.Type}",
                    $"{mangaSearchEntry.Score}",
                    $"{mangaSearchEntry.Description}",
                    $"{mangaSearchEntry.URL}",
                    mangaSearchEntry.ImageURL, Context.User);
            }
            else
            {
                var pages = new List<EmbedPage>();
                var count = 0;
                foreach (var result in mangaSearch)
                {
                    var page = new EmbedPage
                    {
                        Title = $"**{result.Title}**",
                        Description = result.Description,
                        AlternateAuthorTitle = "MyAnimeList",
                        AlternateAuthorIcon = "https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png",
                        ThumbnailUrl = result.ImageURL,
                        Url = result.URL,
                        TimeStamp = Now
                    };
                    count++;
                    if (count == 6) break;
                    pages.Add(page);
                }

                var options = new PaginatedAppearanceOptions
                {
                    InformationText = $"**Possible result for [{query}]**",
                    Next = new Emoji("▶️"),
                    Back = new Emoji("◀️"),
                    Last = new Emoji("↪️"),
                    First = new Emoji("↩️"),
                    Timeout = TimeSpan.FromSeconds(60)
                };

                var paginatedMessage = new PaginatedMessage
                {
                    Pages = pages,
                    Content =
                        $"**Showing maximum of 5 possible options for [{query}] since threshold for match is below 33%**",
                    Options = options,
                    FooterOverride = new EmbedFooterBuilder().WithText($"Requested by {Context.User.Username}")
                        .WithIconUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl()),
                    TimeStamp = Now,
                    Color = new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                };
                await PagedReplyAsync(paginatedMessage, new ReactionList());
            }
        }


        [Command("a", RunMode = RunMode.Async)]
        [Alias("anime", "ani")]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task FetchAnime([Remainder] string query)
        {
            if (query.Length < 3)
            {
                await Context.Channel.SendErrorAsync("Your input must have at least 3 characters !",
                    "Please try again!");
                return;
            }

            var animeSearchConfig = new AnimeSearchConfig
            {
                Type = AnimeType.EveryType
            };
            var animeSearchResult = await _jikan.SearchAnime($"{query}", animeSearchConfig);
            var animeSearch = animeSearchResult.Results;
            if (animeSearch == null)
            {
                await Context.Channel.SendErrorJikanAsync($"Not found any anime for {query}",
                    "Error ! Please recheck your spelling", Context.User);
                return;
            }

            var animeSearchEntry = EvaluateAnime(query, animeSearch);
            //Computing misses
            //var computing = LevenshteinDistanceComputingService.Evaluation(query.ToLower(), animeSearchEntry.Title.ToLower());
            var percentage = query.Length / (double) animeSearchEntry.Title.Length * 100;

            #region Anime version and spinoff

            /*
            var animeVer = await _jikan.GetAnime(animeSearchEntry.MalId);
            var sb = new StringBuilder();
            if (animeVer.Related.SpinOffs != null)
                foreach (var spinOff in animeVer.Related.SpinOffs)
                    sb.AppendLine("Spinoff : " + spinOff.Name + $"\nMAL link : [Click here]({spinOff.Url})");

            if (animeVer.Related.Sequels != null)
                foreach (var sequel in animeVer.Related.Sequels)
                    sb.AppendLine("Sequel : " + sequel.Name + $"\nMAL link : [Click here]({sequel.Url})");

            if (animeVer.Related.AlternativeVersions != null)
                foreach (var alter in animeVer.Related.AlternativeVersions)
                    sb.AppendLine("Alternative : " + alter.Name + $"\nMAL link : [Click here]({alter.Url})");

            if (animeVer.Related.AlternativeSettings != null)
                foreach (var alterS in animeVer.Related.AlternativeSettings)
                    sb.AppendLine("Alternative : " + alterS.Name + $"\nMAL link : [Click here]({alterS.Url})");

            if (animeVer.Related.Adaptations != null)
                foreach (var adaption in animeVer.Related.Adaptations)
                    sb.AppendLine("Adaption : " + adaption.Name + $"\nMAL link : [Click here]({adaption.Url})");

            if (animeVer.Related.Prequels != null)
                foreach (var prequel in animeVer.Related.Prequels)
                    sb.AppendLine("Prequel : " + prequel.Name + $"\nMAL link : [Click here]({prequel.Url})");

            */

            #endregion

            if (percentage >= 33)
            {
                await Context.Channel.SendSuccessAnimeAsync($"Found anime for [{query}]",
                    $"{animeSearchEntry.Title}",
                    $"{animeSearchEntry.Rated}",
                    $"{animeSearchEntry.Score}",
                    $"{animeSearchEntry.Episodes}",
                    $"{animeSearchEntry.Description}",
                    $"{animeSearchEntry.URL}",
                    animeSearchEntry.ImageURL, Context.User);
            }
            else
            {
                var pages = new List<EmbedPage>();
                var count = 0;
                foreach (var result in animeSearch)
                {
                    var page = new EmbedPage
                    {
                        Title = $"**{result.Title}**",
                        Description = result.Description,
                        AlternateAuthorTitle = "MyAnimeList",
                        AlternateAuthorIcon = "https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png",
                        ThumbnailUrl = result.ImageURL,
                        Url = result.URL,
                        TimeStamp = Now
                    };
                    count++;
                    if (count == 6) break;
                    pages.Add(page);
                }

                var options = new PaginatedAppearanceOptions
                {
                    InformationText = $"**Possible result for [{query}]**",
                    Next = new Emoji("▶️"),
                    Back = new Emoji("◀️"),
                    Last = new Emoji("↪️"),
                    First = new Emoji("↩️"),
                    Timeout = TimeSpan.FromSeconds(60)
                };

                var paginatedMessage = new PaginatedMessage
                {
                    Pages = pages,
                    Content =
                        $"**Showing maximum of 5 possible options for [{query}] since threshold for match is below 33%**",
                    Options = options,
                    FooterOverride = new EmbedFooterBuilder().WithText($"Requested by {Context.User.Username}")
                        .WithIconUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl()),
                    TimeStamp = Now,
                    Color = new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                };
                await PagedReplyAsync(paginatedMessage, new ReactionList());
            }
        }
    }
}