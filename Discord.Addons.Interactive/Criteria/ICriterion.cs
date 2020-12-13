using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Discord.Addons.Interactive.Criteria
{
    public interface ICriterion<in T>
    {
        Task<bool> JudgeAsync(SocketCommandContext sourceContext, T parameter);
    }
}