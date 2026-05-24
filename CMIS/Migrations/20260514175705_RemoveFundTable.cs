using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMIS.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFundTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The fund table and its FK constraints were dropped in a prior partial run.
            // Drop the fund_id columns that remain on each table.
            migrationBuilder.Sql(@"
                SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                            WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'district' AND COLUMN_NAME = 'fund_id');
                SET @sql = IF(@col > 0, 'ALTER TABLE `district` DROP COLUMN `fund_id`', 'SELECT 1');
                PREPARE _s FROM @sql; EXECUTE _s; DEALLOCATE PREPARE _s;
            ");
            migrationBuilder.Sql(@"
                SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                            WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'church' AND COLUMN_NAME = 'fund_id');
                SET @sql = IF(@col > 0, 'ALTER TABLE `church` DROP COLUMN `fund_id`', 'SELECT 1');
                PREPARE _s FROM @sql; EXECUTE _s; DEALLOCATE PREPARE _s;
            ");
            migrationBuilder.Sql(@"
                SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                            WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'budget' AND COLUMN_NAME = 'fund_id');
                SET @sql = IF(@col > 0, 'ALTER TABLE `budget` DROP COLUMN `fund_id`', 'SELECT 1');
                PREPARE _s FROM @sql; EXECUTE _s; DEALLOCATE PREPARE _s;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "fund_id",
                table: "district",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fund_id",
                table: "church",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fund_id",
                table: "budget",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "fund",
                columns: table => new
                {
                    fund_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    church_id = table.Column<int>(type: "int", nullable: true),
                    district_id = table.Column<int>(type: "int", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    description = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    level = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.fund_id);
                    table.ForeignKey(
                        name: "fk_fund_church",
                        column: x => x.church_id,
                        principalTable: "church",
                        principalColumn: "church_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_fund_district",
                        column: x => x.district_id,
                        principalTable: "district",
                        principalColumn: "district_id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_district_fund_id",
                table: "district",
                column: "fund_id");

            migrationBuilder.CreateIndex(
                name: "IX_church_fund_id",
                table: "church",
                column: "fund_id");

            migrationBuilder.CreateIndex(
                name: "IX_budget_fund_id",
                table: "budget",
                column: "fund_id");

            migrationBuilder.CreateIndex(
                name: "IX_fund_church_id",
                table: "fund",
                column: "church_id");

            migrationBuilder.CreateIndex(
                name: "IX_fund_district_id",
                table: "fund",
                column: "district_id");

            migrationBuilder.AddForeignKey(
                name: "fk_budget_fund",
                table: "budget",
                column: "fund_id",
                principalTable: "fund",
                principalColumn: "fund_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_church_fund",
                table: "church",
                column: "fund_id",
                principalTable: "fund",
                principalColumn: "fund_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_district_fund",
                table: "district",
                column: "fund_id",
                principalTable: "fund",
                principalColumn: "fund_id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
