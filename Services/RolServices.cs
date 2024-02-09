using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using System.Security.Claims;

namespace tallerbiblioteca.Services
{
    public class RolServices
    {
        BibliotecaDbContext _context;

        // ConfiguracionServices _configuracionServices;
        public RolServices(BibliotecaDbContext bibliotecaDbContext)
        {
            
            _context = bibliotecaDbContext;
            // _configuracionServices = configuracionServices;
        }
        
        public async Task<List<Rol>> ObtenerRoles()
        {
            return await _context.Rol.ToListAsync();
        }

        public List<Rol>Busqueda(string busqueda ){
            return  _context.Rol.Where(l=>l.Nombre.ToLower().Contains(busqueda)||l.Estado.ToLower().Contains(busqueda)).ToList();
        }

        public Rol ObtenerRol(int id){
            return _context.Rol.FirstOrDefault(r=>r.Id == id);
        }

        public async Task<ResponseModel> Editar(ClaimsPrincipal user,Rol rol){

            ConfiguracionServices _configuracionServices  = new ConfiguracionServices(_context,this);

            int status = _configuracionServices.ValidacionConfiguracionActiva("Editar_rol",_configuracionServices.ObtenerRolUserOnline(user));

            if (status == 200){
                _context.Update(rol);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("hicimos todo bien");
            }else{
                Console.WriteLine("algo salio mal");
            }
            
            ResponseModel resultado = _configuracionServices.MensajeRespuestaValidacionPermiso(status);
            return resultado;
        }
    }
}
