using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebHotel_vesion1._0.Migrations
{
    /// <inheritdoc />
    public partial class migracion_05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstaDisponible",
                table: "tb_Habitaciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstaDisponible",
                table: "tb_Habitaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
