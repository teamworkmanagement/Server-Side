using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class edittask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "task_theme_color",
                table: "task",
                type: "varchar(10)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "comment_task_id",
                table: "comment",
                type: "varchar(50)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_comment_comment_task_id",
                table: "comment",
                column: "comment_task_id");

            migrationBuilder.AddForeignKey(
                name: "FK_comment_task_comment_task_id",
                table: "comment",
                column: "comment_task_id",
                principalTable: "task",
                principalColumn: "task_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comment_task_comment_task_id",
                table: "comment");

            migrationBuilder.DropIndex(
                name: "IX_comment_comment_task_id",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "task_theme_color",
                table: "task");

            migrationBuilder.DropColumn(
                name: "comment_task_id",
                table: "comment");
        }
    }
}
