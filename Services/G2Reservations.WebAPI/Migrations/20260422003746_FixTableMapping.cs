using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2Reservations.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixTableMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_G2Reservation",
                table: "G2Reservation");

            migrationBuilder.RenameTable(
                name: "G2Reservation",
                newName: "Reservations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "G2Reservation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_G2Reservation",
                table: "G2Reservation",
                column: "Id");
        }
    }
}
