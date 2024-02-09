using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using tallerbiblioteca.Migrations;
using tallerbiblioteca.Models;

namespace tallerbiblioteca.Services
{
    public class PublicacionesServices
    {
        private readonly BibliotecaDbContext _context;

        public PublicacionesServices(BibliotecaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Publicaciones>> ObtenerPublicaciones()
        {
            var fechaActual = DateTime.Now;
            var publicaciones = await _context.Publicaciones    
           .Where(p => p.Estado == "ACTIVO")
           .ToListAsync();
            foreach (var publicacion in publicaciones)
            {
                if (fechaActual > publicacion.FechaFin)
                {
                    publicacion.Estado = "INACTIVO";
                    await _context.SaveChangesAsync();
                }
            }
            await _context.SaveChangesAsync();

            return publicaciones;
        }

        public async Task<Models.Publicaciones> enviarcambiar(int id)
        {
            try
            {
                var publicaciones = await _context.Publicaciones
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (publicaciones != null)
                {
                    return publicaciones;
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("La publicación no fue encontrada");
            }
            return null;
        }

        public async Task<int> CambiarEstado(int id)
        {
            var publicacion = await _context.Publicaciones.FirstOrDefaultAsync(p => p.Id == id);

            if (publicacion != null)
            {
                publicacion.Estado = "INACTIVO";
                await _context.SaveChangesAsync();
                return 200;
            }
            else
            {
                return 202;
            }
        }
        public async Task<Models.Publicaciones> ObtenerEditar(int id)
        {
            var publicaciones = await _context.Publicaciones.FindAsync(id);
            if(publicaciones == null)
            {
                return null; 
            }
            return publicaciones; 
        }
        public async Task<bool>Editar(Models.Publicaciones publicaciones)
        {
            try
            {
                var existingPublicacion = await _context.Publicaciones.FindAsync(publicaciones.Id);

                if (existingPublicacion != null)
                {
                    existingPublicacion.Tipo = publicaciones.Tipo;
                    existingPublicacion.Nombre = publicaciones.Nombre;
                    existingPublicacion.Descripcion = publicaciones.Descripcion;
                    existingPublicacion.FechaInicio = publicaciones.FechaInicio;
                    existingPublicacion.FechaFin = publicaciones.FechaFin;
                    Console.WriteLine("Editando ando");

                    _context.Update(existingPublicacion);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new Exception("La publicación no fue encontrada");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("La publicación no fue encontrada");

            }
            return false;
        }
        
        public async Task<bool>Crear(Models.Publicaciones publicaciones)
        {
            _context.Publicaciones.Add(publicaciones);
            await _context.SaveChangesAsync();
            return true;
        }

    }


}