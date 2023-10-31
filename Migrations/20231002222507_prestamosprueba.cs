using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallerbiblioteca.Migrations
{
    /// <inheritdoc />
    public partial class prestamosprueba : Migration
    {
        /// <inheritdoc />
   
        protected override void Up(MigrationBuilder migrationBuilder)
        {

             migrationBuilder.CreateTable(
                name: "Prestamos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id_usuario = table.Column<int>(type: "int", nullable: false),
                    Id_ejemplar = table.Column<int>(type: "int", nullable: false),
                    Fecha_inicio  = table.Column<DateTime>(type:"datetime",nullable: false),
                    Fecha_fin = table.Column<DateTime>(type: "datetime", nullable: false),
                    Estado = table.Column<string>(type:"nvarchar(20)",nullable:false,defaultValue:"ACTIVO")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamos", x => x.Id);
                    
                    table.ForeignKey(
                        name: "FK_Prestamos_Ejemplar_Id_ejemplar",
                        column: x => x.Id_ejemplar,
                        principalTable: "Ejemplares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prestamos_Usuarios_Id_usuario",
                        column: x => x.Id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

          

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_Id_ejemplar",
                table: "Prestamos",
                column: "Id_ejemplar");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_Id_usuario",
                table: "Prestamos",
                column: "Id_usuario");
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.DropTable(
                name: "Prestamos");

        }
    }
}
