using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class addprops2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "task_version_task_is_deleted",
                table: "task_version");

            migrationBuilder.AddColumn<string>(
                name: "TaskVersionActionUserId",
                table: "task_version",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_task_version_TaskVersionActionUserId",
                table: "task_version",
                column: "TaskVersionActionUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_task_version_user_TaskVersionActionUserId",
                table: "task_version",
                column: "TaskVersionActionUserId",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_task_version_user_TaskVersionActionUserId",
                table: "task_version");

            migrationBuilder.DropIndex(
                name: "IX_task_version_TaskVersionActionUserId",
                table: "task_version");

            migrationBuilder.DropColumn(
                name: "TaskVersionActionUserId",
                table: "task_version");

            migrationBuilder.AddColumn<bool>(
                name: "task_version_task_is_deleted",
                table: "task_version",
                type: "tinyint(1)",
                nullable: true);
        }
    }
}
