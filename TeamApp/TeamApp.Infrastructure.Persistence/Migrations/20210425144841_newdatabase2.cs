using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class newdatabase2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaskOrderInList",
                table: "task",
                newName: "task_order_inlist");

            migrationBuilder.AlterColumn<int>(
                name: "task_order_inlist",
                table: "task",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "task_order_inlist",
                table: "task",
                newName: "TaskOrderInList");

            migrationBuilder.AlterColumn<string>(
                name: "TaskOrderInList",
                table: "task",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
