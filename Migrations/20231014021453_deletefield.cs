using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tallerbiblioteca.Migrations
{
    /// <inheritdoc />
    public partial class deletefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
  migrationBuilder.DropColumn(
                name: "Id_usuario",
                table: "Prestamos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
