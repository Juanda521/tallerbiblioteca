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
    public class AutoresServices
    {
        private readonly BibliotecaDbContext _context;
        private int Status;
        private ConfiguracionServices _configuracionServices;
        private LibrosServices _librosServices;

        public AutoresServices(BibliotecaDbContext bibliotecaDbContext, ConfiguracionServices configuracionServices, LibrosServices librosServices)
        {

            _context = bibliotecaDbContext;
            _configuracionServices = configuracionServices;
            _librosServices = librosServices;
        }

        public async Task<Autor> Buscar(int Id)
        {
            var autor = await _context.Autores.Where(a => a.Id == Id ).FirstOrDefaultAsync();
            if (autor != null)
            {
                return autor;
            }
            return new();
        }

        public List<Libro> ObtenerLibrosPorAutor(int idAutor)
        {
            return _context.AutoresLibros
                .Where(al => al.Id_autor == idAutor)
                .Select(al => al.Libro)
                .ToList();
        }


        // public async Task<Devolucion> BarraBusqueda(int busqueda)
        // {
        //     if(busqueda!=null)
        //     {
        //         busqueda=await _context.Devoluciones.Where(d=>d.Id.Containts(busqueda)).ToListAsync();
        //     }
        // }

        public async Task<int> Registrar(Autor autor, ClaimsPrincipal User)
        {

            int Status = _configuracionServices.ValidacionConfiguracionActiva("Registrar_autor", _configuracionServices.ObtenerRolUserOnline(User));


            if (Status == 200)
            {
                var nombre_autor = autor.NombreAutor;
                
                _context.Add(autor);
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("A mirar que esta fallando :(" + Status);

            }
            return Status;

        }

        public List<Autor>busqueda(string busqueda ){
            return  _context.Autores.Where(l=>l.NombreAutor.ToLower().Contains(busqueda)).ToList();
        }

        public async Task<List<Autor>>ObtenerAutores(){
            return await _context.Autores.ToListAsync();
        }

        public async Task<int> Editar(Autor autor, ClaimsPrincipal User)
        {

            int Status = _configuracionServices.ValidacionConfiguracionActiva("Actualizar_autor", _configuracionServices.ObtenerRolUserOnline(User));


            if (Status == 200)
            {
                var autor_nombre=  autor.NombreAutor;



                _context.Update(autor);
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

            int Status = _configuracionServices.ValidacionConfiguracionActiva("Eliminar_autor", _configuracionServices.ObtenerRolUserOnline(User));


            if (Status == 200)
            {
                
                var autor = await _context.Autores.FindAsync(id);
                _context.Autores.Remove(autor);
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

        public int Desactivar(int id, ClaimsPrincipal User)
        {
            var Id_rol_string = User.FindFirst(ClaimTypes.Role)?.Value;

            int Id_rol = Int32.Parse(Id_rol_string);
            //debe ser el mismo  nombre de la tabla permisos
            this.Status = _configuracionServices.ValidacionConfiguracionActiva("Actualizar_autor", Id_rol);
            //si el estado que nos devolvio la validacion de la accion a realizar es correcta (status 200) podremos realizar la accion
            var autor = _context.Autores.Find(id);
            if (Status == 200)
            {
                if (autor != null)
                {
                    autor.Estado = "Inhabilitado";
                    _context.SaveChanges();
                }
            }
            return Status;

        }

         public int Activar(int id, ClaimsPrincipal User)
        {
            var Id_rol_string = User.FindFirst(ClaimTypes.Role)?.Value;

            int Id_rol = Int32.Parse(Id_rol_string);
            //debe ser el mismo  nombre de la tabla permisos
            this.Status = _configuracionServices.ValidacionConfiguracionActiva("Actualizar_autor", Id_rol);
            //si el estado que nos devolvio la validacion de la accion a realizar es correcta (status 200) podremos realizar la accion
            var autor = _context.Autores.Find(id);
            if (Status == 200)
            {
                if (autor != null)
                {
                    autor.Estado = "Activo";
                    _context.SaveChanges();
                }
            }
            return Status;

        }


    }
}


