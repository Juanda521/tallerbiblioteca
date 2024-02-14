﻿using System.Security.Claims;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using Microsoft.Build.Framework;
using tallerbiblioteca.Views.Usuarios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;


namespace tallerbiblioteca.Services
{
    public class UsuariosServices
    {
        private readonly BibliotecaDbContext _context;
        private int Status;
        private ConfiguracionServices _configuracionServices;
        private EmailServices _emailServices;
        private RolServices _rolServices;
        public UsuariosServices(BibliotecaDbContext bibliotecaDbContext,EmailServices emailServices,RolServices rolServices, ConfiguracionServices configuracionServices)
        {

            _context = bibliotecaDbContext;
            _configuracionServices = configuracionServices;
            _emailServices  = emailServices;
            _rolServices  =rolServices;
        }
        public async Task<List<Usuario>> ObtenerUsuariosPdf()
        {
            return await _context.Usuarios.Include(u=>u.Rol).ToListAsync();
        }

        public async Task<List<Usuario>> ObtenerUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }
        public async Task<List<Usuario>> ValidarUsuario()
        {
            return await _context.Usuarios.Where(u=>u.Estado=="ACTIVO").ToListAsync();
        }
        public async Task<bool> Create(Usuario usuario)
        {   
            Console.WriteLine($"vamos a registrar un usuario con este rol: {usuario.Id_rol}");
            usuario.Contraseña = Encryptar(usuario.Contraseña);
            usuario.Rol = _rolServices.ObtenerRol(usuario.Id_rol);
            _context.Add(usuario);
            _emailServices.SendEmail(_emailServices.EmailRegisterUser(usuario.Correo));
            return (await _context.SaveChangesAsync() > 0) ? true : false;
        }

        public string Encryptar(string cadena_encriptar){
            Encrypt encrypt = new();
            string resultado = encrypt.Encryptar(cadena_encriptar);
            Console.WriteLine(resultado);
            return resultado;
          
            
        }
        //nsi esta funcion nos devuelve 200, nos dejara acceder a la vista para editar, de lo contrario saldra el status por el cual no se pudo realizar la accion
        public int VistaEdit(ClaimsPrincipal User)
        {
          //hacemos uso de la funcion validacion.... para obtener el status si el usuario en linea puede realizar la accion
          //tambien hacemos uso de la funcion obtenerRo.... para extraer  del objeto tipo User que tenemos guardado en el claim el id del rol del usuario
            return this.Status = _configuracionServices.ValidacionConfiguracionActiva("Editar_Usuario", _configuracionServices.ObtenerRolUserOnline(User));
        }

        public async Task<int> Edit(Usuario usuario){
         
            var existingUser = await _context.Usuarios.FindAsync(usuario.Id);

            existingUser.Name = usuario.Name;
            existingUser.Apellido = usuario.Apellido;
            existingUser.Correo = usuario.Correo;
            existingUser.Numero_documento = usuario.Numero_documento;
            existingUser.Contraseña = Encryptar(usuario.Contraseña);


            existingUser.Rol = _rolServices.ObtenerRol(existingUser.Id_rol);
            // Guarda los cambios en la base de datos
            _context.Update(existingUser);
            await _context.SaveChangesAsync();
            return 200;
        }

        public Usuario? BuscarUsuario(Usuario usuario)
        {
            Encrypt encrypt = new();
            Console.WriteLine(usuario.Contraseña);
            usuario.Contraseña  = encrypt.Encryptar(usuario.Contraseña);
            Console.WriteLine(usuario.Contraseña);
            var UserEncontrado =  _context.Usuarios.Include(u=>u.Rol).FirstOrDefault(u => u.Contraseña == usuario.Contraseña && u.Numero_documento == usuario.Numero_documento);
            if (UserEncontrado != null)
            {
               Console.WriteLine($" hola el nombre del rol a iniciar session es: {UserEncontrado.Rol.Nombre}");
                return UserEncontrado;
            }
            return null;
        }

        public bool validarDocumento(BigInteger documento)
        {
            return _context.Usuarios.Any(i => (long)i.Numero_documento == (long)documento);

        }

        public bool validarCorreo( string correo)
        {
            return _context.Usuarios.Any(c => c.Correo == correo);
            
        }

        public async Task<bool> ValidacionMatriculado(BigInteger documento)
        {
            var matriculados = _context.Matriculados
                .AsNoTracking()
                .FirstOrDefault(i => i.Documento == (long)documento);

            return matriculados != null ? true : false;
        }
        public int Suspender(int id, ClaimsPrincipal User)
        {
            var Id_rol_string = User.FindFirst(ClaimTypes.Role)?.Value;

            int Id_rol = Int32.Parse(Id_rol_string);
            //debe ser el mismo  nombre de la tabla permisos
            this.Status = _configuracionServices.ValidacionConfiguracionActiva("Suspender_usuario", Id_rol);
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
            this.Status = _configuracionServices.ValidacionConfiguracionActiva("Inhabilitar_usuario", Id_rol);
            //si el estado que nos devolvio la validacion de la accion a realizar es correcta (status 200) podremos realizar la accion
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id);
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
        public async Task<bool> ValidarApellidoExistente(BigInteger documento, string nombre)
        {
            Console.WriteLine("ENTRANDO A VALIDAR apellido");
            nombre = nombre.ToUpper();
            long documentoLong = (long)documento;

            var usuarioExistente = await _context.Matriculados                  
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Documento == documentoLong && u.Apellido.ToUpper() == nombre.ToUpper());
            if (usuarioExistente!= null)
            {
                Console.WriteLine("EL NOMBRE COINCIDE");
                return false;
            }
            else
            {
                Console.WriteLine("EL NOMBRE NO COINCIDE");
                return true;
            }
        }
        public async Task<bool> ValidarNombreExistente(BigInteger documento, string nombre)
        {
            Console.WriteLine("ENTRANDO A VALIDAR NOMBRE");
            nombre = nombre.ToLower();
            long documentoLong = (long)documento;
            var usuarioExistente = await _context.Matriculados
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Documento == documentoLong && u.Nombre==nombre.ToUpper());
            if (usuarioExistente!= null)
            {
                Console.WriteLine("EL NOMBRE COINCIDE");
                return false;
            }
            else
            {
                Console.WriteLine("EL NOMBRE NO COINCIDE");
                return true;
            }
        }
        public (int,string,Usuario) RecuperarContraseña(int NumeroDocumento)
        {
            int codigo = 0;
            string mensajeError  =null;
            var usuario = _context.Usuarios.Include(u=>u.Rol).FirstOrDefault(u =>u.Numero_documento == NumeroDocumento);
            if (usuario != null)
            {
                if (usuario.Correo==null)
                {
                    Console.WriteLine("no hay correo registrado");

                }
                else
                {
                    codigo = GeneracionCodigo();
                    Console.WriteLine($";vamos a enviar el codigo {codigo} al siguiete correo {usuario.Correo}");
                    _emailServices.SendEmail(_emailServices.EmailRecuperarContraseña(usuario,codigo));
                }   
               
            }else{
                Console.WriteLine("no esta encontrando el usuario");
                mensajeError = "No se encontro un usuario con este numero de documento";
            }
            return (codigo,mensajeError,usuario);
        }
        public int GeneracionCodigo()
        {
            var random = new Random();
            return random.Next(10000,99999);
        }
        public bool ValidarPassword(string password)
        {
            // Expresión regular para validar la contraseña
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";

            // Validar la contraseña con la expresión regular
            return Regex.IsMatch(password, pattern);
        }
        public bool ValidarUsuarioEnPrestamo(int id_usuario){

             // Utilizamos alias para hacer más legible el código
                var prestamoQuery = _context.Prestamos.Include(p => p.Peticion);

                // Verificamos si existe algún préstamo en curso para el usuario dado
                if (prestamoQuery.Any(p => p.Peticion.Usuario.Id == id_usuario && p.Estado == "En curso"))
                {
                    return true;
                }

                return false;
        }
        public async Task<List<Usuario>>Buscar(string busqueda)
        {
            return await _context.Usuarios.Where(u => u.Name == busqueda || u.Apellido == busqueda || u.Correo == busqueda || u.Numero_documento.ToString().Contains(busqueda)).ToListAsync();
        }
    }
}


