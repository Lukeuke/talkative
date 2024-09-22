using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talkative.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mUserOnlineStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OnlineStatus",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlineStatus",
                table: "Users");
        }
    }
}
