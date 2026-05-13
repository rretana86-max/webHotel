using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebHotel_vesion1._0.Migrations
{
    /// <inheritdoc />
    public partial class migracio_04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Confirmado",
                table: "tb_Reservas",
                newName: "Estado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "tb_Reservas",
                newName: "Confirmado");
        }
    }
}
