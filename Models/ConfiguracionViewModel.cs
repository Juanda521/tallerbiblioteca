namespace tallerbiblioteca.Models
{
    public class ConfiguracionViewModel
    {
        public Rol Rol { get; set; }
        public Configuracion Configuracion { get; set; }

        public Permiso Permiso { get; set; }
        public List<Configuracion> Configuraciones { get; set; }
        public List<Configuracion> ConfiguracionesAdmin { get; set; }
        public List<Configuracion> ConfiguracionesUsuario { get; set; }
        public List<Configuracion> ConfiguracionesAlfabetizador { get; set; }
        public List<Rol> Roles { get; set; }
        public List<Permiso> Permisos { get; set; }
    }
}
