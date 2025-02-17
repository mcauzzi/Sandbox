using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCoreContext.Migrations
{
    /// <inheritdoc />
    public partial class AddedCitiesCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Cities",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Cities",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Cities");
        }
    }
}
