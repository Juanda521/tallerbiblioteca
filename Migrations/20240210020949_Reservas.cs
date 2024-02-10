using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallerbiblioteca.Migrations
{
    /// <inheritdoc />
    public partial class Reservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id_reserva",
                table: "Peticiones",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reserva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEjemplar = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    FechaReserva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reserva", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reserva_Ejemplares_IdEjemplar",
                        column: x => x.IdEjemplar,
                        principalTable: "Ejemplares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reserva_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Peticiones_Id_reserva",
                table: "Peticiones",
                column: "Id_reserva");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_IdEjemplar",
                table: "Reserva",
                column: "IdEjemplar");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_IdUsuario",
                table: "Reserva",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Peticiones_Reserva_Id_reserva",
                table: "Peticiones",
                column: "Id_reserva",
                principalTable: "Reserva",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Peticiones_Reserva_Id_reserva",
                table: "Peticiones");

            migrationBuilder.DropTable(
                name: "Reserva");

            migrationBuilder.DropIndex(
                name: "IX_Peticiones_Id_reserva",
                table: "Peticiones");

            migrationBuilder.DropColumn(
                name: "Id_reserva",
                table: "Peticiones");
        }
    }
}
