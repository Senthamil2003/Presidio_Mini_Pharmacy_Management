using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagementApi.Migrations
{
    public partial class vendorupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecentSellingPrice",
                table: "Medicines",
                newName: "RecentPurchasePrice");

            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mail",
                table: "Vendors");

            migrationBuilder.RenameColumn(
                name: "RecentPurchasePrice",
                table: "Medicines",
                newName: "RecentSellingPrice");
        }
    }
}
