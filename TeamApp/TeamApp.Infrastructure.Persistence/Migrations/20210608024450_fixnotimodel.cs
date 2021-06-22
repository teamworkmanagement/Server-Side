using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class fixnotimodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    group_chat_type = table.Column<string>(type: "enum('double','multi')", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    group_chat_updated_at = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_chat", x => x.group_chat_id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 85, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 85, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.Id);
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
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    user_email = table.Column<string>(type: "varchar(50)", maxLength: 256, nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    user_password = table.Column<string>(type: "varchar(500)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    use_phone_number = table.Column<string>(type: "varchar(20)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    user_fullname = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_date_of_birth = table.Column<DateTime>(type: "timestamp", nullable: true),
                    user_image_url = table.Column<string>(type: "varchar(500)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    LastTimeOnline = table.Column<DateTime>(nullable: true),
                    user_firstime_social = table.Column<bool>(nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 50, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(maxLength: 50, nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_claims_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    group_chat_user_is_deleted = table.Column<bool>(nullable: true),
                    group_chat_user_seen = table.Column<bool>(nullable: true)
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
                    message_is_deleted = table.Column<bool>(nullable: true),
                    is_message = table.Column<bool>(nullable: true),
                    message_type = table.Column<string>(type: "enum('text','file','image')", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                    notification_is_deleted = table.Column<bool>(nullable: true),
                    notification_group = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    notification_actionuser_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.notification_id);
                    table.ForeignKey(
                        name: "FK_notification_user_notification_actionuser_id",
                        column: x => x.notification_actionuser_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "notification_ibfk_1",
                        column: x => x.notification_user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    Expires = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    CreatedByIp = table.Column<string>(nullable: true),
                    Revoked = table.Column<DateTime>(nullable: true),
                    RevokedByIp = table.Column<string>(nullable: true),
                    ReplacedByToken = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_token_user_UserId",
                        column: x => x.UserId,
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
                    team_is_deleted = table.Column<bool>(nullable: true),
                    team_image_url = table.Column<string>(type: "varchar(500)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                name: "user_claims",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 50, nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(maxLength: 50, nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_claims_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_connection",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    user_connection_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(nullable: true),
                    user_connection_type = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_connection", x => new { x.UserId, x.user_connection_id });
                    table.ForeignKey(
                        name: "FK_user_connection_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 50, nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 50, nullable: true),
                    ProviderKey = table.Column<string>(maxLength: 50, nullable: true),
                    ProviderDisplayName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_logins", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_user_logins_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 50, nullable: false),
                    RoleId = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_roles_role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    UserId = table.Column<string>(maxLength: 50, nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_user_tokens_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "kanban_board",
                columns: table => new
                {
                    kanban_board_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kanban_board_is_of_team = table.Column<bool>(nullable: true),
                    kanban_board_userid = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kanban_board_teamid = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kanban_board_name = table.Column<string>(type: "varchar(300)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kanban_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    KanbanBoardIsDeleted = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kanban_board", x => x.kanban_board_id);
                    table.ForeignKey(
                        name: "FK_kanban_board_team_kanban_board_teamid",
                        column: x => x.kanban_board_teamid,
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_kanban_board_user_kanban_board_userid",
                        column: x => x.kanban_board_userid,
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
                    kanban_list_rank_in_board = table.Column<string>(type: "varchar(50)", nullable: true),
                    kanban_list_is_deleted = table.Column<bool>(nullable: true, defaultValue: false)
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

            migrationBuilder.CreateTable(
                name: "post_react",
                columns: table => new
                {
                    post_react_id = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    post_react_postid = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    post_react_userid = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_react", x => x.post_react_id);
                    table.ForeignKey(
                        name: "FK_post_react_post_post_react_postid",
                        column: x => x.post_react_postid,
                        principalTable: "post",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_post_react_user_post_react_userid",
                        column: x => x.post_react_userid,
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
                    task_start_date = table.Column<DateTime>(type: "timestamp", nullable: true),
                    task_status = table.Column<string>(type: "enum('todo','doing','done')", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_completed_percent = table.Column<int>(nullable: true),
                    task_team_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_is_deleted = table.Column<bool>(nullable: true),
                    task_kanbanlist_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_rank_inlist = table.Column<string>(type: "varchar(50)", nullable: true),
                    task_theme_color = table.Column<string>(type: "varchar(10)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_image_url = table.Column<string>(type: "varchar(200)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    task_deadline = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task", x => x.task_id);
                    table.ForeignKey(
                        name: "FK_task_kanban_list_task_kanbanlist_id",
                        column: x => x.task_kanbanlist_id,
                        principalTable: "kanban_list",
                        principalColumn: "kanban_list_id",
                        onDelete: ReferentialAction.Restrict);
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
                    comment_task_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comment_content = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    comment_created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    comment_is_deleted = table.Column<bool>(nullable: true),
                    comment_type = table.Column<string>(type: "enum('text','file')", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                        name: "FK_comment_task_comment_task_id",
                        column: x => x.comment_task_id,
                        principalTable: "task",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "comment_ibfk_2",
                        column: x => x.comment_user_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    file_url = table.Column<string>(type: "varchar(500)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_type = table.Column<string>(type: "enum('word','excel','powerpoint','video','audio','pdf','zip','text','png','css','csv','exe','html','javascript','json','svg','xml','others')", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_userupload_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_userowner_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_teamowner_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_taskowner_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_postowner_id = table.Column<string>(type: "varchar(50)", nullable: true)
                        .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_size = table.Column<double>(nullable: false),
                    file_upload_time = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file", x => x.file_id);
                    table.ForeignKey(
                        name: "FK_file_post_file_postowner_id",
                        column: x => x.file_postowner_id,
                        principalTable: "post",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_file_task_file_taskowner_id",
                        column: x => x.file_taskowner_id,
                        principalTable: "task",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_file_team_file_teamowner_id",
                        column: x => x.file_teamowner_id,
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_file_user_file_userowner_id",
                        column: x => x.file_userowner_id,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_file_user_file_userupload_id",
                        column: x => x.file_userupload_id,
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
                name: "IX_comment_comment_task_id",
                table: "comment",
                column: "comment_task_id");

            migrationBuilder.CreateIndex(
                name: "comment_user_id",
                table: "comment",
                column: "comment_user_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_file_file_userupload_id",
                table: "file",
                column: "file_userupload_id");

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
                name: "IX_kanban_board_kanban_board_teamid",
                table: "kanban_board",
                column: "kanban_board_teamid");

            migrationBuilder.CreateIndex(
                name: "IX_kanban_board_kanban_board_userid",
                table: "kanban_board",
                column: "kanban_board_userid");

            migrationBuilder.CreateIndex(
                name: "IX_kanban_list_kanban_list_belonged_id",
                table: "kanban_list",
                column: "kanban_list_belonged_id");

            migrationBuilder.CreateIndex(
                name: "message_group_chat_id",
                table: "message",
                column: "message_group_chat_id");

            migrationBuilder.CreateIndex(
                name: "message_user_id",
                table: "message",
                column: "message_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_notification_actionuser_id",
                table: "notification",
                column: "notification_actionuser_id");

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
                name: "IX_post_react_post_react_postid",
                table: "post_react",
                column: "post_react_postid");

            migrationBuilder.CreateIndex(
                name: "IX_post_react_post_react_userid",
                table: "post_react",
                column: "post_react_userid");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_UserId",
                table: "refresh_token",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_claims_RoleId",
                table: "role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_task_task_kanbanlist_id",
                table: "task",
                column: "task_kanbanlist_id");

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

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "user",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "user",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_claims_UserId",
                table: "user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                column: "RoleId");
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
                name: "post_react");

            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "tag");

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
                name: "group_chat");

            migrationBuilder.DropTable(
                name: "post");

            migrationBuilder.DropTable(
                name: "task");

            migrationBuilder.DropTable(
                name: "role");

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
