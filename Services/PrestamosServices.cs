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
            
            try
            {
                return await _context.Prestamos
                    .Include(p => p.Peticion)
                        .ThenInclude(p => p.Usuario)
                    .Include(p => p.Peticion)
                        .ThenInclude(p => p.Ejemplar)
                            .ThenInclude(p => p.Libro)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error al obtener los préstamos: {ex.Message}");
                return null; // O maneja el error de otra manera que consideres más adecuada para tu aplicación
            }
        }

       public List<Prestamo> BuscarPrestamos(string busqueda, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                Console.WriteLine("Vamos a buscar PRESTAMOS");
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
                        p.Peticion.Id_ejemplar.ToString().Contains(busqueda) || 
                        p.Peticion.Ejemplar.Libro.Nombre.ToLower().Contains(busqueda) ||
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
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error al buscar préstamos: {ex.Message}");
                return new List<Prestamo>(); // O maneja el error de otra manera que consideres más adecuada para tu aplicación
            }
        }

       
        public async Task<Prestamo> Buscar(int id)
        {
            try
            {
                var peticion = await _context.Prestamos
                    .Include(p => p.Peticion)
                        .ThenInclude(p => p.Usuario)
                    .Include(p => p.Peticion)
                        .ThenInclude(p => p.Ejemplar)
                            .ThenInclude(e => e.Libro)
                    .SingleAsync(p => p.Id == id);

                return peticion;
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error al buscar el préstamo: {ex.Message}");
                return null; // O maneja el error de otra manera que consideres más adecuada para tu aplicación
            }
        }


        public async Task<List<Peticiones>> ObtenerPeticiones(){
            return await _peticionesServices.ObtenerpeticionesEnEspera();
        }

        public async Task<Peticiones>BuscarPeticion( int id){
            return await _peticionesServices.Buscar(id);
        }

        public async Task<ResponseModel> Registrar(Prestamo prestamo, ClaimsPrincipal User)
        {
            try
            {
                int status = _configuracionServices.ValidacionConfiguracionActiva("Registrar_prestamo", _configuracionServices.ObtenerRolUserOnline(User));
                if (status == 200)
                {
                    prestamo.Fecha_inicio = obtenerFechaActual();
                    prestamo.Fecha_fin = ObtenerFechaFinal(prestamo.Fecha_inicio);
                    Console.WriteLine(prestamo.Id_peticion);
                    prestamo.Peticion = await _peticionesServices.Buscar(prestamo.Id_peticion);
                
                    prestamo.Peticion.Estado = "ACEPTADA";
                    prestamo.Peticion.Ejemplar.EstadoEjemplar = "EN PRESTAMO";
                    _context.Prestamos.Add(prestamo);
                    
                    // Guardar los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Este es el correo al que se enviará el correo de confirmación de préstamo: " + prestamo.Peticion.Usuario.Correo);
                
                    // Enviamos correo al usuario confirmando su solicitud
                    _emailServices.SendEmail(_emailServices.EmailPrestamo(prestamo));
                }
                else
                {
                    Console.WriteLine("A mirar qué está fallando :( Código de estado: " + status);
                }
                return _configuracionServices.MensajeRespuestaValidacionPermiso(status);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error al registrar el préstamo: {ex.Message}");
                // Aquí podrías devolver un mensaje específico indicando que ocurrió un error al registrar el préstamo
                return new ResponseModel { Mensaje = "Ocurrió un error al registrar el préstamo.", Icono = "error" };
            }
        }


        public DateTime obtenerFechaActual(){
            return DateTime.Now;
        }

        public DateTime ObtenerFechaFinal(DateTime Fecha_inicio){
            return Fecha_inicio.AddDays(15);
        }

        public async Task<Prestamo> ObtenerPrestamo(int id)
        {
            try
            {
                return await _context.Prestamos.FindAsync(id);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error al obtener el préstamo: {ex.Message}");
                return null; // O maneja el error de otra manera que consideres más adecuada para tu aplicación
            }
        }

       public async Task<ResponseModel> Editar(Prestamo prestamo, ClaimsPrincipal user, DateTime Fecha_fin)
        {
            try
            {
                Console.WriteLine("Llegamos a editar el préstamo");
                int status = _configuracionServices.ValidacionConfiguracionActiva("editar_prestamo", _configuracionServices.ObtenerRolUserOnline(user));
                var resultado = _configuracionServices.MensajeRespuestaValidacionPermiso(status);
                if (status == 200)
                {
                    Console.WriteLine("Aquí ya va a editar el préstamo");

                    Console.WriteLine($"Esta es la fecha fin actual del préstamo: {prestamo.Fecha_fin}");
                    prestamo.Fecha_fin = Fecha_fin;
                    Console.WriteLine($"Esta es la fecha fin actualizada: {prestamo.Fecha_fin}");

                    _context.Update(prestamo);
                    await _context.SaveChangesAsync();
                }
                return resultado;
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error al editar el préstamo: {ex.Message}");
                return new ResponseModel { Mensaje = "Ocurrió un error al editar el préstamo.", Icono = "error" };
            }
        }

       
    }
}