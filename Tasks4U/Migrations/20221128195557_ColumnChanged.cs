using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasks4U.Migrations
{
    public partial class ColumnChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IntermmediateDate",
                table: "Tasks",
                newName: "IntermediateDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IntermediateDate",
                table: "Tasks",
                newName: "IntermmediateDate");
        }
    }
}
