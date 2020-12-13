using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseEntity.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Autoroles",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<ulong>(nullable: false),
                    ServerId = table.Column<ulong>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Autoroles", x => x.Id); });

            migrationBuilder.CreateTable(
                "Ranks",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<ulong>(nullable: false),
                    ServerId = table.Column<ulong>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Ranks", x => x.Id); });

            migrationBuilder.CreateTable(
                "Servers",
                table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Prefix = table.Column<string>(nullable: true),
                    LogMessageChannel = table.Column<ulong>(nullable: false),
                    EventLogChannel = table.Column<ulong>(nullable: false),
                    WelcomeChannel = table.Column<ulong>(nullable: false),
                    LeftChannel = table.Column<ulong>(nullable: false),
                    UserUpdateChannel = table.Column<ulong>(nullable: false),
                    WelcomeUrl = table.Column<ulong>(nullable: false),
                    WelcomeMessage = table.Column<string>(nullable: true),
                    LeaveMessage = table.Column<string>(nullable: true),
                    InviteToggle = table.Column<bool>(nullable: false),
                    BadWordToggle = table.Column<bool>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Servers", x => x.Id); });

            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: true),
                    Exp = table.Column<ulong>(nullable: false),
                    Level = table.Column<ulong>(nullable: false),
                    SteamId = table.Column<ulong>(nullable: false),
                    OpenDotaId = table.Column<long>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Users", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Autoroles");

            migrationBuilder.DropTable(
                "Ranks");

            migrationBuilder.DropTable(
                "Servers");

            migrationBuilder.DropTable(
                "Users");
        }
    }
}