using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;

namespace tallerbiblioteca.Services
{
    public class RolServices
    {
        BibliotecaDbContext _context;
        public RolServices(BibliotecaDbContext bibliotecaDbContext)
        {
            _context = bibliotecaDbContext;
        }
        
        public async Task<List<Rol>> ObtenerRoles()
        {
            return await _context.Rol.ToListAsync();
        }
    }
}
