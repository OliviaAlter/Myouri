using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatabaseEntity
{
    public class Users
    {
        private readonly Database _context;

        public Users(Database context)
        {
            _context = context;
        }

        public async Task<long> GetUserDotaId(ulong userId)
        {
            var openDotaId = await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.OpenDotaId)
                .FirstOrDefaultAsync();
            return await Task.FromResult(openDotaId);
        }

        public async Task SetUserDotaId(ulong userId, long openDotaId)
        {
            var user = await _context.Users
                .FindAsync(userId);
            if (user == null) _context.Add(new User {Id = userId, OpenDotaId = openDotaId});
            await _context.SaveChangesAsync();
        }

        public async Task ModifyUserDotaId(ulong userId, long openDotaId)
        {
            var user = await _context.Users
                .FindAsync(userId);
            if (user == null)
                _context.Add(new User {Id = userId, OpenDotaId = openDotaId});
            else
                user.OpenDotaId = openDotaId;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDotaId(ulong userId)
        {
            var user = await _context.Users
                .FindAsync(userId);
            if (user == null) _context.Add(new User {Id = userId, OpenDotaId = 0});
            await _context.SaveChangesAsync();
        }
    }
}