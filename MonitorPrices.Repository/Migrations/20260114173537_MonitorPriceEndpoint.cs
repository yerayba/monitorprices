using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitorPrices.Repository.Migrations
{
    /// <inheritdoc />
    public partial class MonitorPriceEndpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Precio",
                table: "Products");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaComprobación",
                table: "Products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PrecioActual",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrecioUltimo",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaUltimaComprobación",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PrecioActual",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PrecioUltimo",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
