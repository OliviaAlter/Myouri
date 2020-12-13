using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Utilities;
using Nekos.Net;
using Nekos.Net.Endpoints;

namespace DiscordBot.Modules
{
    [Summary(":sa:")]
    public class Nsfw : InteractiveBase<SocketCommandContext>
    {
        [Command("switch", RunMode = RunMode.Async)]
        [Summary("Enable or disable nsfw")]
        [Alias("switch")]
        public async Task NsfwSwitch()
        {
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageRoles)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var channel = (ITextChannel) Context.Channel;
            await channel.ModifyAsync(r => r.IsNsfw = !IsNsfwChannel());
        }

        [Command("pussy")]
        [Summary("Get pussy nsfw!!")]
        [Alias("pussy")]
        public async Task NekoPussy()
        {
            await BadAtNaming(NsfwEndpoint.Pussy, Context.Channel).ConfigureAwait(false);
        }

        [Command("lewd")]
        [Summary("Get lewd nsfw!!")]
        [Alias("lewds")]
        public async Task NekoLewd()
        {
            await BadAtNaming(NsfwEndpoint.Lewd, Context.Channel).ConfigureAwait(false);
        }

        /*
        [Command("les")]
        [Summary("Get les nsfw!!")]
        [Alias("les")]
        public async Task NekoLes()
        {
            await BadAtNaming(NsfwEndpoint.Les, (ITextChannel)Context.Channel);
        }
        
        [Command("kuni")]
        [Summary("Get kuni nsfw!!")]
        [Alias("kuni")]
        public async Task NekoKuni()
        {
            await BadAtNaming(NsfwEndpoint.Kuni, (ITextChannel)Context.Channel);
        }
        */
        [Command("cum")]
        [Summary("Get cum nsfw!!")]
        [Alias("cums")]
        public async Task NekoCum()
        {
            await BadAtNaming(NsfwEndpoint.Cum, Context.Channel).ConfigureAwait(false);
        }

        /*
        [Command("classic")]
        [Summary("Get classic nsfw!!")]
        [Alias("classic")]
        public async Task NekoClassic()
        {
            await BadAtNaming(NsfwEndpoint.Classic, (ITextChannel)Context.Channel);
        }
        */
        [Command("boob")]
        [Summary("Get boob nsfw!!")]
        [Alias("boobs")]
        public async Task NekoBoob()
        {
            await BadAtNaming(NsfwEndpoint.Boobs, Context.Channel).ConfigureAwait(false);
        }

        [Command("bj")]
        [Summary("Get bj nsfw!!")]
        [Alias("bjs")]
        public async Task NekoBj()
        {
            await BadAtNaming(NsfwEndpoint.Bj, Context.Channel).ConfigureAwait(false);
        }

        [Command("anal")]
        [Summary("Get anal nsfw!!")]
        [Alias("anals")]
        public async Task NekoAnal()
        {
            await BadAtNaming(NsfwEndpoint.Anal, Context.Channel).ConfigureAwait(false);
        }

        [Command("nsfwnekogif")]
        [Summary("Get nsfwnekogifs nsfw!!")]
        [Alias("nsfwnekogifs")]
        public async Task NekoNsfwNekoGif()
        {
            await BadAtNaming(NsfwEndpoint.NsfwNekoGif, Context.Channel).ConfigureAwait(false);
        }

        /*
        [Command("nsfwavatar")]
        [Summary("Get nsfwavatar nsfw!!")]
        [Alias("nsfwavatars")]
        public async Task NekoNsfwAvatar()
        {
            await BadAtNaming(NsfwEndpoint.NsfwAvatar, (ITextChannel)Context.Channel);
        }
        */
        [Command("yuri")]
        [Summary("Get yuri nsfw!!")]
        [Alias("yuri")]
        public async Task NekoYuri()
        {
            await BadAtNaming(NsfwEndpoint.Yuri, Context.Channel).ConfigureAwait(false);
        }

        [Command("trap")]
        [Summary("Get trap nsfw!!")]
        [Alias("traps")]
        public async Task NekoTrap()
        {
            await BadAtNaming(NsfwEndpoint.Trap, Context.Channel).ConfigureAwait(false);
        }

        /*
        [Command("tit")]
        [Summary("Get tits nsfw!!")]
        [Alias("tits")]
        public async Task NekoTit()
        {
            await BadAtNaming(NsfwEndpoint.Tits, (ITextChannel)Context.Channel);
        }
        
        [Command("solog")]
        [Summary("Get solog nsfw!!")]
        [Alias("solog")]
        public async Task NekoSoloG()
        {
            await BadAtNaming(NsfwEndpoint.SoloG, (ITextChannel)Context.Channel);
        }
        
        [Command("solo")]
        [Summary("Get solo nsfw!!")]
        [Alias("solo")]
        public async Task NekoSolo()
        {
            await BadAtNaming(NsfwEndpoint.Solo, (ITextChannel)Context.Channel);
        }

        [Command("smallboob")]
        [Summary("Get smallboobs nsfw!!")]
        [Alias("smallboobs")]
        public async Task NekoSmallBoobs()
        {
            await BadAtNaming(NsfwEndpoint.SmallBoobs, (ITextChannel)Context.Channel);
        }
        
        [Command("PWankG")]
        [Summary("Get pwankg nsfw!!")]
        [Alias("pwankg")]
        public async Task NekoPWankG()
        {
            await BadAtNaming(NsfwEndpoint.PWankG, (ITextChannel)Context.Channel);
        }
        
        [Command("pussypic")]
        [Summary("Get pussypic nsfw!!")]
        [Alias("pussypics")]
        public async Task NekoPussyPic()
        {
            await BadAtNaming(NsfwEndpoint.Pussy_JPG, (ITextChannel)Context.Channel);
        }

        [Command("lewdkemo")]
        [Summary("Get Ero lewdkemo nsfw!!")]
        [Alias("lewdkemo")]
        public async Task NekoLewdKemo()
        {
            await BadAtNaming(NsfwEndpoint.Lewd_Kemo, (ITextChannel)Context.Channel);
        }

        [Command("lewdk")]
        [Summary("Get lewdk nsfw!!")]
        [Alias("lewdk")]
        public async Task NekoLewdK()
        {
            await BadAtNaming(NsfwEndpoint.Lewd_K, (ITextChannel)Context.Channel);
        }

        [Command("keta")]
        [Summary("Get keta nsfw!!")]
        [Alias("keta")]
        public async Task NekoKeta()
        {
            await BadAtNaming(NsfwEndpoint.Keta, (ITextChannel)Context.Channel);
        }
        
        [Command("hololewd")]
        [Summary("Get hololewds nsfw!!")]
        [Alias("hololewds")]
        public async Task NekoHoloLewd()
        {
            await BadAtNaming(NsfwEndpoint.HoloLewd, (ITextChannel)Context.Channel);
        }

        [Command("holoero")]
        [Summary("Get holoero nsfw!!")]
        [Alias("holoeros")]
        public async Task NekoHoloEro()
        {
            await BadAtNaming(NsfwEndpoint.HoloEro, (ITextChannel)Context.Channel);
        }
        */
        [Command("hentai")]
        [Summary("Get hentais nsfw!!")]
        [Alias("hentais")]
        public async Task NekoHentai()
        {
            await BadAtNaming(NsfwEndpoint.Hentai, Context.Channel).ConfigureAwait(false);
        }

        [Command("futanari")]
        [Summary("Get futanari nsfw!!")]
        [Alias("futanari")]
        public async Task NekoFutanari()
        {
            await BadAtNaming(NsfwEndpoint.Futanari, Context.Channel).ConfigureAwait(false);
        }

        [Command("femdom")]
        [Summary("Get femdom nsfw!!")]
        [Alias("femdom")]
        public async Task NekoFemdom()
        {
            await BadAtNaming(NsfwEndpoint.Femdom, Context.Channel).ConfigureAwait(false);
        }

        /*
        [Command("feetg")]
        [Summary("Get feetg nsfw!!")]
        [Alias("feetg")]
        public async Task NekoFeetG()
        {
            await BadAtNaming(NsfwEndpoint.FeetG, (ITextChannel)Context.Channel);
        }
        
        [Command("erofeet")]
        [Summary("Get erofeet nsfw!!")]
        [Alias("erofeet")]
        public async Task NekoEroFeet()
        {
            await BadAtNaming(NsfwEndpoint.EroFeet, (ITextChannel)Context.Channel);
        }
        */
        [Command("ero")]
        [Summary("Get eroge nsfw!!")]
        [Alias("ero", "eroge")]
        public async Task NekoEro()
        {
            await BadAtNaming(NsfwEndpoint.Ero, Context.Channel).ConfigureAwait(false);
        }

/*
        [Command("erok")]
        [Summary("Get erok nsfw!!")]
        [Alias("erok")]
        public async Task NekoEroK()
        {
            await BadAtNaming(NsfwEndpoint.EroK, (ITextChannel)Context.Channel);
        }
        [Command("eron")]
        [Summary("Get eron nsfw!!")]
        [Alias("eron")]
        public async Task NekoEroN()
        {
            await BadAtNaming(NsfwEndpoint.EroN, (ITextChannel)Context.Channel);
        }
*/
        [Command("eroyuri")]
        [Summary("Get eroyuri nsfw!!")]
        [Alias("eyuri")]
        public async Task NekoEroYuri()
        {
            await BadAtNaming(NsfwEndpoint.EroYuri, Context.Channel).ConfigureAwait(false);
        }

        /*
        [Command("cum")]
        [Summary("Get cum nsfw!!")]
        [Alias("cumming")]
        public async Task NekoCumPic()
        {
            await BadAtNaming(NsfwEndpoint.Cum_JPG, (ITextChannel)Context.Channel);
        }

        [Command("blowjob")]
        [Summary("Get blowjob nsfw!!")]
        [Alias("blowjob")]
        public async Task NekoBlowjob()
        {
            await BadAtNaming(NsfwEndpoint.Blowjob, (ITextChannel)Context.Channel);
        }
        */
        private async Task BadAtNaming(NsfwEndpoint endpoint, ISocketMessageChannel channel)
        {
            /*
            if (!IsNsfwChannel() && Context.User is SocketGuildUser userSend && userSend.GuildPermissions.ManageChannels)
            {
                await ReplyAsync("Do you want to enable NSFW on this channel ?");
                var response = await NextMessageAsync(timeout: TimeSpan.FromSeconds(10));
                if (response == null) return;
                if (response.ToString().ToLower().Equals("yes")) await ((ITextChannel)channel).ModifyAsync(r => r.IsNsfw = true);
            }
            */
            if (IsNsfwChannel())
                try
                {
                    var author = Context.User;
                    var image = await NekosClient.GetNsfwAsync(endpoint);
                    await ReplyAsync(null, false,
                        new EmbedBuilder()
                            .WithDescription($"{author.Mention}, here you go '{endpoint.ToString().Replace("_", " ")}'")
                            .WithImageUrl(image.FileUrl).Build());
                }
                catch
                {
                    await ReplyAsync("Error !!");
                }
            else
                await ReplyAsync(
                    "You can only use this command in NSFW channel!! You can use *switch to enable nsfw here");
        }

        private bool IsNsfwChannel()
        {
            return ((ITextChannel) Context.Channel).IsNsfw;
        }
    }
}