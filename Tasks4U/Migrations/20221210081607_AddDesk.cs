using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasks4U.Migrations
{
    public partial class AddDesk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeskID",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Desks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desks", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_DeskID",
                table: "Tasks",
                column: "DeskID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Desks_DeskID",
                table: "Tasks",
                column: "DeskID",
                principalTable: "Desks",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Desks_DeskID",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Desks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_DeskID",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DeskID",
                table: "Tasks");
        }
    }
}
