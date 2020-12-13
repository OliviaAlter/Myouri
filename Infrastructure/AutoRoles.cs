using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatabaseEntity
{
    public class Autoroles
    {
        private readonly Database _context;

        public Autoroles(Database context)
        {
            _context = context;
        }

        public async Task<List<Autorole>> GetAutoRolesAsync(ulong id)
        {
            var autoRoles = await _context.Autoroles
                .Where(x => x.ServerId == id)
                .ToListAsync();

            return await Task.FromResult(autoRoles);
        }

        public async Task AddAutoRolesAsync(ulong id, ulong roleId)
        {
            var server = await _context.Servers
                .FindAsync(id);

            if (server == null)
                _context.Add(new Server {Id = id});

            _context.Add(new Autorole {RoleId = roleId, ServerId = id});
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAutoRolesAsync(ulong id, ulong roleId)
        {
            var autoRoles = await _context.Autoroles
                .Where(x => x.RoleId == roleId)
                .FirstOrDefaultAsync();

            _context.Remove(autoRoles);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAutoRolesAsync(List<Autorole> autoRoles)
        {
            _context.RemoveRange(autoRoles);
            await _context.SaveChangesAsync();
        }
    }
}