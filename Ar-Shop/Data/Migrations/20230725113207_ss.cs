using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ar_Shop.Data.Migrations
{
    /// <inheritdoc />
    public partial class ss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartItemId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CartItemId",
                table: "Carts",
                column: "CartItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_CartItems_CartItemId",
                table: "Carts",
                column: "CartItemId",
                principalTable: "CartItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_CartItems_CartItemId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_CartItemId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "Carts");
        }
    }
}
