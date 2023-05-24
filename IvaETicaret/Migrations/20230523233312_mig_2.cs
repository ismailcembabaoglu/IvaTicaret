using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvaETicaret.Migrations
{
    public partial class mig_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Key",
                table: "Districts",
                newName: "Keye");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "Counties",
                newName: "Keye");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "Cities",
                newName: "Keye");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Keye",
                table: "Districts",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "Keye",
                table: "Counties",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "Keye",
                table: "Cities",
                newName: "Key");
        }
    }
}
