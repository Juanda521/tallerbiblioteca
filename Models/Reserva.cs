using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tallerbiblioteca.Models
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Ejemplar")]
        public int IdEjemplar { get; set; }
        public Ejemplar Ejemplar{get; set;}  =new();
        
        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }

        public Usuario Usuario {get; set;}  =new();
     
        public DateTime FechaReserva { get; set; }
        public string Estado { get; set; } = "";
    }
}
