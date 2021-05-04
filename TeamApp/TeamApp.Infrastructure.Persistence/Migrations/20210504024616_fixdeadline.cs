using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class fixdeadline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "task_duration",
                table: "task");

            migrationBuilder.AddColumn<DateTime>(
                name: "task_deadline",
                table: "task",
                type: "timestamp",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "task_deadline",
                table: "task");

            migrationBuilder.AddColumn<int>(
                name: "task_duration",
                table: "task",
                type: "int",
                nullable: true);
        }
    }
}
