using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace tallerbiblioteca.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [ForeignKey("Rol")]
        public int Id_rol { get; set; }
        public Rol Rol { get; set; } = new();

        [Required(ErrorMessage = "este campo es obligatorio.")]
        [NotMapped]
        public long Numero_documento { get; set; } 

        [Required(ErrorMessage = "este campo es obligatorio.")]
        [RegularExpression(@"^[^0-9]+$",ErrorMessage =  "No se permiten numeros es en este campo")]
        public string Name{get; set; }  ="";

       [Required(ErrorMessage = "este campo es obligatorio.")]
        [RegularExpression(@"^[^0-9]+$",ErrorMessage =  "No se permiten numeros es en este campo")]
        public string Apellido { get; set; }  ="";

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]  
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "El correo electrónico no es válido.")]
        public string Correo { get; set; } = "";

         [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Contraseña {get; set; } = "";

        public string Estado { get; set; }  ="";
    }
    public class usuarioo: Usuario{
        public string Mensaje { get; set;}

        public string NombreLibro {get; set;}

        public string fechaa {get; set;}
    }
}
