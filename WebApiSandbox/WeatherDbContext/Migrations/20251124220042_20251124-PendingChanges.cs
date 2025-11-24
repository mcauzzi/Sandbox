using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCoreContext.Migrations
{
    /// <inheritdoc />
    public partial class _20251124PendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "WeatherForecasts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "WeatherForecasts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "WeatherForecasts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "WeatherForecasts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecasts_Latitude",
                table: "WeatherForecasts",
                column: "Latitude");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecasts_Latitude_Longitude",
                table: "WeatherForecasts",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecasts_Longitude",
                table: "WeatherForecasts",
                column: "Longitude");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WeatherForecasts_Latitude",
                table: "WeatherForecasts");

            migrationBuilder.DropIndex(
                name: "IX_WeatherForecasts_Latitude_Longitude",
                table: "WeatherForecasts");

            migrationBuilder.DropIndex(
                name: "IX_WeatherForecasts_Longitude",
                table: "WeatherForecasts");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "WeatherForecasts");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "WeatherForecasts");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "WeatherForecasts");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "WeatherForecasts",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
