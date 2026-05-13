using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebHotel_vesion1._0.Migrations
{
    /// <inheritdoc />
    public partial class migracio_03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Confirmado",
                table: "tb_Reservas",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Confirmado",
                table: "tb_Reservas",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
