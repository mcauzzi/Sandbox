using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCoreContext.Migrations
{
    /// <inheritdoc />
    public partial class CreateImportMaterializedView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                    CREATE MATERIALIZED VIEW "RandomCityView" AS
                                        SELECT c."Id" AS "CityId", c."Latitude", c."Longitude",Count(w0."Id") as "ForecastCount"
                                        FROM "Cities" AS c
                                        left join "WeatherForecasts" AS w0 on c."Id"=w0."CityId"
                                        group by c."Id","Latitude","Longitude"
                                        order by "ForecastCount";
                                """);
        }

// In the Down method
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS RandomCityView;");
        }
    }
}
