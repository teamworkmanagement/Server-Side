using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class addBoardProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "kanban_created_at",
                table: "kanban_board",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kanban_board_name",
                table: "kanban_board",
                type: "varchar(300)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kanban_created_at",
                table: "kanban_board");

            migrationBuilder.DropColumn(
                name: "kanban_board_name",
                table: "kanban_board");
        }
    }
}
