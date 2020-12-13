using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Discord.Addons.Interactive.Criteria;

namespace DiscordBot.Discord.Addons.Interactive.Paginator
{
    internal class EnsureIsIntegerCriterion : ICriterion<SocketMessage>
    {
        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, SocketMessage parameter)
        {
            var ok = int.TryParse(parameter.Content, out _);
            return Task.FromResult(ok);
        }
    }
}