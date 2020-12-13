using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseEntity;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Extension;
using DiscordBot.Services;
using OpenDotaDotNet;

namespace DiscordBot.Modules
{
    public class Dota2 : InteractiveBase<SocketCommandContext>
    {
        private readonly OpenDotaApi _openDotaApi;
        private readonly ScreenshotService _screenshotService;
        private readonly Users _users;

        public Dota2(OpenDotaApi openDotaApi, Users users, ScreenshotService screenshotService)
        {
            _openDotaApi = openDotaApi;
            _users = users;
            _screenshotService = screenshotService;
        }

        [Command("parse", RunMode = RunMode.Async)]
        [Alias("p")]
        public async Task ParseOpenDotaMatches(long matchId)
        {
            if (string.IsNullOrWhiteSpace(matchId.ToString()))
            {
                await Context.Channel.SendDotaErrorAsync("Found no input",
                    "Description :" +
                    "\nPlease input your openDota Id!\n" +
                    "\nTrivia : \n " +
                    "You can find your ID here : https://www.opendota.com/");
                return;
            }

            var parseRequest = await _openDotaApi.Request.SubmitNewParseRequestAsync(matchId);

            var parsedMatch = await _openDotaApi.Request.GetParseRequestStateAsync(parseRequest.Job.JobId);
            await Context.Channel.SendDotaSuccessAsync("Success",
                "**Your dota ID match has been parsed!**" +
                "\nYou can see your parsed match here : https://www.opendota.com/matches/" +
                $"{parsedMatch.Data.MatchId}");
        }


        [Command("dlink", RunMode = RunMode.Async)]
        [Alias("d")]
        public async Task BindOpenDotaId(long openDotaId)
        {
            if (string.IsNullOrWhiteSpace(openDotaId.ToString()))
            {
                await Context.Channel.SendDotaErrorAsync("Found no input",
                    "Description :" +
                    "\nPlease input your openDota Id!\n" +
                    "\nTrivia : \n " +
                    "You can find your ID here : https://www.opendota.com/");
                return;
            }

            var playerInfo = await _openDotaApi.Players.GetPlayerByIdAsync(openDotaId);
            if (playerInfo.Profile == null)
            {
                await Context.Channel.SendErrorDotaProfileAsync("Dota profile is hidden or not found",
                    "Please recheck your profile and then try again" +
                    "\nYou can get your ID at : https://www.opendota.com/ ");
                return;
            }

            var userId = Context.User.Id;
            await _users.SetUserDotaId(userId, openDotaId);
            await Context.Channel.SendDotaSuccessAsync("Success",
                "**Your dota ID has been bound to your account!**" +
                "\nEnjoy playing!");
        }

        [Command("rlink", RunMode = RunMode.Async)]
        [Alias("r")]
        public async Task RemoveOpenDotaId()
        {
            var userId = Context.User.Id;
            var userCheck = await _users.GetUserDotaId(userId);
            if (userCheck == 0)
            {
                await Context.Channel.SendDotaErrorAsync("Warning",
                    "**Your account does not associate with any ID !**");
                return;
            }

            await _users.RemoveDotaId(userId);
            await Context.Channel.SendDotaSuccessAsync("Success",
                "**Your dota ID has been removed from your account!**" +
                "\nHope to see you again!");
        }

        [Command("profile", RunMode = RunMode.Async)]
        public async Task GetDotaProfile(SocketGuildUser identifier = null)
        {
            identifier = (SocketGuildUser) (identifier ?? Context.User);
            var userDotaId = await _users.GetUserDotaId(identifier.Id);
            var playerInfo = await _openDotaApi.Players.GetPlayerByIdAsync(userDotaId);
            if (userDotaId == 0)
            {
                await Context.Channel.SendErrorDotaProfileAsync("You haven't bound any ID to your account yet",
                    "Please bind your ID and then try again" +
                    "\nYou can get your ID at : https://www.opendota.com/ ");
                return;
            }

            if (playerInfo.Profile == null)
            {
                await Context.Channel.SendErrorDotaProfileAsync("Dota profile is hidden or not found",
                    "Please recheck your input and then try again" +
                    "\n If you haven't bound your ID yet. Try binding first!" +
                    "\nYou can get your ID at : https://www.opendota.com/");
                return;
            }

            var playerWinLoss = await _openDotaApi.Players.GetPlayerWinLossByIdAsync(userDotaId);
            double win = playerWinLoss.Wins;
            double lose = playerWinLoss.Losses;
            var ratio = win / lose;
            var trueRatio = Math.Round(ratio, 2);
            if (lose == 0) trueRatio = 0;

            await Context.Channel.SendDotaProfile
            ($@"Basic details of player {playerInfo.Profile.Personaname}",
                $"Steam name : {playerInfo.Profile.Personaname}" +
                $"\nSteam ID: {playerInfo.Profile.Steamid}" +
                $"\nOpenDota ID : {userDotaId}" +
                $"\nSteam profile direct link: {playerInfo.Profile.Profileurl}" +
                $"\nDota 2 estimated MMR : {playerInfo.MmrEstimate.Estimate ?? 0}" +
                $"\nWin/Loss ratio : {trueRatio}" +
                $"\nTotal games played: {playerWinLoss.Wins + playerWinLoss.Losses}" +
                $"\nTotal wins: {playerWinLoss.Wins}" +
                $"\nTotal losses: {playerWinLoss.Losses}", $"{playerInfo.Profile.Avatarfull}");
        }

        [Command("profile", RunMode = RunMode.Async)]
        public async Task GetDotaProfile(long identifier)
        {
            var playerInfo = await _openDotaApi.Players.GetPlayerByIdAsync(identifier);
            if (playerInfo.Profile == null)
            {
                await Context.Channel.SendErrorDotaProfileAsync("Dota profile is hidden or not found",
                    "Please recheck your input and then try again" +
                    "\nYou can get your ID at : https://www.opendota.com/ ");
                return;
            }

            var playerWinLoss = await _openDotaApi.Players.GetPlayerWinLossByIdAsync(identifier);
            double win = playerWinLoss.Wins;
            double lose = playerWinLoss.Losses;
            var ratio = win / lose;
            var trueRatio = Math.Round(ratio, 2);
            if (lose == 0) trueRatio = 0;

            await Context.Channel.SendDotaProfile
            ($@"Basic details of player {playerInfo.Profile.Personaname}",
                $"Steam name : {playerInfo.Profile.Personaname}" +
                $"\nSteam ID: [Steam profile]({playerInfo.Profile.Steamid})" +
                $"\nOpenDota ID : {identifier}" +
                $"\nSteam profile direct link: {playerInfo.Profile.Profileurl}" +
                $"\nDota 2 estimated MMR : {playerInfo.MmrEstimate.Estimate ?? 0}" +
                $"\nWin/Loss ratio : {trueRatio}" +
                $"\nTotal games played: {playerWinLoss.Wins + playerWinLoss.Losses}" +
                $"\nTotal wins: {playerWinLoss.Wins}" +
                $"\nTotal losses: {playerWinLoss.Losses}", $"{playerInfo.Profile.Avatarfull}");
        }

        [Command("rc", RunMode = RunMode.Async)]
        public async Task GetRecentMatch(SocketGuildUser identifier = null)
        {
            identifier = (SocketGuildUser) (identifier ?? Context.User);
            var userDotaId = await _users.GetUserDotaId(identifier.Id);
            var playerInfo = await _openDotaApi.Players.GetPlayerByIdAsync(userDotaId);
            var matches
                = await _openDotaApi.Players.GetPlayerRecentMatchesAsync(userDotaId);
            var recentMatch = matches.FirstOrDefault();
            if (recentMatch == null)
            {
                await Context.Channel.SendErrorDotaProfileAsync("No recent match found",
                    "Bot can't find your recent match, maybe because your profile is :  " +
                    "\nHidden " +
                    "\nWrong ID " +
                    "\nHaven't bound ID to Discord" +
                    "\nYou can get your ID at : https://www.opendota.com/ ");
                return;
            }

            var teamScore = await _openDotaApi.Matches.GetMatchByIdAsync(recentMatch.MatchId);
            var winner = recentMatch.RadiantWin ? "Radian won" : "Dire won";

            string[] gameMode =
            {
                "Unknown", "All pick", "Captains", "Random draft", "Single draft", "All random", "Intro", "Diretide",
                "Reverse captains", "Greeviling", "Tutorial", "Mid-only", "Least played", "Limited heroes",
                "Compendium match", "Custom", "Captains draft", "Balanced draft", "Ability draft", "Event",
                "All random death match", "1v1 mid", "All draft", "Tubro", "Mutation"
            };

            await Context.Channel.SendDotaProfile
            ($@"Recent match of player {playerInfo.Profile.Personaname}",
                $"\nMatch Id : {recentMatch.MatchId}" +
                $"\nGame mode : {recentMatch.GameMode}" +
                $"\nTotal kills : {recentMatch.Kills}" +
                $"\nTotal assists: {recentMatch.Assists}" +
                $"\nTotal deaths : {recentMatch.Deaths}" +
                $"\nDuration for this match : {TimeSpan.FromSeconds(recentMatch.Duration):mm:ss}" +
                $"\nGPM : {recentMatch.GoldPerMin}" +
                $"\nXPM : {recentMatch.XpPerMin}" +
                $"\nTotal damage dealt to heroes : {recentMatch.HeroDamage}" +
                $"\nTotal damage dealt to building : {recentMatch.TowerDamage}" +
                $"\nTotal healing in this match : {recentMatch.HeroHealing}" +
                $"\nWinner : {winner}" +
                $@"Radiant Score: {teamScore.RadiantScore}. Dire Score: {teamScore.DireScore}."
                , $"{playerInfo.Profile.Avatarfull}");

            /*
            Console.WriteLine("Player heroes");
            var playerQueryParameters = new PlayerEndpointParameters
            {
                Limit = 20
            };
            var playerHeroes = await _openDotaApi.Players.GetPlayerHeroesAsync(openDotaId, playerQueryParameters);

            var playerMostPlayedHeroLast20 = playerHeroes.FirstOrDefault();

            if (playerMostPlayedHeroLast20 != null)
            {
                Console.WriteLine(
                    $@"Most played hero in the last 20 matches is hero : {playerMostPlayedHeroLast20.HeroId} with {playerMostPlayedHeroLast20.Win} wins.");
            }

            Console.WriteLine("Player heroes");

            var matchDetails = await _openDotaApi.Matches.GetMatchByIdAsync(recentMatch.MatchId);

            Console.WriteLine($@"Details about match id {matchId}.");
            Console.WriteLine($@"Duration of game: {TimeSpan.FromSeconds(matchDetails.Duration):mm\:ss}.");
            Console.WriteLine($@"Radiant Score: {matchDetails.RadiantScore}. Dire Score: {matchDetails.DireScore}.");

            Console.WriteLine($@"Nickname of players in the game:");
            foreach (var player in matchDetails.Players)
            {
                Console.WriteLine(string.IsNullOrEmpty(player.Personaname) ? "Anonymous" : $@"{player.Personaname}");
            }
            */
        }


        [Command("live", RunMode = RunMode.Async)]
        public async Task GetLiveMatch()
        {
            var liveMatch = await _openDotaApi.Live.GetTopLiveGamesAsync();
            var topLive = liveMatch.FirstOrDefault();
        }

        [Command("fetch", RunMode = RunMode.Async)]
        public async Task FetchImage(ulong matchId)
        {
            var path = await _screenshotService.ShootingWebsiteTask(matchId);
            Console.WriteLine(path);
            await Context.Channel.SendFileAsync(path);
        }
    }
}