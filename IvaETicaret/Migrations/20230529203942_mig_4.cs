using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IvaETicaret.Migrations
{
    public partial class mig_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adress_Stores_StoreId",
                table: "Adress");

            migrationBuilder.DropIndex(
                name: "IX_Adress_StoreId",
                table: "Adress");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Adress");

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "OrderHeaders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_StoreId",
                table: "OrderHeaders",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Stores_StoreId",
                table: "OrderHeaders",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Stores_StoreId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_StoreId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<Guid>(
                name: "StoreId",
                table: "Adress",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Adress_StoreId",
                table: "Adress",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Adress_Stores_StoreId",
                table: "Adress",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
