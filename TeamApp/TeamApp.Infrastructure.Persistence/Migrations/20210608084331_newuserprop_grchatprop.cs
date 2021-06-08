using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class newuserprop_grchatprop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "user_address",
                table: "user",
                type: "varchar(350)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "user_description",
                table: "user",
                type: "text",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "user_facebook_link",
                table: "user",
                type: "varchar(350)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "user_github_link",
                table: "user",
                type: "varchar(350)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "group_chat_imageurl",
                table: "group_chat",
                type: "varchar(500)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_address",
                table: "user");

            migrationBuilder.DropColumn(
                name: "user_description",
                table: "user");

            migrationBuilder.DropColumn(
                name: "user_facebook_link",
                table: "user");

            migrationBuilder.DropColumn(
                name: "user_github_link",
                table: "user");

            migrationBuilder.DropColumn(
                name: "group_chat_imageurl",
                table: "group_chat");
        }
    }
}
