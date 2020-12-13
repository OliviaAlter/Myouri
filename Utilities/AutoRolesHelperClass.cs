using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseEntity;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Utilities
{
    public class AutoRolesHelperClass
    {
        private readonly Autoroles _autoRoles;

        public AutoRolesHelperClass(Autoroles autoRoles)
        {
            _autoRoles = autoRoles;
        }

        public async Task<List<IRole>> GetAutoRolesAsync(IGuild guild)
        {
            var roles = new List<IRole>();
            var invalidAutoRoles = new List<Autorole>();

            var autoRoles = await _autoRoles.GetAutoRolesAsync(guild.Id);

            foreach (var autoRole in autoRoles)
            {
                var role = guild.Roles.FirstOrDefault(x => x.Id == autoRole.RoleId);
                if (role == null)
                {
                    invalidAutoRoles.Add(autoRole);
                }
                else
                {
                    var currentUser = await guild.GetCurrentUserAsync();
                    var hierarchy = ((SocketGuildUser) currentUser).Hierarchy;
                    if (role.Position > hierarchy)
                        invalidAutoRoles.Add(autoRole);
                    else
                        roles.Add(role);
                }
            }

            if (invalidAutoRoles.Count > 0)
                await _autoRoles.ClearAutoRolesAsync(invalidAutoRoles);
            return roles;
        }
    }
}