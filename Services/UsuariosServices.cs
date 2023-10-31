using System.Security.Claims;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using Microsoft.Build.Framework;
using tallerbiblioteca.Views.Usuarios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace tallerbiblioteca.Services
{
    public class UsuariosServices
    {
        private readonly BibliotecaDbContext _context;
        private int Status;
        private ConfiguracionServices _configuracionServices;
        private EmailServices _emailServices;
        public UsuariosServices(BibliotecaDbContext bibliotecaDbContext,EmailServices emailServices)
        {

            _context = bibliotecaDbContext;
            _configuracionServices = new ConfiguracionServices(_context);
            _emailServices  = emailServices;
        }

        public async Task<List<Usuario>> ObtenerUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }
        public async Task<bool> Create(Usuario usuario)
        {
            _context.Add(usuario);
            _emailServices.SendEmail(_emailServices.EmailRegisterUser(usuario.Correo));
            return (await _context.SaveChangesAsync() > 0) ? true : false;
        }

        public int Edit(ClaimsPrincipal User)
        {
            var Id_rol_string = User.FindFirst(ClaimTypes.Role).Value;
            int Id_rol = Int32.Parse(Id_rol_string);

            return this.Status = _configuracionServices.ValidacionConfiguracionActiva("Editar_Usuario", Id_rol);
           
        }

        public async Task<int> Edit(Usuario usuario){
         
            var existingUser = await _context.Usuarios.FindAsync(usuario.Id);

          

            existingUser.Name = usuario.Name;
            existingUser.Apellido = usuario.Apellido;
            existingUser.Correo = usuario.Correo;
            existingUser.Numero_documento = usuario.Numero_documento;
            existingUser.Contraseña = usuario.Contraseña;
        

            // Guarda los cambios en la base de datos
            _context.Update(existingUser);
            await _context.SaveChangesAsync();
            return 200;
        }

        public Usuario BuscarUsuario(Usuario usuario)
        {
            var UserEncontrado =  _context.Usuarios.FirstOrDefault(u => u.Contraseña == usuario.Contraseña && u.Numero_documento == usuario.Numero_documento);
            if (UserEncontrado != null)
            {
                return UserEncontrado;
            }
            return UserEncontrado  =new Usuario() ;
        }
           

        public int Suspender(int id, ClaimsPrincipal User)
        {
            var Id_rol_string = User.FindFirst(ClaimTypes.Role)?.Value;

            int Id_rol = Int32.Parse(Id_rol_string);
            //debe ser el mismo  nombre de la tabla permisos
            this.Status = _configuracionServices.ValidacionConfiguracionActiva("Suspender_Usuario", Id_rol);
            //si el estado que nos devolvio la validacion de la accion a realizar es correcta (status 200) podremos realizar la accion
            var usuario = _context.Usuarios.Find(id);
            if (Status == 200)
            {
                if (usuario != null)
                {
                    usuario.Estado = "SUSPENDIDO";
                    _context.SaveChanges();
                }
            }
            return Status;

        }

        public int Inhabilitar(int id, ClaimsPrincipal User)
        {
            var Id_rol_string = User.FindFirst(ClaimTypes.Role)?.Value;
            int Id_rol = Int32.Parse(Id_rol_string);
            this.Status = _configuracionServices.ValidacionConfiguracionActiva("Inhabilitar_Usuario", Id_rol);
            //si el estado que nos devolvio la validacion de la accion a realizar es correcta (status 200) podremos realizar la accion
            var usuario = _context.Usuarios.Find(id);
            if (Status == 200)
            {
                if (usuario != null)
                {
                    usuario.Estado = "INHABILITADO";
                    _context.SaveChanges();
                }
            }
            return Status;
        }

        public List<Usuario> Buscar(string filtroBusqueda)
        {
            return _context.Usuarios
                .Where(u => u.Name.Contains(filtroBusqueda) || u.Apellido.Contains(filtroBusqueda) || u.Correo.Contains(filtroBusqueda))
                .ToList();
        }

        public async Task<Usuario> Buscar(int Id){
            var usuario  =await _context.Usuarios.FindAsync(Id);

            if(usuario!=null){
                return usuario;
            }
            return new();
        }

        public async Task<int> Login(Usuario usuario, HttpContext httpContext)
        {

            var UserEncontrado = _context.Usuarios.FirstOrDefault(u => u.Contraseña == usuario.Contraseña && u.Numero_documento == usuario.Numero_documento);
            if (UserEncontrado != null)
            {
                if (UserEncontrado.Estado == "Inhabilitado")
                {
                    return this.Status = 403;  // Usuario Inhabilitado
                }
                else if (UserEncontrado.Estado == "Suspendido")
                {
                    return this.Status = 402; //usuario suspendido
                }

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,UserEncontrado.Name),
                           new Claim(ClaimTypes.Email,UserEncontrado.Correo),
                              new Claim(ClaimTypes.Role,UserEncontrado.Id_rol.ToString()),
                                 new Claim("Id",UserEncontrado.Id.ToString()),
                    };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    // Puedes configurar propiedades de autenticación como la expiración de la cookie, etc.
                };

                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            }
            return this.Status = 400;

        }

        public void EliminarSancion(int id)
        {
            var usuarios = _context.Usuarios.Find(id);

            if (usuarios != null)
            {
                usuarios.Estado = "ACTIVO";
                _context.SaveChanges();
            }
        }
    }
}

