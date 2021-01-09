using Microsoft.EntityFrameworkCore;

namespace DatabaseEntity
{
    public class Database : DbContext
    {

        public Database(DbContextOptions<Database> options)
            : base(options)
        {
        }

        public DbSet<Server> Servers { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Autorole> Autoroles { get; set; }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql("Server=localhost;User=root;database=Discord;port=3306;Connect Timeout=5");
        */
    }

    public class Server
    {
        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public ulong LogMessageChannel { get; set; }
        public ulong EventLogChannel { get; set; }
        public ulong WelcomeChannel { get; set; }
        public ulong LeftChannel { get; set; }
        public ulong UserUpdateChannel { get; set; }
        public ulong WelcomeUrl { get; set; }
        public string WelcomeMessage { get; set; }
        public string LeaveMessage { get; set; }
        public bool InviteToggle { get; set; }
        public bool BadWordToggle { get; set; }
        public bool RoleMentionToggle { get; set; }
        public bool UserMentionToggle { get; set; }
    }

    public class User
    {
        public ulong Id { get; set; }
        public string UserName { get; set; }
        public ulong Exp { get; set; }
        public ulong Level { get; set; }
        public ulong SteamId { get; set; }
        public long OpenDotaId { get; set; }
    }

    public class Rank
    {
        public int Id { get; set; }
        public ulong RoleId { get; set; }
        public ulong ServerId { get; set; }
    }

    public class Autorole
    {
        public int Id { get; set; }
        public ulong RoleId { get; set; }
        public ulong ServerId { get; set; }
    }
}