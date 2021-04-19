using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class newfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "file_size",
                table: "file",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "file_team_id",
                table: "file",
                type: "varchar(50)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_file_file_team_id",
                table: "file",
                column: "file_team_id");

            migrationBuilder.AddForeignKey(
                name: "FK_file_team_file_team_id",
                table: "file",
                column: "file_team_id",
                principalTable: "team",
                principalColumn: "team_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_file_team_file_team_id",
                table: "file");

            migrationBuilder.DropIndex(
                name: "IX_file_file_team_id",
                table: "file");

            migrationBuilder.DropColumn(
                name: "file_size",
                table: "file");

            migrationBuilder.DropColumn(
                name: "file_team_id",
                table: "file");
        }
    }
}
