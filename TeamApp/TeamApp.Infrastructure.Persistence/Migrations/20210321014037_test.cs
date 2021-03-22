using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "file",
                columns: table => new
                {
                    file_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_name = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_url = table.Column<string>(type: "varchar(200)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "enum('word','excel','powerpoint','mp4','mp3','txt','zip','rar','others')", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file", x => x.file_id);
                });

            migrationBuilder.CreateTable(
                name: "group_chat",
                columns: table => new
                {
                    group_chat_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    group_chat_name = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    group_chat_updated_at = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_chat", x => x.group_chat_id);
                });

            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    tag_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tag_content = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tag_link = table.Column<string>(type: "varchar(200)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag", x => x.tag_id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_email = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_password = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_fullname = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_date_of_birth = table.Column<DateTime>(type: "timestamp", nullable: true),
                    use_phone_number = table.Column<string>(type: "varchar(20)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_image_url = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    user_is_theme_light = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "group_chat_user",
                columns: table => new
                {
                    group_chat_user_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    group_chat_user_user_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    group_chat_user_group_chat_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    group_chat_user_is_deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_chat_user", x => x.group_chat_user_id);
                    table.ForeignKey(
                        name: "group_chat_user_ibfk_2",
                        column: x => x.group_chat_user_group_chat_id,
                        principalTable: "group_chat",
                        principalColumn: "group_chat_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "group_chat_user_ibfk_1",
                        column: x => x.group_chat_user_user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "message",
                columns: table => new
                {
                    message_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message_user_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message_group_chat_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message_content = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    message_is_deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message", x => x.message_id);
                    table.ForeignKey(
                        name: "message_ibfk_2",
                        column: x => x.message_group_chat_id,
                        principalTable: "group_chat",
                        principalColumn: "group_chat_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "message_ibfk_1",
                        column: x => x.message_user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    notification_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notification_user_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notification_content = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notification_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    notification_link = table.Column<string>(type: "varchar(200)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notification_status = table.Column<bool>(nullable: true),
                    notification_is_deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.notification_id);
                    table.ForeignKey(
                        name: "notification_ibfk_1",
                        column: x => x.notification_user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "team",
                columns: table => new
                {
                    team_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    team_leader_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    team_name = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    team_description = table.Column<string>(type: "varchar(500)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    team_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    team_code = table.Column<string>(type: "varchar(20)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    team_is_deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team", x => x.team_id);
                    table.ForeignKey(
                        name: "team_ibfk_1",
                        column: x => x.team_leader_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "participation",
                columns: table => new
                {
                    participation_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    participation_user_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    participation_team_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    participation_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    participation_is_deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participation", x => x.participation_id);
                    table.ForeignKey(
                        name: "participation_ibfk_2",
                        column: x => x.participation_team_id,
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "participation_ibfk_1",
                        column: x => x.participation_user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "post",
                columns: table => new
                {
                    post_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    post_user_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    post_team_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    post_content = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    post_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    post_comment_count = table.Column<int>(nullable: true),
                    post_is_deleted = table.Column<bool>(nullable: true),
                    post_is_pinned = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post", x => x.post_id);
                    table.ForeignKey(
                        name: "post_ibfk_2",
                        column: x => x.post_team_id,
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "post_ibfk_1",
                        column: x => x.post_user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "task",
                columns: table => new
                {
                    task_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_name = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_description = table.Column<string>(type: "varchar(500)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_point = table.Column<int>(nullable: true),
                    task_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    task_deadline = table.Column<DateTime>(type: "timestamp", nullable: true),
                    task_status = table.Column<string>(type: "enum('todo','doing','done')", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_completed_percent = table.Column<int>(nullable: true),
                    task_team_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_is_deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task", x => x.task_id);
                    table.ForeignKey(
                        name: "task_ibfk_1",
                        column: x => x.task_team_id,
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comment",
                columns: table => new
                {
                    comment_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comment_post_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comment_user_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comment_content = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comment_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    comment_is_deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment", x => x.comment_id);
                    table.ForeignKey(
                        name: "comment_ibfk_1",
                        column: x => x.comment_post_id,
                        principalTable: "post",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "comment_ibfk_2",
                        column: x => x.comment_user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "handle_task",
                columns: table => new
                {
                    handle_task_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    handle_task_user_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    handle_task_task_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    handle_task_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    handle_task_is_deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_handle_task", x => x.handle_task_id);
                    table.ForeignKey(
                        name: "handle_task_ibfk_2",
                        column: x => x.handle_task_task_id,
                        principalTable: "task",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "handle_task_ibfk_1",
                        column: x => x.handle_task_user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "task_version",
                columns: table => new
                {
                    task_version_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_version_task_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_version_updated_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    task_version_task_name = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_version_task_description = table.Column<string>(type: "varchar(500)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_version_task_point = table.Column<int>(nullable: true),
                    task_version_task_deadline = table.Column<DateTime>(type: "timestamp", nullable: true),
                    task_version_task_status = table.Column<string>(type: "enum('todo','doing','done')", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_version_task_completed_percent = table.Column<int>(nullable: true),
                    task_version_task_is_deleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_version", x => x.task_version_id);
                    table.ForeignKey(
                        name: "task_version_ibfk_1",
                        column: x => x.task_version_task_id,
                        principalTable: "task",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "comment_post_id",
                table: "comment",
                column: "comment_post_id");

            migrationBuilder.CreateIndex(
                name: "comment_user_id",
                table: "comment",
                column: "comment_user_id");

            migrationBuilder.CreateIndex(
                name: "group_chat_user_group_chat_id",
                table: "group_chat_user",
                column: "group_chat_user_group_chat_id");

            migrationBuilder.CreateIndex(
                name: "group_chat_user_user_id",
                table: "group_chat_user",
                column: "group_chat_user_user_id");

            migrationBuilder.CreateIndex(
                name: "handle_task_task_id",
                table: "handle_task",
                column: "handle_task_task_id");

            migrationBuilder.CreateIndex(
                name: "handle_task_user_id",
                table: "handle_task",
                column: "handle_task_user_id");

            migrationBuilder.CreateIndex(
                name: "message_group_chat_id",
                table: "message",
                column: "message_group_chat_id");

            migrationBuilder.CreateIndex(
                name: "message_user_id",
                table: "message",
                column: "message_user_id");

            migrationBuilder.CreateIndex(
                name: "notification_user_id",
                table: "notification",
                column: "notification_user_id");

            migrationBuilder.CreateIndex(
                name: "participation_team_id",
                table: "participation",
                column: "participation_team_id");

            migrationBuilder.CreateIndex(
                name: "participation_user_id",
                table: "participation",
                column: "participation_user_id");

            migrationBuilder.CreateIndex(
                name: "post_team_id",
                table: "post",
                column: "post_team_id");

            migrationBuilder.CreateIndex(
                name: "post_user_id",
                table: "post",
                column: "post_user_id");

            migrationBuilder.CreateIndex(
                name: "task_team_id",
                table: "task",
                column: "task_team_id");

            migrationBuilder.CreateIndex(
                name: "task_version_task_id",
                table: "task_version",
                column: "task_version_task_id");

            migrationBuilder.CreateIndex(
                name: "team_leader_id",
                table: "team",
                column: "team_leader_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "file");

            migrationBuilder.DropTable(
                name: "group_chat_user");

            migrationBuilder.DropTable(
                name: "handle_task");

            migrationBuilder.DropTable(
                name: "message");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "participation");

            migrationBuilder.DropTable(
                name: "tag");

            migrationBuilder.DropTable(
                name: "task_version");

            migrationBuilder.DropTable(
                name: "post");

            migrationBuilder.DropTable(
                name: "group_chat");

            migrationBuilder.DropTable(
                name: "task");

            migrationBuilder.DropTable(
                name: "team");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
