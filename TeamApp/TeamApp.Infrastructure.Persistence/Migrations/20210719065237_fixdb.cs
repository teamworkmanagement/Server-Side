using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class fixdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hour",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "Minute",
                table: "appointment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Hour",
                table: "appointment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Minute",
                table: "appointment",
                type: "int",
                nullable: true);
        }
    }
}
