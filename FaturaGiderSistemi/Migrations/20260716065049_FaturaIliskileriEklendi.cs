using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaturaGiderSistemi.Migrations
{
    /// <inheritdoc />
    public partial class FaturaIliskileriEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "KdvOrani",
                table: "Faturalar",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<bool>(
                name: "Durum",
                table: "Faturalar",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "FaturaNo",
                table: "Faturalar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Tutar",
                table: "Faturalar",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaturaNo",
                table: "Faturalar");

            migrationBuilder.DropColumn(
                name: "Tutar",
                table: "Faturalar");

            migrationBuilder.AlterColumn<decimal>(
                name: "KdvOrani",
                table: "Faturalar",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Durum",
                table: "Faturalar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
