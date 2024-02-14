using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Claims;
using tallerbiblioteca.Context;
using tallerbiblioteca.Migrations;
using tallerbiblioteca.Models;
namespace tallerbiblioteca.Services
{

    public class ReservasServices
    {
        private readonly BibliotecaDbContext _context;
        private EjemplarServices _ejemplarservices;
        private UsuariosServices _usuariosServices;
        private ConfiguracionServices _configuracionServices;
        public ReservasServices(BibliotecaDbContext context, EjemplarServices ejemplarservices, UsuariosServices usuariosServices, ConfiguracionServices configuracionServices)
        {
            _context = context;
            _ejemplarservices = ejemplarservices;
            _usuariosServices = usuariosServices;
            _configuracionServices = configuracionServices;
        }
        public async Task<List<Ejemplar>> obtenerEjemplares()
        {
            try
            {
                var ejemplares = await _context.Ejemplares.Include(e => e.Libro).Where(e => e.EstadoEjemplar == "NO DISPONIBLE" || e.EstadoEjemplar == "EN PETICION").ToListAsync();
                return ejemplares;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public async Task<List<Ejemplar>> buscarEj()
        {
            var ejemplares = await obtenerEjemplares();
            return ejemplares;
        }
        public async Task<List<Usuario>> buscarUs()
        {
            return _context.Usuarios.ToList();
        }
        public async Task<List<Reserva>> ObtenerReservasPdf()
        {
            return _context.Reserva.Include(r => r.Ejemplar).ThenInclude(l => l.Libro).Include(r => r.Usuario)
                .ToList();
        }
        public async Task<List<Reserva>> ObtenerReservas()
        {
            return _context.Reserva.Include(r => r.Ejemplar).ThenInclude(l => l.Libro).Include(r => r.Usuario).Where(r => r.Estado == "ACTIVO")
                .ToList();
        }
        public async Task<ResponseModel> Crear(Reserva reserva, ClaimsPrincipal User)
        {
            Console.WriteLine("LLEGAMOS A CREAR....");
            int status = _configuracionServices.ValidacionConfiguracionActiva("Registrar_reserva", _configuracionServices.ObtenerRolUserOnline(User));
            if (status == 200)
            {
                reserva.FechaReserva = DateTime.Now;
                reserva.Ejemplar = await _ejemplarservices.BuscarEjemplar(reserva.IdEjemplar);
                reserva.Usuario = await _usuariosServices.Buscar(reserva.IdUsuario);

                reserva.Estado = "ACTIVO";
                _context.Reserva.Add(reserva);
                _context.SaveChanges();

            }
            var resultado = _configuracionServices.MensajeRespuestaValidacionPermiso(status);
            return resultado;


        }
        public async Task<bool> Buscar(int dato)
        {
            var buscar = _context.Reserva.FirstOrDefaultAsync(p => p.IdEjemplar == dato);
            if (buscar != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<ResponseModel> Cambiarestado(int id, ClaimsPrincipal User)
        {
            int status = _configuracionServices.ValidacionConfiguracionActiva("Rechazar_reserva", _configuracionServices.ObtenerRolUserOnline(User));
            if (status == 200)
            {
                var encontrar = await _context.Reserva.FirstOrDefaultAsync(e => e.Id == id);
                if (encontrar != null)
                {
                    encontrar.Estado = "RECHAZADA";
                    await _context.SaveChangesAsync();
                }
            }
            var resultado = _configuracionServices.MensajeRespuestaValidacionPermiso(status);
            return resultado;
        }
        public async Task<Reserva> enviarR(int id)
        {
            return _context.Reserva.FirstOrDefault(c => c.Id == id);

        }
        public async Task<bool> Encontrarreserva(int idusuario)
        {
            Console.WriteLine("BUSCANDO RESERVA");
            var buscar = _context.Reserva.FirstOrDefault(p => p.IdUsuario == idusuario);
            if (buscar != null)
            {
                Console.WriteLine("RESERVA ENCONTRADA");
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<Reserva>> Rechazadas()
        {
            return _context.Reserva.Include(r => r.Ejemplar).ThenInclude(l => l.Libro).Include(r => r.Usuario).Where(r => r.Estado == "RECHAZADA")
            .ToList();
        }
        public async Task<List<Reserva>> Buscar(string buscar)
        {
            return _context.Reserva.Where(r => r.Ejemplar.Libro.Nombre == buscar && r.Estado == "ACTIVO" || r.Estado == "ACEPTADA" || r.Usuario.Name == buscar.ToUpper() && r.Estado == "ACTIVO" || r.Estado == "ACEPTADA").ToList();
        }
        public async Task<List<Reserva>> BuscarR(int id)
        {
            Console.WriteLine("BUSCANDO PORQUE ES ROL 2");
            return _context.Reserva.Include(r => r.Ejemplar).ThenInclude(l => l.Libro).Include(r => r.Usuario).Where(r => r.Usuario.Id==id)
                .ToList();
        }
        public async Task<List<Reserva>> Buscarporfecha(DateTime? Fecha)
        {
            if (Fecha.HasValue)
            {
                Fecha = Fecha.Value.Date;
                return _context.Reserva
                    .Where(f => f.FechaReserva.Date == Fecha && (f.Estado == "ACTIVO" || f.Estado == "ACEPTADA"))
                    .Include(r => r.Ejemplar)
                    .ThenInclude(l => l.Libro)
                    .Include(r => r.Usuario)
                    .ToList();
            }
            else
            {
                // Si la fecha es nula, devuelve una lista vacía o maneja el caso según tus necesidades
                return new List<Reserva>();
            }
            //return _context.Reserva.Where(f=>f.FechaReserva == Fecha && f.Estado=="ACTIVO" || f.Estado=="ACEPTADA").Include(r => r.Ejemplar).ThenInclude(l => l.Libro).Include(r => r.Usuario).ToList();
        }
    }
}
