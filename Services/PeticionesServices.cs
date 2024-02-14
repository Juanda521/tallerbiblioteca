using System.Security.Claims;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
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
        public PeticionesServices(BibliotecaDbContext bibliotecaDbContext, ConfiguracionServices configuracionServices, UsuariosServices usuariosServices, EjemplarServices ejemplarServices, EmailServices emailServices)
        {
            _context = bibliotecaDbContext;
            _configuracionServices = configuracionServices;
            _usuariosServices = usuariosServices;
            _ejemplarServices = ejemplarServices;
            _emailServices = emailServices;

        }
        public async Task<List<Peticiones>> BuscarObtener(string busqueda, DateTime? fechaInicio, DateTime? fechaFin)
        {
            IQueryable<Peticiones> query = _context.Peticiones
                .Include(p => p.Usuario)
                .Include(p => p.Ejemplar)
                    .ThenInclude(e => e.Libro);
            if (!string.IsNullOrEmpty(busqueda))
            {
                string busquedaUpper = busqueda.ToUpper();
                query = query.Where(p =>
                    (p.Usuario != null && (p.Usuario.Name.ToUpper().Contains(busquedaUpper) || p.Usuario.Apellido.ToUpper().Contains(busquedaUpper))) ||
                    p.Motivo.ToUpper().Contains(busquedaUpper) ||
                    p.Estado.ToUpper().Contains(busquedaUpper) ||
                    (p.Ejemplar != null && p.Ejemplar.Libro.Nombre.ToUpper().Contains(busquedaUpper))
                );
            }
            if (fechaInicio != null && fechaFin != null)
            {
                query = query.Where(p => p.FechaPeticion >= fechaInicio && p.FechaPeticion <= fechaFin);
            }

            return await query.ToListAsync();
        }
        public async Task<List<Peticiones>>Buscarechazadas(DateTime? fechaini, DateTime? fechaFin, string? busqueda)
        {
        if (busqueda != null)
        {
                 return await _context.Peticiones
                .Include(p => p.Usuario)
                .Include(p => p.Ejemplar)
                    .ThenInclude(e => e.Libro)
                .Where(p =>p.Ejemplar.Libro.Nombre == busqueda || p.Usuario.Name == busqueda || p.Usuario.Apellido == busqueda && p.Estado == "RECHAZADA")
                .ToListAsync();
                }
                else
                {
                    return await _context.Peticiones
                .Include(p => p.Usuario)
                .Include(p => p.Ejemplar)
                    .ThenInclude(e => e.Libro)
                .Where(p => p.Estado == "RECHAZADA" && p.FechaPeticion >= fechaini && p.FechaPeticion <= fechaFin)
                .ToListAsync();
                }
            }
        public async Task<List<Peticiones>> Rechazadas()
        {
            
            return await _context.Peticiones
            .Include(p => p.Usuario)
            .Include(p => p.Ejemplar)
                .ThenInclude(e => e.Libro)
            .Where(p => p.Estado == "RECHAZADA" )
            .ToListAsync();
        }

        public async Task<List<Peticiones>> ObtenerpeticionesPdf()
        {
            return await _context.Peticiones
        .Include(p => p.Usuario)
        .Include(p => p.Ejemplar)
            .ThenInclude(e => e.Libro)
        .ToListAsync();
        }
        
        public async Task<List<Peticiones>> Obtenerpeticiones()
        {
            return await _context.Peticiones
        .Include(p => p.Usuario)
        .Include(p => p.Ejemplar)
            .ThenInclude(e => e.Libro)
        .Where(p => p.Estado == "EN ESPERA" || p.Estado == "ACEPTADA")
        .ToListAsync();
        }
        public async Task<List<Peticiones>> ObtenerpeticionesEnEspera()
        {
            var peticionesEnEspera = await _context.Peticiones
                .Where(p => p.Estado != "ACEPTADA" && p.Estado !="RECHAZADA")
                .Include(p => p.Usuario)
                .Include(p => p.Ejemplar)
                .ThenInclude(e => e.Libro)
                .ToListAsync();

            // Puedes imprimir las peticiones para verificar
            foreach (var peticion in peticionesEnEspera)
            {
                Console.WriteLine($"ID: {peticion.Id}, Estado: {peticion.Estado}");
            }

            return peticionesEnEspera;
        }
        public async Task<Peticiones>Buscar(int id)
        {
            var peticion = await _context.Peticiones.Include(p => p.Usuario).Include(p => p.Ejemplar).ThenInclude(e => e.Libro).SingleAsync(p => p.Id == id);
            if (peticion != null)
            {
                Console.WriteLine("el id del ejemplar relacionado a la peticion es: " + peticion.Ejemplar.Id);
                Console.WriteLine($"el nombre del libro relacionado del ejemplar relacionado a la peticion es: {peticion.Ejemplar.Libro.Nombre}");
                return peticion;
            }
            return new();
        }
        public async Task<List<Peticiones>> BuscarP(int userId)
        {
            var peticiones = await _context.Peticiones
                .Include(p => p.Usuario)
                .Include(p => p.Ejemplar)
                .ThenInclude(e => e.Libro)
                .Where(p => p.Usuario.Id == userId)
                .ToListAsync();

            foreach (var peticion in peticiones)
            {
                Console.WriteLine($"El id del ejemplar relacionado a la peticion es: {peticion.Ejemplar.Id}");
                Console.WriteLine($"El nombre del libro relacionado del ejemplar relacionado a la peticion es: {peticion.Ejemplar.Libro.Nombre}");
            }

            return peticiones;
        }
        public async Task<Prestamo>BuscarPeticionAceptada(int id){

            return await _context.Prestamos.SingleOrDefaultAsync(p=>p.Id_peticion == id);
        }
        public async Task<int> EliminarPeticion(ClaimsPrincipal User, int id)
        {

            var Id_rol_string = User.FindFirst(ClaimTypes.Role).Value;
            int Id_rol = Int32.Parse(Id_rol_string);
            Status = _configuracionServices.ValidacionConfiguracionActiva("Eliminar_peticion", Id_rol);

            if (Status == 200)
            {
                var peticion = await Buscar(id);
                if (peticion != null)
                {
                    var ejemplar = await _ejemplarServices.BuscarEjemplar(peticion.Id_ejemplar);
                    if (ejemplar != null)
                    {
                        ejemplar.EstadoEjemplar = "DISPONIBLE";
                    }
                    peticion.Estado = "RECHAZADA";

                }
                await _context.SaveChangesAsync();
            }
            Console.WriteLine("este es el status final: " + Status);
            return Status;
        }
        public async Task<int> Registrar(ClaimsPrincipal User, Peticiones peticion)
        {


            Status = _configuracionServices.ValidacionConfiguracionActiva("Registrar_peticion", _configuracionServices.ObtenerRolUserOnline(User));
            Console.WriteLine("vamos a registrar una peticion con este ejemplar: " + peticion.Id_ejemplar);
            if (Status == 200)
            {
                peticion.FechaPeticion = ObtenerFechaActual();

                if (await ValidacionPeticionPendiente(peticion))
                {

                    return Status = 500;

                }
                var usuario = await _usuariosServices.Buscar(peticion.Id_usuario);


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
                if (ejemplar != null && usuario != null)
                {
                    // ejemplar.Estado  ="EN PRESTAMO";
                    peticion.Ejemplar = ejemplar;
                    peticion.Ejemplar.EstadoEjemplar = "EN PETICION";
                
                    peticion.Usuario = usuario;
                    _context.Add(peticion);
                    Libro libro = await _ejemplarServices.BuscarLibro(ejemplar.Id_libro);
                    Console.WriteLine("el nombre del libro es:" + libro.Nombre);
                    //enviamos correo de peticion al administrador para que este tome las medidas necesarias
                    _emailServices.SendEmail(_emailServices.EmailPeticion(peticion));
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("despues miramos esto");
                }
            }
            return Status;
        }
        public int ObtenerRolUserOnline(ClaimsPrincipal User){
            return _configuracionServices.ObtenerRolUserOnline(User);
        }
        private DateTime ObtenerFechaActual()
        {
            return DateTime.Now;

        }
        private async Task<bool> ValidacionPeticionPendiente(Peticiones peticion)
        {

            return await _context.Peticiones.AnyAsync(u => u.Id_usuario == peticion.Id_usuario && u.Estado == "EN ESPERA");
        }

        public ResponseModel MensajeRespuestaValidacionPermiso(int status)
        {
            return _configuracionServices.MensajeRespuestaValidacionPermiso(status);
        }
    }
}