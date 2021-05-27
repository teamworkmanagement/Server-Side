using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class lexorank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropColumn(
                name: "task_order_inlist",
                table: "task");*/

            /*migrationBuilder.DropColumn(
                name: "kanban_list_order_in_board",
                table: "kanban_list");*/

            migrationBuilder.AddColumn<string>(
                name: "task_rank_inlist",
                table: "task",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kanban_list_rank_in_board",
                table: "kanban_list",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "task_rank_inlist",
                table: "task");

            migrationBuilder.DropColumn(
                name: "kanban_list_rank_in_board",
                table: "kanban_list");

            migrationBuilder.AddColumn<float>(
                name: "task_order_inlist",
                table: "task",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "kanban_list_order_in_board",
                table: "kanban_list",
                type: "float",
                nullable: true);
        }
    }
}
