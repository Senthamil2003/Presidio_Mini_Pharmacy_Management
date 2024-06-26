using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagementApi.Migrations
{
    public partial class purchaseupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellingPrice",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "DosageForm",
                table: "PurchaseDetails");

            migrationBuilder.DropColumn(
                name: "StorageRequirement",
                table: "PurchaseDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SellingPrice",
                table: "Stocks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "DosageForm",
                table: "PurchaseDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StorageRequirement",
                table: "PurchaseDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
