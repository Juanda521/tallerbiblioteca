using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallerbiblioteca.Migrations
{
    /// <inheritdoc />
    public partial class Permisos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "ACTIVO")
                }, constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                }
            );
            migrationBuilder.CreateTable(
              name: "Configuracion",
              columns: table => new
              {
                  Id = table.Column<int>(type: "int", nullable: false)
                      .Annotation("SqlServer:Identity", "1, 1"),
                  Id_rol = table.Column<int>(type: "int", nullable: false),
                  Id_permiso = table.Column<int>(type: "int", nullable: false),
                  Estado = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "ACTIVO")
              }, constraints: table =>
              {
                  table.PrimaryKey("PK_Configuracion", x => x.Id);
                  table.ForeignKey(
                       name: "FK_Configuracion_permiso",
                       column: x => x.Id_permiso,
                       principalTable: "Permisos",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                  table.ForeignKey(
                       name: "FK_Configuracion_rol",
                       column: x => x.Id_rol,
                       principalTable: "Rol",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
              }
            );
            migrationBuilder.CreateIndex(
               name: "IX_Configuracion_Id_permiso",
               table: "Configuracion",
               column: "Id_permiso");

            migrationBuilder.CreateIndex(
                name: "IX_Configuracion_Id_rol",
                table: "Configuracion",
                column: "Id_Rol");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "permisos");

            migrationBuilder.DropForeignKey(
            name: "FK_Configuracion_rol",
            table: "Configuracion");

            migrationBuilder.DropForeignKey(
              name: "FK_Configuracion_permiso",
              table: "Configuracion");

            migrationBuilder.DropIndex(
             name: "IX_Configuracion_Id_rol",
             table: "Configuracion");

            migrationBuilder.DropIndex(
               name: "IX_Configuracion_Id_permiso",
               table: "Configuracion");

            migrationBuilder.DropTable(
                name: "Configuration");
        }
    }
}
