using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasks4U.Migrations
{
    public partial class AddDirections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDescriptionLeftToRight",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNameLeftToRight",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRelatedToLeftToRight",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDescriptionLeftToRight",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsNameLeftToRight",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsRelatedToLeftToRight",
                table: "Tasks");
        }
    }
}
