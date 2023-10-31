using tallerbiblioteca.Models;
using Microsoft.Build.Framework;
using tallerbiblioteca.Views.Usuarios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using System.Security.Claims;

namespace tallerbiblioteca.Services
{
    public class LibrosServices
    {
        private readonly BibliotecaDbContext _context;
        private int Status;
        private ConfiguracionServices _configuracionServices;
        public LibrosServices(BibliotecaDbContext bibliotecaDbContext)
        {

            _context = bibliotecaDbContext;
            _configuracionServices = new ConfiguracionServices(_context);
        }

        public async Task<bool> Registrar(Libro libro,ClaimsPrincipal User)
        {

            var Id_rolString = User.FindFirst(ClaimTypes.Role)?.ToString();
            if(Id_rolString!=null){
                int Id_rol_User = Int32.Parse(Id_rolString);
                if(_configuracionServices.ValidacionConfiguracionActiva("Registrar Libro",Id_rol_User)==200){
                    
                }Status = _configuracionServices.ValidacionConfiguracionActiva("Registrar Libro",Id_rol_User);
                _context.Add(libro);
                 return (await _context.SaveChangesAsync() > 0) ? true : false;
                
            }
           
            _context.Add(libro);
            return (await _context.SaveChangesAsync() > 0) ? true : false;
        } 

        public async Task<List<Libro>>ObtenerLibros(){
            return await _context.Libros.ToListAsync();
        }

       
        
    }
}