using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseEntity;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Utilities
{
    public class RanksHelperClass
    {
        private readonly Ranks _ranks;

        public RanksHelperClass(Ranks ranks)
        {
            _ranks = ranks;
        }

        public async Task<List<IRole>> GetRankAsync(IGuild guild)
        {
            var roles = new List<IRole>();
            var invalidRanks = new List<Rank>();

            var ranks = await _ranks.GetRankAsync(guild.Id);

            foreach (var rank in ranks)
            {
                var role = guild.Roles.FirstOrDefault(x => x.Id == rank.RoleId);
                if (role == null)
                {
                    invalidRanks.Add(rank);
                }
                else
                {
                    var currentUser = await guild.GetCurrentUserAsync();
                    var hierarchy = ((SocketGuildUser) currentUser).Hierarchy;
                    if (role.Position > hierarchy)
                        invalidRanks.Add(rank);
                    else
                        roles.Add(role);
                }
            }

            if (invalidRanks.Count > 0)
                await _ranks.ClearRankAsync(invalidRanks);
            return roles;
        }
    }
}