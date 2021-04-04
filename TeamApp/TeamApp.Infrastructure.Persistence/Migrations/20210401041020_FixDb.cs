using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class FixDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFile",
                table: "message",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMessage",
                table: "message",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GroupChatUserSeen",
                table: "group_chat_user",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFile",
                table: "message");

            migrationBuilder.DropColumn(
                name: "IsMessage",
                table: "message");

            migrationBuilder.DropColumn(
                name: "GroupChatUserSeen",
                table: "group_chat_user");
        }
    }
}
