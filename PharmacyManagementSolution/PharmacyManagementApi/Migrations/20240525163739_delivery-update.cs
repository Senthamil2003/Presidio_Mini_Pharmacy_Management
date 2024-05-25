using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagementApi.Migrations
{
    public partial class deliveryupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_PurchaseDetails_PurchaseDetailId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_PurchaseDetailId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "PurchaseDetailId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "StockId",
                table: "Carts",
                newName: "Quantity");

            migrationBuilder.AlterColumn<double>(
                name: "Cost",
                table: "OrderDetails",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "Cost",
                table: "Carts",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MedicineId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalCost",
                table: "Carts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "DeliveryDetails",
                columns: table => new
                {
                    DeliveryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderDetailId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryDetails", x => x.DeliveryId);
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_Medicines_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicines",
                        principalColumn: "MedicineId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryDetails_OrderDetails_OrderDetailId",
                        column: x => x.OrderDetailId,
                        principalTable: "OrderDetails",
                        principalColumn: "OrderDetailId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_MedicineId",
                table: "Carts",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_CustomerId",
                table: "DeliveryDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_MedicineId",
                table: "DeliveryDetails",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetails_OrderDetailId",
                table: "DeliveryDetails",
                column: "OrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Medicines_MedicineId",
                table: "Carts",
                column: "MedicineId",
                principalTable: "Medicines",
                principalColumn: "MedicineId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Medicines_MedicineId",
                table: "Carts");

            migrationBuilder.DropTable(
                name: "DeliveryDetails");

            migrationBuilder.DropIndex(
                name: "IX_Carts_MedicineId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "MedicineId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Carts",
                newName: "StockId");

            migrationBuilder.AlterColumn<int>(
                name: "Cost",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "OrderDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PurchaseDetailId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Cost",
                table: "Carts",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_PurchaseDetailId",
                table: "OrderDetails",
                column: "PurchaseDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_PurchaseDetails_PurchaseDetailId",
                table: "OrderDetails",
                column: "PurchaseDetailId",
                principalTable: "PurchaseDetails",
                principalColumn: "PurchaseDetailId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
