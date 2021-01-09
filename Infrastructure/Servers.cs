using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatabaseEntity
{
    public class Servers
    {
        private readonly Database _context;

        public Servers(Database context)
        {
            _context = context;
        }

        #region Guild prefix
        public async Task ModifyGuildPrefix(ulong id, string prefix)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server {Id = id, Prefix = prefix});
            else
                server.Prefix = prefix;
            await _context.SaveChangesAsync();
        }
       
        public async Task<string> GetGuildPrefix(ulong id)
        {
            var prefix = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.Prefix)
                .FirstOrDefaultAsync();
            return await Task.FromResult(prefix);
        }
        #endregion

        #region Event log channel
        public async Task<ulong> GetEventLogChannel(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.EventLogChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task SetEventLogChannel(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null) _context.Add(new Server {Id = id, EventLogChannel = channelId});
            await _context.SaveChangesAsync();
        }

        public async Task ModifyEventLogChannel(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server {Id = id, EventLogChannel = channelId});
            else
                server.EventLogChannel = channelId;
            await _context.SaveChangesAsync();
        }
       
        public async Task RemoveEventLogChannel(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.EventLogChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Message log channel 
        public async Task<ulong> GetLogMessageChannel(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.LogMessageChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task ModifyMessageLogChannel(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server {Id = id, LogMessageChannel = channelId});
            else
                server.LogMessageChannel = channelId;
            await _context.SaveChangesAsync();
        }

        public async Task SetLogMessageChannel(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null) _context.Add(new Server {Id = id, LogMessageChannel = channelId});
            await _context.SaveChangesAsync();
        }

        public async Task RemoveLogMessageChannel(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.LogMessageChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Welcome channel
        public async Task<ulong> GetWelcomeChannel(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.WelcomeChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task SetWelcomeChannel(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null) _context.Add(new Server {Id = id, WelcomeChannel = channelId});
            await _context.SaveChangesAsync();
        }

        public async Task ModifyWelcomeChannel(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server {Id = id, WelcomeChannel = channelId});
            else
                server.WelcomeChannel = channelId;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveWelcomeChannel(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.WelcomeChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Leave channel 
        public async Task<ulong> GetLeftChannel(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.LeftChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task SetLeftChannel(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null) _context.Add(new Server {Id = id, LeftChannel = channelId});
            await _context.SaveChangesAsync();
        }

        public async Task ModifyLeftChannel(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server {Id = id, LeftChannel = channelId});
            else
                server.LeftChannel = channelId;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveLeftChannel(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.LeftChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region User log channel 
        public async Task<ulong> GetUserLogChannel(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.UserUpdateChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task SetUserLogChannel(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null) _context.Add(new Server {Id = id, UserUpdateChannel = channelId});
            await _context.SaveChangesAsync();
        }

        public async Task ModifyUserLogChannel(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server {Id = id, UserUpdateChannel = channelId});
            else
                server.UserUpdateChannel = channelId;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserLogChannel(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.UserUpdateChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region URL welcome 
        public async Task<ulong> GetWelcomeUrl(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.UserUpdateChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task SetWelcomeUrl(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null) _context.Add(new Server { Id = id, UserUpdateChannel = channelId });
            await _context.SaveChangesAsync();
        }

        public async Task ModifyWelcomeUrl(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server { Id = id, UserUpdateChannel = channelId });
            else
                server.UserUpdateChannel = channelId;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveWelcomeUrl(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.UserUpdateChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Welcome message
        public async Task<ulong> GetWelcomeMessage(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.UserUpdateChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task SetWelcomeMessage(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null) _context.Add(new Server { Id = id, UserUpdateChannel = channelId });
            await _context.SaveChangesAsync();
        }

        public async Task ModifyWelcomeMessage(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server { Id = id, UserUpdateChannel = channelId });
            else
                server.UserUpdateChannel = channelId;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveWelcomeMessage(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.UserUpdateChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Leave message
        public async Task<ulong> GetLeaveMessage(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.UserUpdateChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task SetLeaveMessage(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null) _context.Add(new Server { Id = id, UserUpdateChannel = channelId });
            await _context.SaveChangesAsync();
        }

        public async Task ModifyLeaveMessage(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server { Id = id, UserUpdateChannel = channelId });
            else
                server.UserUpdateChannel = channelId;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveLeaveMessage(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.UserUpdateChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Guild user mention per message
        public async Task ModifyGuildUserMessageMention(ulong id, string prefix)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server { Id = id, Prefix = prefix });
            else
                server.Prefix = prefix;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetGuildUserMessageMention(ulong id)
        {
            var prefix = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.Prefix)
                .FirstOrDefaultAsync();
            return await Task.FromResult(prefix);
        }

        public async Task<ulong> SetGuildUserMessageMention(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.EventLogChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task RemoveGuildUserMessageMention(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.UserUpdateChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Guild role mention per message
        public async Task ModifyGuildRoleMessageMention(ulong id, string prefix)
        {
            var server = await _context.Servers
                .FindAsync(id);
            if (server == null)
                _context.Add(new Server { Id = id, Prefix = prefix });
            else
                server.Prefix = prefix;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetGuildRoleMessageMention(ulong id)
        {
            var prefix = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.Prefix)
                .FirstOrDefaultAsync();
            return await Task.FromResult(prefix);
        }

        public async Task<ulong> SetGuildRoleMessageMention(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.EventLogChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task RemoveGuildRoleMessageMention(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.UserUpdateChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Disable/Enable user mention per message
        public async Task<string> GetGuildMessageMentionUser(ulong id)
        {
            var prefix = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.Prefix)
                .FirstOrDefaultAsync();
            return await Task.FromResult(prefix);
        }

        public async Task<ulong> SetMessageMessageMentionUser(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.EventLogChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task RemoveMessageMentionUser(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.UserUpdateChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Disable/Enable role mention per message
        public async Task<string> GetGuildMessageMentionRole(ulong id)
        {
            var prefix = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.Prefix)
                .FirstOrDefaultAsync();
            return await Task.FromResult(prefix);
        }

        public async Task<ulong> SetMessageMessageMentionRole(ulong id)
        {
            var channelId = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.EventLogChannel)
                .FirstOrDefaultAsync();
            return await Task.FromResult(channelId);
        }

        public async Task RemoveMessageMentionRole(ulong id, ulong channelId)
        {
            var channel = await _context.Servers
                .FindAsync(id);
            channel.UserUpdateChannel = 0;
            await _context.SaveChangesAsync();
        }
        #endregion

    }
}