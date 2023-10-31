using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;

namespace tallerbiblioteca.Services
{
    public class PermisosServices
    {
        private readonly BibliotecaDbContext _context;

        public PermisosServices(BibliotecaDbContext bibliotecaDbContext){
            _context  = bibliotecaDbContext;
        }

        public List<Permiso> ObtenerPermisos(){
            return _context.Permisos.ToList();
        }
    }
}


 