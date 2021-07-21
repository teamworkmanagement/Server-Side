using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class commentreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comment_report",
                columns: table => new
                {
                    comment_report_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comment_report_commentid = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comment_report_userid = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment_report", x => x.comment_report_id);
                    table.ForeignKey(
                        name: "FK_comment_report_comment_comment_report_commentid",
                        column: x => x.comment_report_commentid,
                        principalTable: "comment",
                        principalColumn: "comment_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comment_report_user_comment_report_userid",
                        column: x => x.comment_report_userid,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });            

            migrationBuilder.CreateIndex(
                name: "IX_comment_report_comment_report_commentid",
                table: "comment_report",
                column: "comment_report_commentid");

            migrationBuilder.CreateIndex(
                name: "IX_comment_report_comment_report_userid",
                table: "comment_report",
                column: "comment_report_userid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointment");

            migrationBuilder.DropTable(
                name: "comment_report");

            migrationBuilder.DropTable(
                name: "feedback");

            migrationBuilder.DropTable(
                name: "file");

            migrationBuilder.DropTable(
                name: "group_chat_user");

            migrationBuilder.DropTable(
                name: "handle_task");

            migrationBuilder.DropTable(
                name: "meeting");

            migrationBuilder.DropTable(
                name: "meeting_user");

            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "participation");

            migrationBuilder.DropTable(
                name: "post_react");

            migrationBuilder.DropTable(
                name: "post_report");

            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "task_version");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_connection");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "group_chat");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "post");

            migrationBuilder.DropTable(
                name: "task");

            migrationBuilder.DropTable(
                name: "kanban_list");

            migrationBuilder.DropTable(
                name: "kanban_board");

            migrationBuilder.DropTable(
                name: "team");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
