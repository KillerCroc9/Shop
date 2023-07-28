using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ar_Shop.Data.Migrations
{
    /// <inheritdoc />
    public partial class UPDATES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NeedToDelete",
                table: "Picture",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedToDelete",
                table: "Picture");
        }
    }
}
