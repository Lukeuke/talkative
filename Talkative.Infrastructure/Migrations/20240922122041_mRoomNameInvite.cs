using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talkative.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mRoomNameInvite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoomName",
                table: "Invites",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomName",
                table: "Invites");
        }
    }
}
