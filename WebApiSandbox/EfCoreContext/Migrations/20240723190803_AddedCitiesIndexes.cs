using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCoreContext.Migrations
{
    /// <inheritdoc />
    public partial class AddedCitiesIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cities_Country",
                table: "Cities",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_State",
                table: "Cities",
                column: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cities_Country",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_State",
                table: "Cities");
        }
    }
}
