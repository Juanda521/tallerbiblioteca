using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace tallerbiblioteca.Models
{
    public class Rol
    {
        public int Id { get; set; } 
        [Required(ErrorMessage = "este campo es obligatorio.")]
        public string Nombre { get; set; } = "";
        [Required(ErrorMessage = "este campo es obligatorio.")]
        public string Estado { get; set; }  = "";
    }
}
