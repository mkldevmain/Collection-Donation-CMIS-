using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMIS.Migrations
{
    /// <inheritdoc />
    public partial class DropFundTableIfExists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys that reference `fund`, then drop the fund_id columns, then drop the table.
            // Each step is guarded so the migration is idempotent across environments where prior
            // partial migrations may have already done some of the work.

            DropForeignKeyIfExists(migrationBuilder, "church",   "fk_church_fund");
            DropForeignKeyIfExists(migrationBuilder, "district", "fk_district_fund");
            DropForeignKeyIfExists(migrationBuilder, "budget",   "fk_budget_fund");

            DropColumnIfExists(migrationBuilder, "church",   "fund_id");
            DropColumnIfExists(migrationBuilder, "district", "fund_id");
            DropColumnIfExists(migrationBuilder, "budget",   "fund_id");

            migrationBuilder.Sql("DROP TABLE IF EXISTS `fund`;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

        private static void DropForeignKeyIfExists(MigrationBuilder mb, string table, string fk)
        {
            mb.Sql($@"
                SET @fk := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                            WHERE CONSTRAINT_SCHEMA = DATABASE()
                              AND TABLE_NAME = '{table}'
                              AND CONSTRAINT_NAME = '{fk}'
                              AND CONSTRAINT_TYPE = 'FOREIGN KEY');
                SET @sql := IF(@fk > 0, 'ALTER TABLE `{table}` DROP FOREIGN KEY `{fk}`', 'SELECT 1');
                PREPARE _s FROM @sql; EXECUTE _s; DEALLOCATE PREPARE _s;
            ");
        }

        private static void DropColumnIfExists(MigrationBuilder mb, string table, string column)
        {
            mb.Sql($@"
                SET @col := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                             WHERE TABLE_SCHEMA = DATABASE()
                               AND TABLE_NAME = '{table}'
                               AND COLUMN_NAME = '{column}');
                SET @sql := IF(@col > 0, 'ALTER TABLE `{table}` DROP COLUMN `{column}`', 'SELECT 1');
                PREPARE _s FROM @sql; EXECUTE _s; DEALLOCATE PREPARE _s;
            ");
        }
    }
}
