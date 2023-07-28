using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ar_Shop.Data.Migrations
{
    /// <inheritdoc />
    public partial class Productpic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureUrls",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrls",
                table: "Product");
        }
    }
}
