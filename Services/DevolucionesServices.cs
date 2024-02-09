using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;
// using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Win32;
using tallerbiblioteca.Context;
using System.Globalization;

namespace tallerbiblioteca.Services
{
    public class DevolucionesServices
    {
        private readonly BibliotecaDbContext _context;
        private int Status;
        private ConfiguracionServices _configuracionServices;
        private PrestamosServices _prestamosServices;

        public DevolucionesServices(BibliotecaDbContext bibliotecaDbContext, ConfiguracionServices configuracionServices, PrestamosServices prestamosServices)
        {

            _context = bibliotecaDbContext;
            _configuracionServices = configuracionServices;
            _prestamosServices = prestamosServices;
        }

        public async Task<Devolucion> Buscar(int Id)
        {
            var devolucion = await _context.Devoluciones.Include(p => p.Prestamo).SingleAsync(p => p.Id == Id);
            if (devolucion != null)
            {
                return devolucion;
            }
            return new();
        }

        public async Task<bool> BuscarDevolucionExistente(int id){
          try
            {
               
                Console.WriteLine("Estamos en la función de validación");
                var devolucion = await _context.Devoluciones.Include(p => p.Prestamo).FirstOrDefaultAsync(d => d.Id_prestamo == id);
                
                if (devolucion != null)
                {
                    Console.WriteLine("Se encontró una devolución con el ID del préstamo");
                    return true;
                }
                else
                {
                    Console.WriteLine("No hay devoluciones registradas con este ID de préstamo");
                    return false;
                }

               
                
                
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar devolución existente: {ex.Message}");
                return false;
            }

        }

        // public async Task<Devolucion> BarraBusqueda(int busqueda)
        // {
        //     if(busqueda!=null)
        //     {
        //         busqueda=await _context.Devoluciones.Where(d=>d.Id.Containts(busqueda)).ToListAsync();
        //     }
        // }

        public async Task<int> Registrar(Devolucion devolucion, ClaimsPrincipal User)
        {

            int Status = _configuracionServices.ValidacionConfiguracionActiva("Registrar_devolucion", _configuracionServices.ObtenerRolUserOnline(User));


            if (Status == 200)
            {
              
                // var fecha_devolucion = devolucion.Fecha_devolucion;
                // Console.WriteLine(fecha_devolucion);
                var prestamo = await _prestamosServices.Buscar(devolucion.Id_prestamo);
                prestamo.Estado = "Devuelto";
                prestamo.Peticion.Ejemplar.EstadoEjemplar =" DISPONIBLE";

                devolucion.Prestamo = prestamo;

                Console.WriteLine("ya vamos a registrar la devolucion");
                _context.Add(devolucion);
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("A mirar que esta fallando :(" + Status);

            }
            return Status;

        }

        public DateTime obtenerFechaActual(){
            return DateTime.Now;
        }

        public async Task<List<Devolucion>> ObtenerDevoluciones()
        {
            return await _context.Devoluciones.Include(p=>p.Prestamo)
                                                .ThenInclude(p => p.Peticion)
                                                .ThenInclude(p => p.Usuario)
                                            .Include(p => p.Prestamo)
                                                .ThenInclude(p => p.Peticion)
                                                .ThenInclude(p => p.Ejemplar)
                                            .ToListAsync();
        }

        public DateTime ObtenerFechaDevolucion(int Id)
        {

            var devolucionFecha = _context.Prestamos.Find(Id);
            return (DateTime)devolucionFecha.Fecha_fin;

        }

        public async Task<int> Editar(Devolucion devolucion, ClaimsPrincipal User)
        {

            int Status = _configuracionServices.ValidacionConfiguracionActiva("Actualizar_devolucion", _configuracionServices.ObtenerRolUserOnline(User));


            if (Status == 200)
            {
                var prestamo = await _prestamosServices.Buscar(devolucion.Id_prestamo);
                devolucion.Prestamo = prestamo;
                _context.Update(devolucion);
                await _context.SaveChangesAsync();
                // return View(devolucion);

            }
            else
            {
                Console.WriteLine("A mirar que esta fallando :(" + Status);

            }
            return Status;

        }

        public async Task<int> Eliminar(int id, ClaimsPrincipal User)
        {

            int Status = _configuracionServices.ValidacionConfiguracionActiva("Eliminar_devolucion", _configuracionServices.ObtenerRolUserOnline(User));


            if (Status == 200)
            {
                
                var devolucion = await _context.Devoluciones.FindAsync(id);
                var prestamo = await _prestamosServices.Buscar(devolucion.Id_prestamo);
                devolucion.Prestamo = prestamo;
                _context.Devoluciones.Remove(devolucion);
                await _context.SaveChangesAsync();
                // return RedirectToAction(nameof(Index));
            }
            else
            {
                Console.WriteLine("A mirar que esta fallando :(" + Status);

            }
            return Status;
        }

        public ResponseModel MensajeRespuestaValidacionPermiso(int Status)
        {
            return _configuracionServices.MensajeRespuestaValidacionPermiso(Status);
        }


    }
}


