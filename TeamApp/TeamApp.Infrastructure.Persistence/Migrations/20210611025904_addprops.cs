using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class addprops : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TaskVersionDoneDate",
                table: "task_version",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TaskVersionStartDate",
                table: "task_version",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "task_done_date",
                table: "task",
                type: "timestamp",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskVersionDoneDate",
                table: "task_version");

            migrationBuilder.DropColumn(
                name: "TaskVersionStartDate",
                table: "task_version");

            migrationBuilder.DropColumn(
                name: "task_done_date",
                table: "task");
        }
    }
}
