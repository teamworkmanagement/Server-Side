using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class refactortaskmodel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaskDuration",
                table: "task",
                newName: "task_duration");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "task_duration",
                table: "task",
                newName: "TaskDuration");
        }
    }
}
