using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tallerbiblioteca.Models
{
    public class Ejemplar
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Libro")]
        public int Id_libro { get; set; }
        public Libro Libro {get; set;}  = new();

        public string Isbn_libro { get; set; } = "";

        public string EstadoEjemplar { get; set; } = "";

    }
}