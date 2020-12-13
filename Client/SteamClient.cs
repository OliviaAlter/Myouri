using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DiscordBot.Factory;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;

namespace DiscordBot.Client
{
    public class SteamClient : SteamFactory
    {
        private readonly SteamApps _steamApps;
        private readonly PlayerService _steamPlayerService;
        private readonly SteamUser _steamUser;
        private readonly SteamUserStats _steamUserStats;


        public SteamClient()
        {
            _steamPlayerService = Factory.CreateSteamWebInterface<PlayerService>(new HttpClient());
            _steamUserStats = Factory.CreateSteamWebInterface<SteamUserStats>(new HttpClient());
            _steamUser = Factory.CreateSteamWebInterface<SteamUser>(new HttpClient());
            _steamApps = Factory.CreateSteamWebInterface<SteamApps>(new HttpClient());
        }

        public async Task<int> SteamUserLevel(ulong steamId)
        {
            var response = await _steamPlayerService.GetSteamLevelAsync(steamId);
            if (response.Data != null)
                return (int) response.Data;
            return -1;
        }

        public async Task<string> SteamRecentGame(ulong steamId)
        {
            var response = await _steamPlayerService.GetRecentlyPlayedGamesAsync(steamId);
            var recentGame = response.Data.RecentlyPlayedGames.FirstOrDefault()?.Name;
            return recentGame ?? "None";
        }

        public async Task<uint> SteamRecentPlayTime(ulong steamId)
        {
            var response = await _steamPlayerService.GetRecentlyPlayedGamesAsync(steamId);
            return response.Data.RecentlyPlayedGames.First().Playtime2Weeks;
        }

        public async Task<string> SteamBadges(ulong steamId)
        {
            var response = await _steamPlayerService.GetBadgesAsync(steamId);
            return response.Data?.Badges.Count.ToString();
        }

        public async Task<string> SteamGamesOwned(ulong steamId)
        {
            var response = await _steamPlayerService.GetOwnedGamesAsync(steamId, false, true);
            return response.Data?.GameCount.ToString();
        }

        public async Task<string> SteamAvatarUrl(ulong steamId)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId);
            return response.Data?.AvatarFullUrl;
        }

        public async Task<string> SteamCountryCode(ulong steamId)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId);
            return response.Data?.CountryCode;
        }

        public async Task<string> SteamCreatedDate(ulong steamId)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId);
            return response.Data?.AccountCreatedDate.ToLongDateString();
        }

        public async Task<string> SteamLastLogin(ulong steamId)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId);
            return response.Data?.LastLoggedOffDate.ToLongDateString();
        }

        public async Task<string> SteamNickName(ulong steamId)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId);
            return response.Data?.Nickname;
        }

        public async Task<string> SteamRealName(ulong steamId)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId);
            return response.Data?.RealName;
        }

        public async Task<string> SteamPlayingNow(ulong steamId)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId);
            return response.Data?.PlayingGameName;
        }

        public async Task<string> SteamStatus(ulong steamId)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId);
            return response.Data?.UserStatus.ToString();
        }

        public async Task<ulong> SteamId(ulong steamId)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId);
            return response.Data.SteamId;
        }

        public async Task<string> SteamProfileUrl(ulong steamId)
        {
            var response = await _steamUser.GetPlayerSummaryAsync(steamId);
            return response.Data.ProfileUrl;
        }

        public async Task<string> SteamFriends(ulong steamId)
        {
            var response = await _steamUser.GetFriendsListAsync(steamId);
            return response.Data.ToString();
        }

        public async Task<string> SteamCustomUrl(ulong steamId)
        {
            var response = await _steamUser.GetCommunityProfileAsync(steamId);
            return response.CustomURL;
        }

        public async Task<double> SteamHoursPlayed(ulong steamId)
        {
            var response = await _steamUser.GetCommunityProfileAsync(steamId);
            return response.HoursPlayedLastTwoWeeks;
        }

        public async Task<string> SteamTradeBanState(ulong steamId)
        {
            var response = await _steamUser.GetCommunityProfileAsync(steamId);
            return response.TradeBanState;
        }

        public async Task<bool> SteamVacBan(ulong steamId)
        {
            var response = await _steamUser.GetCommunityProfileAsync(steamId);
            return response.IsVacBanned;
        }

        public async Task<bool> SteamLimitedAccount(ulong steamId)
        {
            var response = await _steamUser.GetCommunityProfileAsync(steamId);
            return response.IsLimitedAccount;
        }

        public async Task<List<SteamCommunityProfileMostPlayedGameModel>> SteamGameMostPlayed(ulong steamId)
        {
            var response = await _steamUser.GetCommunityProfileAsync(steamId);
            return response.MostPlayedGames.ToList();
        }

        public async Task<string> SteamInGameInfo(ulong steamId)
        {
            var response = await _steamUser.GetCommunityProfileAsync(steamId);
            return response.InGameInfo.GameName;
        }

        public async Task<ulong> SteamVanityUrl(string url)
        {
            var response = await _steamUser.ResolveVanityUrlAsync(url);
            return response.Data;
        }

        public async Task<uint> NumberOfPlayerForGame(uint steamGame)
        {
            var response = await _steamUserStats.GetNumberOfCurrentPlayersForGameAsync(steamGame);
            var data = response.Data;
            return data;
        }
    }
}