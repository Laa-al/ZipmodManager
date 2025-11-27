using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zmm.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LinkSize",
                table: "ZipmodLink",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkSize",
                table: "ZipmodLink");
        }
    }
}
