using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseEntity.Migrations
{
    public partial class Guildupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MentionToggle",
                table: "Servers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MentionToggle",
                table: "Servers");
        }
    }
}
