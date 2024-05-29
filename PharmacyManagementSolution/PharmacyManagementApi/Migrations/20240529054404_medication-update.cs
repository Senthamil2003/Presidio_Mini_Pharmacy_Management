using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagementApi.Migrations
{
    public partial class medicationupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationItems_Medications_MedicationId",
                table: "MedicationItems");

            migrationBuilder.AlterColumn<int>(
                name: "MedicationId",
                table: "MedicationItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationItems_Medications_MedicationId",
                table: "MedicationItems",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationItems_Medications_MedicationId",
                table: "MedicationItems");

            migrationBuilder.AlterColumn<int>(
                name: "MedicationId",
                table: "MedicationItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationItems_Medications_MedicationId",
                table: "MedicationItems",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId");
        }
    }
}
