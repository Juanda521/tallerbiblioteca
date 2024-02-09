using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;
using System.Runtime.Serialization;

namespace tallerbiblioteca.Services
{
    public class PrestamosServices
    {
        private readonly BibliotecaDbContext _context;
        private PeticionesServices _peticionesServices;
        private ConfiguracionServices _configuracionServices;
        private EmailServices _emailServices;


        public PrestamosServices(BibliotecaDbContext bibliotecaDbContext,PeticionesServices peticionesServices,ConfiguracionServices configuracionServices,EmailServices emailServices){
            _context  = bibliotecaDbContext;
            _peticionesServices = peticionesServices;
            _configuracionServices = configuracionServices;
            _emailServices = emailServices;

        }


        public async Task<List<Prestamo>> ObtenerPrestamos(){
            
            return await _context.Prestamos.Include(p=>p.Peticion)
                                                .ThenInclude(p=>p.Usuario)
                                            .Include(p=>p.Peticion)
                                                .ThenInclude(p=>p.Ejemplar).ThenInclude(p=>p.Libro)
                                            .ToListAsync();
        }

        public List<Prestamo> BuscarPrestamos(string busqueda, DateTime? fechaInicio, DateTime? fechaFin)
        {
            Console.WriteLine("vamos a buscar PRESTAMOS");
             Console.WriteLine(fechaInicio);
             Console.WriteLine(fechaFin);
            List<Prestamo> prestamos = _context.Prestamos
                .Include(p => p.Peticion)
                    .ThenInclude(p => p.Usuario)
                .Include(p => p.Peticion)
                    .ThenInclude(p => p.Ejemplar).ThenInclude(p => p.Libro)
                .ToList();

            if (!string.IsNullOrEmpty(busqueda))
            {
                prestamos = prestamos.Where(p =>
                    p.Peticion.Usuario.Name.ToLower().Contains(busqueda) ||
                    p.Peticion.Usuario.Numero_documento.ToString().Contains(busqueda) ||
                    p.Peticion.Id_ejemplar.ToString().Contains(busqueda) || p.Peticion.Ejemplar.Libro.Nombre.ToLower().Contains(busqueda) ||
                    p.Estado.ToLower().Contains(busqueda))
                    .ToList();
            }

              if (fechaInicio != null && fechaFin != null)
                {
                    DateTime fechaInicioValue = fechaInicio.Value.Date;
                    DateTime fechaFinValue = fechaFin.Value.Date.AddDays(1); // Incrementa un día para incluir la fecha de fin

                    prestamos = prestamos.Where(p =>
                        p.Fecha_inicio.Date >= fechaInicioValue && p.Fecha_fin.Date < fechaFinValue)
                        .ToList();
                }

            return prestamos;
        }


        // public List<Prestamo> BuscarPrestamos(string busqueda){
            
        //      List<Prestamo> prestamos;
        //      prestamos  = _context.Prestamos.Include(p=>p.Peticion)
        //                                         .ThenInclude(p=>p.Usuario)
        //                                     .Include(p=>p.Peticion)
        //                                         .ThenInclude(p=>p.Ejemplar).ThenInclude(p=>p.Libro).ToList();
        //      if(int.TryParse(busqueda,out int Id_ejemplar)){
        //             prestamos = prestamos.Where(p=>p.Peticion.Usuario.Name.ToLower().Contains(busqueda)||p.Peticion.Usuario.Numero_documento.ToString().Contains(busqueda) || p.Peticion.Id_ejemplar.ToString().Contains(busqueda)).ToList();
        //         }else{
        //              prestamos =  _context.Prestamos.Where(p=>p.Peticion.Usuario.Name.ToLower().Contains(busqueda)).ToList();
        //         }
            

              
        //     return prestamos;
        // }

         public async Task<Prestamo> Buscar(int id)
        {
            var peticion = await _context.Prestamos.Include(p => p.Peticion)
                                                        .ThenInclude(p => p.Usuario)
                                                .Include(p => p.Peticion)
                                                    .ThenInclude(p => p.Ejemplar)
                                                    .ThenInclude(e=>e.Libro).SingleAsync(p => p.Id == id);
            if (peticion != null)
            {
                return peticion;
            }
            return new();
        }

        public async Task<List<Peticiones>> ObtenerPeticiones(){
            return await _peticionesServices.ObtenerpeticionesEnEspera();
        }

        public async Task<Peticiones>BuscarPeticion( int id){
            return await _peticionesServices.Buscar(id);
        }

     



        public async Task<ResponseModel> Registrar(Prestamo prestamo,ClaimsPrincipal User){

            int status = _configuracionServices.ValidacionConfiguracionActiva("Registrar_prestamo",_configuracionServices.ObtenerRolUserOnline(User));
            if(status==200){
              
                prestamo.Fecha_inicio = obtenerFechaActual();
                
              
                prestamo.Fecha_fin = ObtenerFechaFinal(prestamo.Fecha_inicio);
                Console.WriteLine(prestamo.Id_peticion);
                prestamo.Peticion =  await _peticionesServices.Buscar(prestamo.Id_peticion);
           
                prestamo.Peticion.Estado  = "ACEPTADA";
                prestamo.Peticion.Ejemplar.EstadoEjemplar  = "EN PRESTAMO";
                _context.Prestamos.Add(prestamo);
                Console.WriteLine("este es el corrreo al que se enviara el correo de confirmacion de prestamo: "+ prestamo.Peticion.Usuario.Correo);
               
                //enviamos correo a usuario confirmandole su solicitud
                _emailServices.SendEmail(_emailServices.EmailPrestamo(prestamo));
                
                await _context.SaveChangesAsync();
            }else{
                Console.WriteLine("A mirar que esta fallando :("+status);
            }
            return  _configuracionServices.MensajeRespuestaValidacionPermiso(status);
        }

        public DateTime obtenerFechaActual(){
            return DateTime.Now;
        }

        public DateTime ObtenerFechaFinal(DateTime Fecha_inicio){
            return Fecha_inicio.AddDays(15);
        }

        public async Task<Prestamo>ObtenerPrestamo(int id){
            return  await _context.Prestamos.FindAsync(id);
//      
        }

        public DateTime Renovar(DateTime fecha){
            return fecha.AddDays(15);
        }

        public async Task<ResponseModel>Editar(Prestamo prestamo,ClaimsPrincipal user,DateTime Fecha_fin){

            Console.WriteLine("llegamos a editar prestamo");
            int status = _configuracionServices.ValidacionConfiguracionActiva("editar_prestamo",_configuracionServices.ObtenerRolUserOnline(user));
            var resultado  =_configuracionServices.MensajeRespuestaValidacionPermiso(status);
            if(status==200){
                Console.WriteLine("aca ya va editar");

                Console.WriteLine($"esta es la fecha fin actual del prestamo: {prestamo.Fecha_fin}");
                prestamo.Fecha_fin = Fecha_fin;
                  Console.WriteLine($"esta es la fecha fin actualizada: {prestamo.Fecha_fin}");

                  // Obtenemos el tipo de dato de la propiedad Fecha_fin
                    Type type = prestamo.Fecha_fin.GetType();

                    // Imprimimos el tipo de dato
                    Console.WriteLine(type);

               
                  

                 _context.Update(prestamo);
                await _context.SaveChangesAsync();

            }
            return resultado;


        }

       
    }
}