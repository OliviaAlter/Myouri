using System.IO;
using System.Threading.Tasks;
using DatabaseEntity;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Client;
using DiscordBot.Discord.Addons.Interactive;
using DiscordBot.Extension;
using DiscordBot.Services;
using DiscordBot.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHentai.NET.Client;
using NHentai.NET.Helpers;
using OpenDotaDotNet;

namespace DiscordBot
{
    public class Program
    {
        private static async Task Main()
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsetting.json", false, true)
                        .Build();

                    x.AddConfiguration(configuration);
                })
                .ConfigureLogging(x =>
                {
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel
                        .Warning); // Defines what kind of information should be logged (e.g. Debug, Information, Warning, Critical) adjust this to your liking
                })
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity
                            .Warning, // Defines what kind of information should be logged from the API (e.g. Verbose, Info, Warning, Critical) adjust this to your liking
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200,
                        ExclusiveBulkDelete = true,
                        RateLimitPrecision = 0
                    };

                    config.Token = context.Configuration["token"];
                })
                .UseCommandService((context, config) =>
                {
                    config.CaseSensitiveCommands = false;
                    config.LogLevel = LogSeverity.Warning;
                    config.DefaultRunMode = RunMode.Sync;
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddHostedService<CommandHandler>()
                        //.AddDbContext<Database>()
                        .AddDbContext<Database>(options => options.UseMySql(context.Configuration["database"]))
                        .AddSingleton<Servers>()
                        .AddSingleton<Ranks>()
                        .AddSingleton<Users>()
                        .AddSingleton<Autoroles>()
                        .AddSingleton<AutoRolesHelperClass>()
                        .AddSingleton<RanksHelperClass>()
                        .AddSingleton<ImageService>()
                        .AddSingleton<RequestOptions>()
                        .AddSingleton<InteractiveService>()
                        .AddSingleton<OpenDotaApi>()
                        .AddSingleton<SteamClient>()
                        .AddSingleton<ScreenshotService>()
                        .AddSingleton<TableExtension>()
                        .AddHentaiClient()
                        /*
                        .AddLavaNode(x =>
                        {
                            x.ReconnectAttempts = 3;
                            x.SelfDeaf = true;
                            x.EnableResume = true;
                            x.LogSeverity = LogSeverity.Debug;
                        })*/;
                })
                .UseConsoleLifetime();
            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}