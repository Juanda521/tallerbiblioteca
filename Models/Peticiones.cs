﻿

using System.ComponentModel.DataAnnotations.Schema;

namespace tallerbiblioteca.Models
{
    public class Peticiones
    {
        // [Key]
        // public int idPeticion { get; set; }
        public int Id {get; set;}
        
        [ForeignKey("Ejemplar")]
        public int Id_ejemplar { get; set; }
        public Ejemplar Ejemplar {get;set;} = new ();

        [ForeignKey("Usuario")]
        public int Id_usuario { get; set; }
        public Usuario Usuario {get;set;} = new();

        [ForeignKey("Reserva")]

        public int? Id_reserva {get; set;}

         public virtual Reserva Reserva { get; set; } = new();




        public DateTime FechaPeticion { get; set; }
        public string Motivo { get; set; }  = "Breve descripcion de la peticion";

        public string Estado  {get; set;} = "EN ESPERA";

        public string IdEjemplarAsString{ 
            get { return Id_ejemplar.ToString(); }
        }
        public string IdUsuarioAsString
        {
            get { return Id_usuario.ToString();}
        }
        
    

      
    }
}
