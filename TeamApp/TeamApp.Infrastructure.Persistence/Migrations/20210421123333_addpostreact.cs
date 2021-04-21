using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class addpostreact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserReactId = table.Column<string>(nullable: true)
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
                        name: "FK_post_react_user_UserReactId",
                        column: x => x.UserReactId,
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_post_react_post_react_postid",
                table: "post_react",
                column: "post_react_postid");

            migrationBuilder.CreateIndex(
                name: "IX_post_react_UserReactId",
                table: "post_react",
                column: "UserReactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "post_react");
        }
    }
}
