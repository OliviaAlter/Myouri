using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Client;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Extension;

namespace DiscordBot.Modules
{
    public class Steam : InteractiveBase<SocketCommandContext>
    {
        private readonly SteamClient _steamClient;

        public Steam(SteamClient steamClient)
        {
            _steamClient = steamClient;
        }

        [Command("steam", RunMode = RunMode.Async)]
        public async Task GetSteamProfile(string steamIdentifier)
        {
            try
            {
                if (ulong.TryParse(steamIdentifier, out var steamId))
                {
                    var level = await _steamClient.SteamUserLevel(steamId);
                    var recentGame = await _steamClient.SteamRecentGame(steamId);
                    var defaultUrl = await _steamClient.SteamProfileUrl(steamId);
                    var customUrl = await _steamClient.SteamCustomUrl(steamId);
                    var createdDate = await _steamClient.SteamCreatedDate(steamId);
                    var lastLogin = await _steamClient.SteamLastLogin(steamId);
                    var avatarUrl = await _steamClient.SteamAvatarUrl(steamId);
                    var isTradeBan = await _steamClient.SteamTradeBanState(steamId);
                    var isVacBan = await _steamClient.SteamVacBan(steamId);
                    var isLimited = await _steamClient.SteamLimitedAccount(steamId);
                    var nickName = await _steamClient.SteamNickName(steamId);

                    await Context.Channel.SendSteamProfile($"Detail steam profile of [{nickName}]",
                        $"\nSteam ID : {steamId}" +
                        $"\nSteam name : {nickName}" +
                        $"\nSteam level : {level}" +
                        $"\nSteam profile link : [Steam Profile]({defaultUrl ?? customUrl})" +
                        $"\nCreated on : {createdDate}" +
                        $"\nLast login : {lastLogin}" +
                        $"\nRecently played : {recentGame}" +
                        $"\nVac ban : {isVacBan}" +
                        $"\nTrade ban : {isTradeBan} " +
                        $"\nLimited account : {isLimited}", avatarUrl);
                }
                else
                {
                    var vanityUrlDecoder = await _steamClient.SteamVanityUrl(steamIdentifier);

                    var level = await _steamClient.SteamUserLevel(vanityUrlDecoder);
                    var recentGame = await _steamClient.SteamRecentGame(vanityUrlDecoder);
                    var defaultUrl = await _steamClient.SteamProfileUrl(vanityUrlDecoder);
                    var customUrl = await _steamClient.SteamCustomUrl(vanityUrlDecoder);
                    var createdDate = await _steamClient.SteamCreatedDate(vanityUrlDecoder);
                    var lastLogin = await _steamClient.SteamLastLogin(vanityUrlDecoder);
                    var avatarUrl = await _steamClient.SteamAvatarUrl(vanityUrlDecoder);
                    var isTradeBan = await _steamClient.SteamTradeBanState(vanityUrlDecoder);
                    var isVacBan = await _steamClient.SteamVacBan(vanityUrlDecoder);
                    var isLimited = await _steamClient.SteamLimitedAccount(vanityUrlDecoder);
                    var nickName = await _steamClient.SteamNickName(vanityUrlDecoder);
                    var steamVanityId = await _steamClient.SteamId(vanityUrlDecoder);

                    await Context.Channel.SendSteamProfile($"Detail steam profile of [{nickName}]",
                        $"\nSteam ID : {steamVanityId}" +
                        $"\nSteam name : {nickName}" +
                        $"\nSteam level : {level}" +
                        $"\nSteam profile link : {defaultUrl ?? customUrl}" +
                        $"\nCreated on : {createdDate}" +
                        $"\nLast login : {lastLogin}" +
                        $"\nRecently played : {recentGame}" +
                        $"\nVac ban : {isVacBan}" +
                        $"\nTrade ban : {isTradeBan} " +
                        $"\nLimited account : {isLimited}", avatarUrl);
                }
            }
            catch
            {
                await Context.Channel.SendErrorSteamProfileAsync("Steam profile not found",
                    "Please recheck your profile's id and then try again");
            }
        }

        /*
        [Command("player")]
        public async Task GetPlayerFromGame(uint steamGame)
        {
            //try 440
            await Context.Channel.
                SendMessageAsync(
                    $"{await _steamClient.NumberOfPlayerForGame(steamGame)} players");
        }
        */
    }
}