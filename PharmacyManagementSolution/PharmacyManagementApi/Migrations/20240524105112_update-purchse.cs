using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagementApi.Migrations
{
    public partial class updatepurchse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Vendors_VendorId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_VendorId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Purchases");

            migrationBuilder.AlterColumn<double>(
                name: "TotalAmount",
                table: "Purchases",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "PurchaseDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalSum",
                table: "PurchaseDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "PurchaseDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDetails_VendorId",
                table: "PurchaseDetails",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetails_Vendors_VendorId",
                table: "PurchaseDetails",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetails_Vendors_VendorId",
                table: "PurchaseDetails");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseDetails_VendorId",
                table: "PurchaseDetails");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "PurchaseDetails");

            migrationBuilder.DropColumn(
                name: "TotalSum",
                table: "PurchaseDetails");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "PurchaseDetails");

            migrationBuilder.AlterColumn<double>(
                name: "TotalAmount",
                table: "Purchases",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_VendorId",
                table: "Purchases",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Vendors_VendorId",
                table: "Purchases",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
