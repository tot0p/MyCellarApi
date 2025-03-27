using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyCellarApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCellarTableBottlesTableAndStockInformationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MyCellar");

            migrationBuilder.CreateTable(
                name: "Cellars",
                schema: "MyCellar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cellars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WineBottles",
                schema: "MyCellar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BottleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    VarietalType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WineType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Producer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetContent = table.Column<float>(type: "real", nullable: false),
                    AlcoholContent = table.Column<float>(type: "real", nullable: false),
                    UnitPrice = table.Column<float>(type: "real", nullable: false),
                    Labels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SulfiteWarning = table.Column<bool>(type: "bit", nullable: false),
                    Vegan = table.Column<bool>(type: "bit", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WineBottles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockInformations",
                schema: "MyCellar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WineId = table.Column<int>(type: "int", nullable: false),
                    CellarId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInformations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockInformations_Cellars_CellarId",
                        column: x => x.CellarId,
                        principalSchema: "MyCellar",
                        principalTable: "Cellars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockInformations_WineBottles_WineId",
                        column: x => x.WineId,
                        principalSchema: "MyCellar",
                        principalTable: "WineBottles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockInformations",
                schema: "MyCellar");

            migrationBuilder.DropTable(
                name: "Cellars",
                schema: "MyCellar");

            migrationBuilder.DropTable(
                name: "WineBottles",
                schema: "MyCellar");
        }
    }
}
