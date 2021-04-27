using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class newfile2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "file_user_id",
                table: "file",
                type: "varchar(50)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_file_file_user_id",
                table: "file",
                column: "file_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_file_user_file_user_id",
                table: "file",
                column: "file_user_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_file_user_file_user_id",
                table: "file");

            migrationBuilder.DropIndex(
                name: "IX_file_file_user_id",
                table: "file");

            migrationBuilder.DropColumn(
                name: "file_user_id",
                table: "file");
        }
    }
}
