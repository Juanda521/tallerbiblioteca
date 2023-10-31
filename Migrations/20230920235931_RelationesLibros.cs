using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallerbiblioteca.Migrations
{
    /// <inheritdoc />
    public partial class RelationesLibros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
              migrationBuilder.CreateTable(
                name: "AutoresLibros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_autor = table.Column<int>(type: "int", nullable: false),
                    Id_libro = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoresLibros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutoresLibros_Autores_Id_autor",
                        column: x => x.Id_autor,
                        principalTable: "Autores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AutoresLibros_Libros_Id_libro",
                        column: x => x.Id_libro,
                        principalTable: "Libros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
                 migrationBuilder.CreateTable(
                name: "GenerosLibros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_genero = table.Column<int>(type: "int", nullable: false),
                    Id_libro = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerosLibros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenerosLibros_Genero_Id_genero",
                        column: x => x.Id_genero,
                        principalTable: "Genero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenerosLibros_Libros_Id_libro",
                        column: x => x.Id_libro,
                        principalTable: "Libros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutoresLibros_Id_autor",
                table: "AutoresLibros",
                column: "Id_autor");

            migrationBuilder.CreateIndex(
                name: "IX_AutoresLibros_Id_libro",
                table: "AutoresLibros",
                column: "Id_libro");

            migrationBuilder.CreateIndex(
                name: "IX_GenerosLibros_Id_genero",
                table: "GenerosLibros",
                column: "Id_genero");

            migrationBuilder.CreateIndex(
                name: "IX_GenerosLibros_Id_libro",
                table: "GenerosLibros",
                column: "Id_libro");
        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
