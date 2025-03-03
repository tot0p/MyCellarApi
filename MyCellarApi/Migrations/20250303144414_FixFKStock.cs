using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCellarApi.Migrations
{
    /// <inheritdoc />
    public partial class FixFKStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockInformations_Cellars_CellarId",
                schema: "MyCellar",
                table: "StockInformations");

            migrationBuilder.DropForeignKey(
                name: "FK_StockInformations_WineBottles_WineId",
                schema: "MyCellar",
                table: "StockInformations");

            migrationBuilder.DropIndex(
                name: "IX_StockInformations_CellarId",
                schema: "MyCellar",
                table: "StockInformations");

            migrationBuilder.DropIndex(
                name: "IX_StockInformations_WineId",
                schema: "MyCellar",
                table: "StockInformations");

            migrationBuilder.RenameColumn(
                name: "WineId",
                schema: "MyCellar",
                table: "StockInformations",
                newName: "WineBottleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WineBottleId",
                schema: "MyCellar",
                table: "StockInformations",
                newName: "WineId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInformations_CellarId",
                schema: "MyCellar",
                table: "StockInformations",
                column: "CellarId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInformations_WineId",
                schema: "MyCellar",
                table: "StockInformations",
                column: "WineId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockInformations_Cellars_CellarId",
                schema: "MyCellar",
                table: "StockInformations",
                column: "CellarId",
                principalSchema: "MyCellar",
                principalTable: "Cellars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockInformations_WineBottles_WineId",
                schema: "MyCellar",
                table: "StockInformations",
                column: "WineId",
                principalSchema: "MyCellar",
                principalTable: "WineBottles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
