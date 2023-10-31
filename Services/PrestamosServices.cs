using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;

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
        
    }
}