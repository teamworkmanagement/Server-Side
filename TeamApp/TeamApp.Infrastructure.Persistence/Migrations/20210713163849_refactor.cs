using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class refactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tag");

            migrationBuilder.DropColumn(
                name: "CreatedByIp",
                table: "refresh_token");

            migrationBuilder.DropColumn(
                name: "ReplacedByToken",
                table: "refresh_token");

            migrationBuilder.DropColumn(
                name: "Revoked",
                table: "refresh_token");

            migrationBuilder.DropColumn(
                name: "RevokedByIp",
                table: "refresh_token");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByIp",
                table: "refresh_token",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplacedByToken",
                table: "refresh_token",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Revoked",
                table: "refresh_token",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevokedByIp",
                table: "refresh_token",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    tag_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tag_content = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tag_link = table.Column<string>(type: "varchar(200)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag", x => x.tag_id);
                });
        }
    }
}
