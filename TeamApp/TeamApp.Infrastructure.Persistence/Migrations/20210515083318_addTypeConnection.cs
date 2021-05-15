using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class addTypeConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "user_connection_type",
                table: "user_connection",
                type: "varchar(50)",
                nullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_connection_type",
                table: "user_connection");
        }
    }
}
