using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallerbiblioteca.Migrations
{
    /// <inheritdoc />
    public partial class Peticiones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

              migrationBuilder.CreateTable(
                name: "Genero",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreGenero = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genero", x => x.Id);
                });

                  migrationBuilder.CreateTable(
                name: "Autores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreAutor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autores", x => x.Id);
                });

                  migrationBuilder.CreateTable(
                name: "Libros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CantidadLibros = table.Column<int>(type:"int",nullable: false, defaultValue:0),
                    ImagenLibro =  table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion =  table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.Id);
                });


             migrationBuilder.CreateTable(
              name: "Ejemplares",
              columns: table => new
              {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Id_libro = table.Column<int>(type: "int", nullable: false),
                Isbn_libro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EstadoEjemplar = table.Column<string>(type: "nvarchar(max)", nullable: false,defaultValue:"ACTIVO"),
                 
              }, constraints: table =>
              {
                  table.PrimaryKey("PK_Ejemplar", x => x.Id);
              });

             migrationBuilder.CreateTable(
              name: "Peticiones",
              columns: table => new
              {
                  Id = table.Column<int>(type: "int", nullable: false)
                      .Annotation("SqlServer:Identity", "1, 1"),
                  Id_ejemplar = table.Column<int>(type: "int", nullable: false),
                  Id_usuario = table.Column<int>(type: "int", nullable: false),
                  fechaPeticion = table.Column<DateTime>(type: "dateTime", nullable: false),
                  Motivo= table.Column<string>(type: "nvarchar(max)", nullable: false)
              }, constraints: table =>
              {
                  table.PrimaryKey("PK_Peticiones", x => x.Id);
                  table.ForeignKey(
                       name: "FK_Peticiones_Usuario",
                       column: x => x.Id_usuario,
                       principalTable: "Usuarios",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                  table.ForeignKey(
                       name: "FK_Peticiones_ejemplar",
                       column: x => x.Id_ejemplar,
                       principalTable: "Ejemplares",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
              }


            );

                migrationBuilder.AddForeignKey(
                name: "FK_Ejemplares_Libro",
                table: "Ejemplares",
                column: "Id_libro",
                principalTable: "Libros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

                 migrationBuilder.CreateIndex(
                name: "IX_Ejemplares_Id_Libro",
                table: "Ejemplares",
                column: "Id_Libro");

               migrationBuilder.CreateIndex(
                name: "IX_Peticiones_Id_ejemplar",
                table: "Ejemplares",
                column: "Id");

                migrationBuilder.CreateIndex(
                name: "IX_Peticiones_Id_usuario",
                table: "Usuarios",
                column: "Id");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
               migrationBuilder.DropTable(
                name: "Peticiones");

               migrationBuilder.DropTable(
                name: "Ejemplares");
        }
    }
}
