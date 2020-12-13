using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace DiscordBot.Services
{
    public class ScreenshotService
    {
        public async Task<string> ShootingWebsiteTask(ulong matchId)
        {
            var options = new LaunchOptions
            {
                Headless = false
            };

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            await using (var browser = await Puppeteer.LaunchAsync(options))
            await using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync("https://www.opendota.com/matches/" + $"{matchId}");
                await page.ScreenshotAsync($"{Guid.NewGuid()}.png");
            }

            var image = Directory.GetFiles($"{Directory.GetCurrentDirectory()}", $"{Guid.NewGuid()}.png")
                .FirstOrDefault();
            return await Task.FromResult(image);
        }
    }
}