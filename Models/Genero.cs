using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tallerbiblioteca.Models
{
    public class Genero
    {
        public int Id { get; set; }

         [Required(ErrorMessage = "Este campo es obligatorio")]
        public string NombreGenero { get; set; } = "";

        public string Estado {get; set;}  ="ACTIVO";

        [NotMapped]
        public List<Libro> Libros {get; set;}  =new();
        


    }
}
