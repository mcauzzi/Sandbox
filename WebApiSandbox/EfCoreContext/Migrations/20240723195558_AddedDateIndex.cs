using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCoreContext.Migrations
{
    /// <inheritdoc />
    public partial class AddedDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecasts_Date",
                table: "WeatherForecasts",
                column: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WeatherForecasts_Date",
                table: "WeatherForecasts");
        }
    }
}
