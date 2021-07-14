using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class refacordb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "post_is_pinned",
                table: "post");

            migrationBuilder.DropColumn(
                name: "group_chat_user_seen",
                table: "group_chat_user");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "post_is_pinned",
                table: "post",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "group_chat_user_seen",
                table: "group_chat_user",
                type: "tinyint(1)",
                nullable: true);
        }
    }
}
