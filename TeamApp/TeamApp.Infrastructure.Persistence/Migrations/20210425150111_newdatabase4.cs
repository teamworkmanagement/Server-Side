using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class newdatabase4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_file_team_TeamId",
                table: "file");

            migrationBuilder.DropIndex(
                name: "IX_file_TeamId",
                table: "file");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "file");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "file",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_file_TeamId",
                table: "file",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_file_team_TeamId",
                table: "file",
                column: "TeamId",
                principalTable: "team",
                principalColumn: "team_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
