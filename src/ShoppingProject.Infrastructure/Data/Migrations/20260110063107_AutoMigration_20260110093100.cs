using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingProject.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AutoMigration_20260110093100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contributors",
                table: "Contributors");

            migrationBuilder.RenameTable(
                name: "Contributors",
                newName: "Contributor");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contributor",
                table: "Contributor",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Contributor",
                table: "Contributor");

            migrationBuilder.RenameTable(
                name: "Contributor",
                newName: "Contributors");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contributors",
                table: "Contributors",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AccessToken = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                });
        }
    }
}
