using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class addpostreact2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_post_react_user_UserReactId",
                table: "post_react");

            migrationBuilder.DropIndex(
                name: "IX_post_react_UserReactId",
                table: "post_react");

            migrationBuilder.DropColumn(
                name: "UserReactId",
                table: "post_react");

            migrationBuilder.CreateIndex(
                name: "IX_post_react_post_react_userid",
                table: "post_react",
                column: "post_react_userid");

            migrationBuilder.AddForeignKey(
                name: "FK_post_react_user_post_react_userid",
                table: "post_react",
                column: "post_react_userid",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_post_react_user_post_react_userid",
                table: "post_react");

            migrationBuilder.DropIndex(
                name: "IX_post_react_post_react_userid",
                table: "post_react");

            migrationBuilder.AddColumn<string>(
                name: "UserReactId",
                table: "post_react",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_post_react_UserReactId",
                table: "post_react",
                column: "UserReactId");

            migrationBuilder.AddForeignKey(
                name: "FK_post_react_user_UserReactId",
                table: "post_react",
                column: "UserReactId",
                principalTable: "user",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
