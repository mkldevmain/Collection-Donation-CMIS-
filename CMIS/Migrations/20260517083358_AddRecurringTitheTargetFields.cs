using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMIS.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringTitheTargetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TargetAmount",
                table: "recurring_tithes",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "recurring_tithes",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetAmount",
                table: "recurring_tithes");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "recurring_tithes");
        }
    }
}
