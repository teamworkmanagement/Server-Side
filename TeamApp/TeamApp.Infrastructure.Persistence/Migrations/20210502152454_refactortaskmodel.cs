using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class refactortaskmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "task_deadline",
                table: "task",
                newName: "task_start_date");

            migrationBuilder.AddColumn<int>(
                name: "TaskDuration",
                table: "task",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskDuration",
                table: "task");

            migrationBuilder.RenameColumn(
                name: "task_start_date",
                table: "task",
                newName: "task_deadline");
        }
    }
}
