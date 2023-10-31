using System.ComponentModel.DataAnnotations.Schema;

namespace tallerbiblioteca.Models
{
    public class Configuracion
    {
        public int Id { get; set; }

        [ForeignKey("Rol")]
        public int Id_rol { get; set; }
        public Rol Rol { get; set; }

        [ForeignKey("Permiso")]
        public int Id_permiso { get; set; }
        public Permiso Permiso { get; set; }

        public string Estado { get; set; }
    }
}
