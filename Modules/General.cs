using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DatabaseEntity;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Discord.Addons.Interactive.InlineReaction;
using DiscordBot.Extension;
using DiscordBot.Model;
using DiscordBot.Services;
using DiscordBot.Utilities;
using Newtonsoft.Json;

namespace DiscordBot.Modules
{
    [Summary(":sa:")]
    public class General : InteractiveBase<SocketCommandContext>
    {
        private const ulong OwnerId = 247742975608750090;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly HttpClient _httpClient;
        private readonly HttpClientService _httpClientService;
        private readonly ImageService _imageService;
        private readonly Servers _servers;

        public General(CommandService commands, Servers servers, DiscordSocketClient client, ImageService imageService,
            HttpClientService httpClientService, HttpClient httpClient)
        {
            _commands = commands;
            _servers = servers;
            _client = client;
            _imageService = imageService;
            _httpClientService = httpClientService;
            _httpClient = httpClient;
        }

        [Command("image", RunMode = RunMode.Async)]
        public async Task Image(SocketGuildUser user)
        {
            var path = await _imageService.CreateImageAsync(user);
            await Context.Channel.SendFileAsync(path);
            File.Delete(path);
        }

        [Command("reaction", RunMode = RunMode.Async)]
        public async Task Test_ReactionReply()
        {
            await InlineReactionReplyAsync(new ReactionCallbackData("text")
                .WithCallback(new Emoji("👍"),
                    (c, r) => c.Channel.SendMessageAsync($"{r.User.Value.Mention} replied with 👍"))
                .WithCallback(new Emoji("👎"),
                    (c, r) => c.Channel.SendMessageAsync($"{r.User.Value.Mention} replied with 👎"))
            );
        }

        [Command("ping", true, RunMode = RunMode.Async)]
        [Summary("Get bot latency in ms")]
        public async Task Ping()
        {
            await Context.Channel.SendSuccessAsync("Ping", "Bonk bonk the bot with " + _client.Latency + " ms");
        }

        [Command("help", RunMode = RunMode.Async)]
        [Alias("help me")]
        public async Task Help([Remainder] string arg = null)
        {
            var guildPrefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? "*";
            var builder = new EmbedBuilder();
            builder.WithTitle("Help me please")
                .WithDescription($"You can use `{guildPrefix}help <cmd/catalog>!`")
                .WithFooter(_commands.Commands.Count() + " commands", Context.Client.CurrentUser.GetAvatarUrl());
            if (arg == null)
            {
                foreach (var module in _commands.Modules)
                    builder.AddField($"{module.Summary.ToLower()} {module.Name}", $"{module.Commands.Count} commands",
                        true);
                await ReplyAsync(embed: builder.Build());
                return;
            }

            arg = arg.ToLower();
            var m = _commands.Modules.FirstOrDefault(c => c.Name.ToLower().Equals(arg));
            if (m == null)
            {
                var cmd = _commands.Commands.FirstOrDefault(c => c.Name.ToLower().Equals(arg));
                if (cmd == null)
                {
                    await ReplyAsync(
                        $"Not found command/catalog `{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(arg)}`");
                    return;
                }

                var desc = "Description: " + (cmd.Summary ?? "No description");
                var parameter = cmd.Parameters.Aggregate<ParameterInfo, string>(null,
                    (current, parameterInfo) =>
                        $"{(current == null ? null : current + "\n")}'{parameterInfo.Name}' [{parameterInfo.Type.Name}] - {parameterInfo.Summary ?? "No description"}");
                builder.WithDescription(desc).WithTitle($"Help for: {cmd.Name}");
                if (!string.IsNullOrEmpty(parameter)) builder.AddField("Parameters", parameter);
                {
                    await ReplyAsync(embed: builder.Build());
                }
                return;
            }

            foreach (var cmd in m.Commands)
                builder.AddField($"{guildPrefix}{cmd.Name}", cmd.Summary ?? "No description", true)
                    .WithFooter(m.Commands.Count + " commands", Context.Client.CurrentUser.GetAvatarUrl());
            await ReplyAsync(embed: builder.Build());
        }

        [Command("info", RunMode = RunMode.Async)]
        [Summary("Get your information")]
        [RequireContext(ContextType.Guild)]
        public async Task Info(SocketGuildUser user = null)
        {
            user = (SocketGuildUser)(user ?? (IGuildUser)Context.User);
            var builder = new EmbedBuilder();
            builder.WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithDescription($"Basic information about {user.Mention}!")
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()))
                .AddField("User ID: ", user.Id, true)
                .AddField("Created at ", user.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at", user.JoinedAt?.ToString("dd/MM/yyyy"), true)
                .AddField("Roles", string.Join(", ", user.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention)))
                .WithCurrentTimestamp()
                .WithAuthor(Context.User);
            await ReplyAsync(embed: builder.Build());
        }

        [Command("bot", RunMode = RunMode.Async)]
        [Summary("Get bot data")]
        public async Task BotInfo()
        {
            await BotData();
        }

        [Command("usage", RunMode = RunMode.Async)]
        [Summary("Bot data usage")]
        public async Task BotDataUsage()
        {
            if (Context.User.Id.Equals(OwnerId) && Context.User is SocketGuildUser)
                await BotUsage();
            else
                await Context.Channel.SendErrorAsync("Invalid permission", "That command is for developer only!");
        }

        [Command("8ball", RunMode = RunMode.Async)]
        [Summary("8ball like command")]
        public async Task EightBall([Remainder] string message)
        {
            var builder = new EmbedBuilder();
            builder.WithTitle("8Ball game")
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()))
                .WithAuthor(Context.User)
                .AddField("Your question", $"{message}");
            var replies = new[]
            {
                "Yes",
                "No",
                "Maybe...",
                "I don't know"
            };
            if (string.IsNullOrEmpty(message))
                builder.WithDescription("8Ball can't answer if you don't ask!!");
            else
                builder.AddField("Answer", $"{replies[new Random().Next(replies.Length - 1)]}");
            await ReplyAsync(embed: builder.Build());
        }

        [Command("say", RunMode = RunMode.Async)]
        [Description("Repeat what User type")]
        [Summary("Make bot says")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Say([Remainder] string message)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.SendMessageAsync(message);
        }

        [Command("Server", RunMode = RunMode.Async)]
        [Summary("Get information about this Server")]
        public async Task Server()
        {
            var builder = new EmbedBuilder();
            builder.WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("Basic information about the current Server.")
                .WithTitle($"{Context.Guild.Name} Information")
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()))
                .AddField("Created at", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"))
                .AddField("Member count", Context.Guild.MemberCount + " members")
                .AddField("Online Users",
                    Context.Guild.Users.Count(x => x.Status != UserStatus.Offline) +
                    " members")
                .AddField("Roles count",
                    Context.Guild.Roles.Count)
                .AddField("Owner", Context.Guild.Owner.Mention)
                .WithCurrentTimestamp();
            await ReplyAsync(embed: builder.Build());
        }

        [Command("allroles", RunMode = RunMode.Async)]
        [Summary("Get all roles in this Server")]
        public async Task ListRoles()
        {
            var guild = Context.Guild;
            var builder = new EmbedBuilder();
            builder.WithThumbnailUrl(Context.Guild.IconUrl)
                .WithTitle($"{Context.Guild.Name} Information")
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()))
                .WithCurrentTimestamp()
                .WithDescription(
                    $"All roles :\n {string.Join(", ", guild.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention))}");
            await ReplyAsync(embed: builder.Build());
        }

        public async Task<IUserMessage> BotData()
        {
            var builder = new EmbedBuilder();
            builder.WithTitle("Bot status")
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl() ??
                                  Context.Client.CurrentUser.GetDefaultAvatarUrl())
                .AddField("Bot ID", Context.Client.CurrentUser.Id)
                .AddField("Serving for", CountUsers() + " Users")
                .AddField("Currently in", CountGuilds() + " Servers")
                .AddField("Owner", "**Olivia#8888** (ID: 247742975608750090)")
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));
            return await ReplyAsync(embed: builder.Build());
        }

        public async Task<IUserMessage> BotUsage()
        {
            var builder = new EmbedBuilder();
            builder.WithTitle("Bot status")
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl() ??
                                  Context.Client.CurrentUser.GetDefaultAvatarUrl())
                .AddField("CPU usage", Utils.GetCpuCounter() + " %", true)
                .AddField("Ram usage", Utils.RamUsage() + " %", true)
                .WithCurrentTimestamp()
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()))
                .WithAuthor(Context.User);
            return await ReplyAsync(embed: builder.Build());
        }

        [Command("invite", RunMode = RunMode.Async)]
        [Summary("get bot's invite link")]
        public async Task GetInviteLink()
        {
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl() ??
                                  Context.Client.CurrentUser.GetDefaultAvatarUrl())
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .AddField("Here is my invite link ! ",
                    "[Click here](https://discord.com/api/oauth2/authorize?client_id=526256077440942122&permissions=8&scope=bot)",
                    true)
                .WithAuthor(Context.User)
                .WithColor(new Color(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor()));
            await ReplyAsync(embed: builder.Build());
        }

        [Command("joke", RunMode = RunMode.Async)]
        [Alias("funny")]
        public async Task Joke()
        {
            var result = await _httpClient.GetStringAsync("https://sv443.net/jokeapi/v2/joke/Any?type=single");

            if (Equals(result, "error"))
            {
                await Context.Channel.SendErrorAsync("Error", "There is something wrong while accessing the website!");
                return;
            }

            var jokeDeserialize = JsonConvert.DeserializeObject<JokeModel>(result);

            var embed = new EmbedBuilder { Title = "This is just a joke" };
            embed
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .AddField("Context", jokeDeserialize.Joke)
                .AddField("Category", jokeDeserialize.Category, true)
                .WithAuthor(Context.User)
                .WithFooter("Powered by sv443.net");
            await ReplyAsync(embed: embed.Build());
        }

        [Command("donald", RunMode = RunMode.Async)]
        [Alias("dump")]
        public async Task DTrump()
        {
            var value = await GetRandom();

            var sb = new StringBuilder();
            foreach (var tag in value.Tags) sb.Append($"{tag}, ");

            var embed = new EmbedBuilder { Title = "Quote from Tronald !" };
            embed
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .AddField("Context", value.Value)
                .AddField("Tag", sb)
                .WithAuthor(Context.User)
                .WithFooter("Powered by tronalddump.io");
            await ReplyAsync(embed: embed.Build());
        }

        public async Task<Root> GetRandom()
        {
            return await _httpClientService.GetWithHostAsync<Root>("https://tronalddump.io/random/quote",
                "tronalddump.io");
        }

        [Command("def", RunMode = RunMode.Async)]
        [Alias("wd")]
        public async Task WordDefinition(string language, [Remainder] string phase)
        {
            var response = await _httpClient.GetAsync("https://api.dictionaryapi.dev/api/v2/entries/" + $"{language}" +
                                                      $"/{phase}");
            var jsonAsync = await response.Content.ReadAsStringAsync();

            var dictionaryDeserialize = JsonConvert.DeserializeObject<IEnumerable<RootDictionary>>(jsonAsync);
            var dictionaryInvalidDeserialize = JsonConvert.DeserializeObject<IEnumerable<InvalidWord>>(jsonAsync);

            var rootDictionaries = dictionaryDeserialize.ToList();
            var rootInvalidDictionaries = dictionaryInvalidDeserialize.ToList();

            IEnumerable<Meaning> meanings = rootDictionaries[0].Meanings.ToList();
            IEnumerable<Phonetic> phonetics = rootDictionaries[0].Phonetics.ToList();

            var title = rootInvalidDictionaries[0].Title;
            var message = rootInvalidDictionaries[0].Message;
            var resolution = rootInvalidDictionaries[0].Resolution;

            var meaning = new StringBuilder();
            var phonetic = new StringBuilder();
            var example = new StringBuilder();
            var synonyms = new StringBuilder();
            var antonyms = new StringBuilder();

            foreach (var antonymWord in meanings)
            {
                WordDefinition first = antonymWord.Definitions.FirstOrDefault();
                if (first?.Antonyms == null) continue;
                foreach (var antonym in first?.Antonyms)
                {
                    antonyms.AppendLine("- " + antonym);
                }
            }

            foreach (var synonymWord in meanings)
            {
                WordDefinition first = synonymWord.Definitions.FirstOrDefault();
                if (first?.Synonyms == null) continue;
                foreach (var synonym in first?.Synonyms)
                {
                    synonyms.AppendLine("- " + synonym);
                }
            }

            foreach (var partOfSpeech in meanings)
                meaning.AppendLine("- " + partOfSpeech.PartOfSpeech);

            foreach (var definitions in meanings)
                foreach (var definition in definitions.Definitions)
                    meaning.AppendLine("- " + definition.Definition);

            foreach (var definitionExample in meanings)
                foreach (var definition in definitionExample.Definitions)
                    example.AppendLine("- " + definition.Example);

            foreach (var text in phonetics)
                phonetic.AppendLine("- " + text.Text);

            var meaningCap = meaning.Length != 0 ? meaning.ToString() : "No meaning";
            var exampleCap = example.Length != 0 ? example.ToString() : "No example";
            var phoneticCap = phonetic.Length != 0 ? phonetic.ToString() : "No text";
            var synonymsCap = synonyms.Length != 0 ? synonyms.ToString() : "No synonyms";
            var antonymsCap = antonyms.Length != 0 ? antonyms.ToString() : "No antonyms";

            var embed = new EmbedBuilder { Title = $"Definition for {phase}" };
            embed
                .WithColor(Utils.RandomColor(), Utils.RandomColor(), Utils.RandomColor())
                .WithAuthor(author =>
                {
                    author.WithIconUrl("https://image.flaticon.com/icons/png/512/1493/1493308.png")
                        .WithName("Dictionary");
                })
                .WithCurrentTimestamp()
                .WithFooter("Powered by dictionaryapi.dev");
            if (meaning.Capacity == 0 && phonetic.Capacity == 0 && example.Capacity == 0 && synonyms.Capacity == 0)
            {
                embed.AddField($"{title}", "**Look like you input a wrong word or phase**")
                    .AddField("Message from us", $"{message}")
                    .AddField("Resolution", $"{resolution}");
            }
            else
            {
                embed.AddField("Definition", meaningCap)
                    .AddField("Text", phoneticCap)
                    .AddField("Example", exampleCap)
                    .AddField("Synonyms", synonymsCap)
                    .AddField("Antonyms", antonymsCap);
            }
            await ReplyAsync(embed: embed.Build());
        }

        private int CountGuilds()
        {
            return Context.Client.Guilds.Count;
        }

        private int CountUsers()
        {
            return Context.Client.Guilds.Sum(guild => guild.MemberCount);
        }

    }
}
