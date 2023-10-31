using System.Security.Claims;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using Microsoft.EntityFrameworkCore;
// using System.Linq.Async;

namespace tallerbiblioteca.Services
{
    public class PeticionesServices
    {
        private readonly BibliotecaDbContext _context;
        private int Status;
        private ConfiguracionServices _configuracionServices;
        private EjemplarServices _ejemplarServices;

        private UsuariosServices _usuariosServices;

        private EmailServices _emailServices;
        public PeticionesServices(BibliotecaDbContext bibliotecaDbContext,UsuariosServices usuariosServices,EjemplarServices ejemplarServices,EmailServices emailServices)
        {
            _context = bibliotecaDbContext;
            _configuracionServices = new ConfiguracionServices(_context);
            _usuariosServices  = usuariosServices;
            _ejemplarServices  = ejemplarServices;
            _emailServices = emailServices;
        
        }

        public async Task<List<Peticiones>> Obtenerpeticiones(){
            return await _context.Peticiones.Include(p=>p.Usuario).ToListAsync();
        }

        public async Task<Peticiones> Buscar (int id){
              var peticion = await _context.Peticiones.Include(p => p.Usuario).Include(p=>p.Ejemplar).SingleAsync(p => p.Id == id);
            if (peticion!=null){
                return peticion ;
            }
            return new();
        }

        public async Task<int> EliminarPeticion(ClaimsPrincipal User,int id){

            var Id_rol_string = User.FindFirst(ClaimTypes.Role).Value;  
            int Id_rol = Int32.Parse(Id_rol_string);
            Status = _configuracionServices.ValidacionConfiguracionActiva("Eliminar_Peticion",Id_rol);

            if (Status ==200){
                var peticion = await Buscar(id);
                if (peticion != null)
                {
                    var ejemplar = await _ejemplarServices.BuscarEjemplar(peticion.Id_ejemplar);
                    if(ejemplar!=null){
                        ejemplar.EstadoEjemplar = "DISPONIBLE";
                    }
                    _context.Peticiones.Remove(peticion);
                } 
                await _context.SaveChangesAsync();
            }
            Console.WriteLine("este es el status final: "+Status);
            return Status;
        }

        public async Task<int> Registrar(ClaimsPrincipal User,Peticiones peticion){
            

            var Id_rol_string = User.FindFirst(ClaimTypes.Role).Value;  
            int Id_rol = Int32.Parse(Id_rol_string);
            Status =  _configuracionServices.ValidacionConfiguracionActiva("Registrar_Peticion",Id_rol);
            Console.WriteLine("vamos a registrar una peticion con este ejemplar: "+peticion.Id_ejemplar);
            if (Status==200){
                peticion.FechaPeticion  = ObtenerFechaActual();

                if(await ValidacionPeticionPendiente(peticion)){
                    //temporal
                     return Status = 500;

                }
                var usuario = await _usuariosServices.Buscar(peticion.Id_usuario);


                // var correo = new SendEmailDTO(){
                //     Para = usuario.Correo,
                //     Contenido  = "este es un mensaje de prueba pa desde bookware",
                //     Asunto  = "pir**** hpt**** care**** gono***"

                // };

                // _emailServices.SendEmail(correo);
                // Console.WriteLine("ya debio haber enviado el correo al correo: "+usuario.Correo);

                switch (usuario.Estado)
                {
                    case "SUSPENDIDO":
                        //status de error al usuario estar suspendido (no puede realizar la accion asi tenga el permiso el rol para hacerla)
                    return Status = 501;
                    case "INHABILITADO":
                        //status de error al usuario estar Inhabilitado (no puede realizar la accion asi tenga el permiso el rol para hacerla)
                    return Status = 501;
                }



                var ejemplar = await _ejemplarServices.BuscarEjemplar(peticion.Id_ejemplar);
            //  var ejemplar = await _context.Ejemplares.FindAsync(peticion.Id_ejemplar);
                if(ejemplar!=null && usuario!=null){
                    peticion.Ejemplar = ejemplar;
                    peticion.Ejemplar.EstadoEjemplar = "EN PETICION";
                    peticion.Usuario = usuario;
                    _context.Add(peticion);
                    //enviamos coorreo de peticion al administrador para que este tome las medidas necesarias
                    _emailServices.SendEmail(_emailServices.EmailPeticion(peticion));
                    await _context.SaveChangesAsync();
                }else{
                    Console.WriteLine("despues miramos esto");
                }
            }
            return Status;
        }

        private DateTime ObtenerFechaActual(){
            return  DateTime.Now;
           
        }
        //nos devuelve true si el usuario que esta realizando la peticion ya tiene una pendiente
        private async Task<bool> ValidacionPeticionPendiente(Peticiones peticion){
            
            return await _context.Peticiones.AnyAsync(u => u.Id_usuario == peticion.Id_usuario);
        }

        public ResponseModel MensajeRespuestaValidacionPermiso(int status){
            return _configuracionServices.MensajeRespuestaValidacionPermiso(status);
        }
    }
}