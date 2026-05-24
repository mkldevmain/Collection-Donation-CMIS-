using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMIS.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLevelFromBudgetAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "level",
                table: "budget_allocation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "level",
                table: "budget_allocation",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
