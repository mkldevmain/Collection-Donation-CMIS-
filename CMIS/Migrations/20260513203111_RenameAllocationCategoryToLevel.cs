using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMIS.Migrations
{
    /// <inheritdoc />
    public partial class RenameAllocationCategoryToLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "category",
                table: "budget_allocation",
                newName: "level");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "level",
                table: "budget_allocation",
                newName: "category");
        }
    }
}
