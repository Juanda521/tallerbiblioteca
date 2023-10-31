using System.ComponentModel.DataAnnotations.Schema;

namespace tallerbiblioteca.Models
{
    public class GeneroLibro
    {
        public int Id { get; set; }

        [ForeignKey("Genero")]
        public int Id_genero { get; set; }
        public Genero Genero { get; set; }

        [ForeignKey("Libro")]
        public int Id_libro { get; set; }
        public Libro Libro { get; set; }
    }
}
