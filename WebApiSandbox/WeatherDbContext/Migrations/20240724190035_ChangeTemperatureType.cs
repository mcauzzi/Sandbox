using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCoreContext.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTemperatureType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TemperatureC",
                table: "WeatherForecasts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TemperatureC",
                table: "WeatherForecasts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
