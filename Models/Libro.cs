using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace tallerbiblioteca.Models

{
    public class Libro
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public int CantidadLibros { get; set; }

        public string Descripcion{ get; set; }

        public string ImagenLibro{get; set;}

          // Nuevas propiedades para los géneros y autores seleccionados
        [NotMapped]
        public List<int> GeneroIds { get; set; }
        [NotMapped]
        public List<int> AutorIds { get; set; }

       
    }
}
