using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallerbiblioteca.Migrations
{
    /// <inheritdoc />
    public partial class prestamosnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Ejemplar_Id_ejemplar",
                table: "Prestamos");

            migrationBuilder.DropForeignKey(
                name:  "FK_Prestamos_Usuarios_Id_usuario",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_Id_ejemplar",
                table: "Prestamos");

            migrationBuilder.DropIndex(
                name: "IX_Prestamos_Id_usuario",
                table: "Prestamos");

               

            migrationBuilder.RenameColumn(
                name: "Id_ejemplar",
                table: "Prestamos",
                newName: "Id_peticion");

                 migrationBuilder.CreateIndex(
                name: "IX_Prestamos_Id_Peticion",
                table: "Prestamos",
                column: "Id_peticion");


            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Peticiones_Id_peticion",
                table: "Prestamos",
                column: "Id_peticion",
                principalTable: "Peticiones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
