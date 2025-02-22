using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthContextEfCore.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    User = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => new { x.User, x.Token });
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_User",
                table: "RefreshTokens",
                column: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");
        }
    }
}
