using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class addtaskimage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "task_progress",
                table: "task");

            migrationBuilder.AddColumn<string>(
                name: "task_image_url",
                table: "task",
                type: "varchar(200)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "task_image_url",
                table: "task");

            migrationBuilder.AddColumn<double>(
                name: "task_progress",
                table: "task",
                type: "double",
                nullable: true);
        }
    }
}
