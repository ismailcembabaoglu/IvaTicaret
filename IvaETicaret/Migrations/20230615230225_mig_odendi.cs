using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvaETicaret.Migrations
{
    public partial class mig_odendi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Odendimi",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Odendimi",
                table: "OrderHeaders");
        }
    }
}
