using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class IdentityInit111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Dob",
                table: "studentt",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2021, 3, 20, 8, 10, 52, 549, DateTimeKind.Unspecified).AddTicks(2757), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValue: new DateTime(2021, 3, 20, 15, 8, 24, 658, DateTimeKind.Local).AddTicks(7968));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Dob",
                table: "studentt",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(2021, 3, 20, 15, 8, 24, 658, DateTimeKind.Local).AddTicks(7968),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2021, 3, 20, 8, 10, 52, 549, DateTimeKind.Unspecified).AddTicks(2757), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
