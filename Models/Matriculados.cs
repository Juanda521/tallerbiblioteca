using System.ComponentModel.DataAnnotations;

namespace tallerbiblioteca.Models
{
    public class Matriculados
    {
        [Key]
        public int Id { get; set; }
        public long Documento { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

    }
}