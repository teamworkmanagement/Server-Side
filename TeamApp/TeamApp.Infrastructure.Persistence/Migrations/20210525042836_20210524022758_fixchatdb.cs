using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class _20210524022758_fixchatdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_file_user_file_user_id",
                table: "file");

            migrationBuilder.RenameColumn(
                name: "kanban_board_belonged_id",
                table: "kanban_board",
                newName: "kanban_board_userid");

            migrationBuilder.RenameColumn(
                name: "file_user_id",
                table: "file",
                newName: "file_userupload_id");

            migrationBuilder.RenameColumn(
                name: "file_belonged_id",
                table: "file",
                newName: "file_userowner_id");

            migrationBuilder.RenameIndex(
                name: "IX_file_file_user_id",
                table: "file",
                newName: "IX_file_file_userupload_id");

            migrationBuilder.AddColumn<bool>(
                name: "KanbanBoardIsDeleted",
                table: "kanban_board",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kanban_board_teamid",
                table: "kanban_board",
                type: "varchar(50)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "group_chat_type",
                table: "group_chat",
                type: "enum('double','multi')",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "file_postowner_id",
                table: "file",
                type: "varchar(50)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "file_taskowner_id",
                table: "file",
                type: "varchar(50)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "file_teamowner_id",
                table: "file",
                type: "varchar(50)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_kanban_board_kanban_board_teamid",
                table: "kanban_board",
                column: "kanban_board_teamid");

            migrationBuilder.CreateIndex(
                name: "IX_kanban_board_kanban_board_userid",
                table: "kanban_board",
                column: "kanban_board_userid");

            migrationBuilder.CreateIndex(
                name: "IX_file_file_postowner_id",
                table: "file",
                column: "file_postowner_id");

            migrationBuilder.CreateIndex(
                name: "IX_file_file_taskowner_id",
                table: "file",
                column: "file_taskowner_id");

            migrationBuilder.CreateIndex(
                name: "IX_file_file_teamowner_id",
                table: "file",
                column: "file_teamowner_id");

            migrationBuilder.CreateIndex(
                name: "IX_file_file_userowner_id",
                table: "file",
                column: "file_userowner_id");

            migrationBuilder.AddForeignKey(
                name: "FK_file_post_file_postowner_id",
                table: "file",
                column: "file_postowner_id",
                principalTable: "post",
                principalColumn: "post_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_file_task_file_taskowner_id",
                table: "file",
                column: "file_taskowner_id",
                principalTable: "task",
                principalColumn: "task_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_file_team_file_teamowner_id",
                table: "file",
                column: "file_teamowner_id",
                principalTable: "team",
                principalColumn: "team_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_file_user_file_userowner_id",
                table: "file",
                column: "file_userowner_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_file_user_file_userupload_id",
                table: "file",
                column: "file_userupload_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_kanban_board_team_kanban_board_teamid",
                table: "kanban_board",
                column: "kanban_board_teamid",
                principalTable: "team",
                principalColumn: "team_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_kanban_board_user_kanban_board_userid",
                table: "kanban_board",
                column: "kanban_board_userid",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_file_post_file_postowner_id",
                table: "file");

            migrationBuilder.DropForeignKey(
                name: "FK_file_task_file_taskowner_id",
                table: "file");

            migrationBuilder.DropForeignKey(
                name: "FK_file_team_file_teamowner_id",
                table: "file");

            migrationBuilder.DropForeignKey(
                name: "FK_file_user_file_userowner_id",
                table: "file");

            migrationBuilder.DropForeignKey(
                name: "FK_file_user_file_userupload_id",
                table: "file");

            migrationBuilder.DropForeignKey(
                name: "FK_kanban_board_team_kanban_board_teamid",
                table: "kanban_board");

            migrationBuilder.DropForeignKey(
                name: "FK_kanban_board_user_kanban_board_userid",
                table: "kanban_board");

            migrationBuilder.DropIndex(
                name: "IX_kanban_board_kanban_board_teamid",
                table: "kanban_board");

            migrationBuilder.DropIndex(
                name: "IX_kanban_board_kanban_board_userid",
                table: "kanban_board");

            migrationBuilder.DropIndex(
                name: "IX_file_file_postowner_id",
                table: "file");

            migrationBuilder.DropIndex(
                name: "IX_file_file_taskowner_id",
                table: "file");

            migrationBuilder.DropIndex(
                name: "IX_file_file_teamowner_id",
                table: "file");

            migrationBuilder.DropIndex(
                name: "IX_file_file_userowner_id",
                table: "file");

            migrationBuilder.DropColumn(
                name: "KanbanBoardIsDeleted",
                table: "kanban_board");

            migrationBuilder.DropColumn(
                name: "kanban_board_teamid",
                table: "kanban_board");

            migrationBuilder.DropColumn(
                name: "group_chat_type",
                table: "group_chat");

            migrationBuilder.DropColumn(
                name: "file_postowner_id",
                table: "file");

            migrationBuilder.DropColumn(
                name: "file_taskowner_id",
                table: "file");

            migrationBuilder.DropColumn(
                name: "file_teamowner_id",
                table: "file");

            migrationBuilder.RenameColumn(
                name: "kanban_board_userid",
                table: "kanban_board",
                newName: "kanban_board_belonged_id");

            migrationBuilder.RenameColumn(
                name: "file_userupload_id",
                table: "file",
                newName: "file_user_id");

            migrationBuilder.RenameColumn(
                name: "file_userowner_id",
                table: "file",
                newName: "file_belonged_id");

            migrationBuilder.RenameIndex(
                name: "IX_file_file_userupload_id",
                table: "file",
                newName: "IX_file_file_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_file_user_file_user_id",
                table: "file",
                column: "file_user_id",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
