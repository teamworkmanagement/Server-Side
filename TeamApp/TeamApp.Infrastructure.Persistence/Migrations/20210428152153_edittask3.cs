using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class edittask3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaskId",
                table: "file",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_file_TaskId",
                table: "file",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_file_task_TaskId",
                table: "file",
                column: "TaskId",
                principalTable: "task",
                principalColumn: "task_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_file_task_TaskId",
                table: "file");

            migrationBuilder.DropIndex(
                name: "IX_file_TaskId",
                table: "file");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "file");
        }
    }
}
