using System.ComponentModel.DataAnnotations.Schema;

namespace tallerbiblioteca.Models
{
    public class AutorLibro
    {
        public int Id { get; set; }

        [ForeignKey("Autor")]
        public int Id_autor { get; set; }
        public Autor Autor { get; set; }  = new();

        [ForeignKey("Libro")]
        public int Id_libro { get; set; }
        public Libro Libro { get; set; } = new();

    }
}
