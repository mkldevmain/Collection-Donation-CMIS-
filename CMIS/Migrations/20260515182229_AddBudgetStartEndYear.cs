using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMIS.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetStartEndYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "start_year",
                table: "budget",
                type: "int",
                nullable: false,
                defaultValue: 2024);

            migrationBuilder.AddColumn<int>(
                name: "end_year",
                table: "budget",
                type: "int",
                nullable: false,
                defaultValue: 2025);

            migrationBuilder.DropColumn(
                name: "fiscal_year",
                table: "budget");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "start_year",
                table: "budget");

            migrationBuilder.DropColumn(
                name: "end_year",
                table: "budget");

            migrationBuilder.AddColumn<string>(
                name: "fiscal_year",
                table: "budget",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
