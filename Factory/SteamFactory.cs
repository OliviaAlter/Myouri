using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SteamWebAPI2.Utilities;

namespace DiscordBot.Factory
{
    public class SteamFactory
    {
        protected readonly SteamWebInterfaceFactory Factory;

        public SteamFactory()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsetting.json");
            IConfiguration configuration = builder.Build();
            var factoryOptions = new SteamWebInterfaceFactoryOptions
            {
                SteamWebApiKey = configuration["SteamWebApiKey"]
            };
            Factory = new SteamWebInterfaceFactory(Options.Create(factoryOptions));
        }
    }
}