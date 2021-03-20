using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class IdentityInit11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Rate = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "studentt",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Age = table.Column<int>(nullable: false),
                    Dob = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2021, 3, 20, 15, 8, 24, 658, DateTimeKind.Local).AddTicks(7968))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_studentt", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "studentt");
        }
    }
}
