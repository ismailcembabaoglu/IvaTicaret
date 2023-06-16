using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvaETicaret.Migrations
{
    public partial class mig_storeimage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Stores");
        }
    }
}
