using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class changename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MeetingUser",
                table: "MeetingUser");

            migrationBuilder.RenameTable(
                name: "Meeting",
                newName: "meeting");

            migrationBuilder.RenameTable(
                name: "MeetingUser",
                newName: "meeting_user");

            migrationBuilder.AddPrimaryKey(
                name: "PK_meeting",
                table: "meeting",
                column: "MeetingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_meeting_user",
                table: "meeting_user",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_meeting",
                table: "meeting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_meeting_user",
                table: "meeting_user");

            migrationBuilder.RenameTable(
                name: "meeting",
                newName: "Meeting");

            migrationBuilder.RenameTable(
                name: "meeting_user",
                newName: "MeetingUser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting",
                column: "MeetingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeetingUser",
                table: "MeetingUser",
                column: "Id");
        }
    }
}
