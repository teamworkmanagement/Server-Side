using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamApp.Infrastructure.Persistence.Migrations
{
    public partial class fixenumfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "file_type",
                table: "file",
                type: "enum('word','excel','powerpoint','video','audio','pdf','zip','text','png','css','csv','exe','html','javascript','json','svg','xml','others')",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "enum('word','excel','powerpoint','mp4','mp3','txt','zip','rar','others')",
                oldNullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "file_type",
                table: "file",
                type: "enum('word','excel','powerpoint','mp4','mp3','txt','zip','rar','others')",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "enum('word','excel','powerpoint','video','audio','pdf','zip','text','png','css','csv','exe','html','javascript','json','svg','xml','others')",
                oldNullable: true)
                .Annotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:Collation", "utf8mb4_0900_ai_ci")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
