using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tallerbiblioteca.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [ForeignKey("Rol")]
        public int Id_rol { get; set; }
        public Rol Rol { get; set; }

      
        public int Numero_documento { get; set; }

        public string Name{get; set; }
       
        public string Apellido { get; set; }

        public string Correo { get; set; }

        
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Contraseña {get; set; }

        public string Estado { get; set; }  
    }
}
