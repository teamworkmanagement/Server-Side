using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class newdatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_file_team_file_team_id",
                table: "file");

            migrationBuilder.DropIndex(
                name: "IX_file_file_team_id",
                table: "file");

            migrationBuilder.DropColumn(
                name: "user_is_theme_light",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "file_team_id",
                table: "file",
                newName: "file_belonged_id");

            migrationBuilder.AddColumn<string>(
                name: "task_kanbanlist_id",
                table: "task",
                type: "varchar(50)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TaskOrderInList",
                table: "task",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "file",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "kanban_board",
                columns: table => new
                {
                    kanban_board_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kanban_board_is_of_team = table.Column<bool>(nullable: true),
                    kanban_board_belonged_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kanban_board", x => x.kanban_board_id);
                });

            migrationBuilder.CreateTable(
                name: "kanban_list",
                columns: table => new
                {
                    kanban_list_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kanban_list_title = table.Column<string>(type: "varchar(150)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kanban_list_belonged_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kanban_list_order_in_board = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kanban_list", x => x.kanban_list_id);
                    table.ForeignKey(
                        name: "FK_kanban_list_kanban_board_kanban_list_belonged_id",
                        column: x => x.kanban_list_belonged_id,
                        principalTable: "kanban_board",
                        principalColumn: "kanban_board_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_task_task_kanbanlist_id",
                table: "task",
                column: "task_kanbanlist_id");

            migrationBuilder.CreateIndex(
                name: "IX_file_TeamId",
                table: "file",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_kanban_list_kanban_list_belonged_id",
                table: "kanban_list",
                column: "kanban_list_belonged_id");

            migrationBuilder.AddForeignKey(
                name: "FK_file_team_TeamId",
                table: "file",
                column: "TeamId",
                principalTable: "team",
                principalColumn: "team_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_task_kanban_list_task_kanbanlist_id",
                table: "task",
                column: "task_kanbanlist_id",
                principalTable: "kanban_list",
                principalColumn: "kanban_list_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_file_team_TeamId",
                table: "file");

            migrationBuilder.DropForeignKey(
                name: "FK_task_kanban_list_task_kanbanlist_id",
                table: "task");

            migrationBuilder.DropTable(
                name: "kanban_list");

            migrationBuilder.DropTable(
                name: "kanban_board");

            migrationBuilder.DropIndex(
                name: "IX_task_task_kanbanlist_id",
                table: "task");

            migrationBuilder.DropIndex(
                name: "IX_file_TeamId",
                table: "file");

            migrationBuilder.DropColumn(
                name: "task_kanbanlist_id",
                table: "task");

            migrationBuilder.DropColumn(
                name: "TaskOrderInList",
                table: "task");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "file");

            migrationBuilder.RenameColumn(
                name: "file_belonged_id",
                table: "file",
                newName: "file_team_id");

            migrationBuilder.AddColumn<bool>(
                name: "user_is_theme_light",
                table: "user",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_file_file_team_id",
                table: "file",
                column: "file_team_id");

            migrationBuilder.AddForeignKey(
                name: "FK_file_team_file_team_id",
                table: "file",
                column: "file_team_id",
                principalTable: "team",
                principalColumn: "team_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
