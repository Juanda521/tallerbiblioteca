using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;
using System.Net.Mail;
using System.Numerics;

namespace tallerbiblioteca.Controllers
{
    [Authorize]
    public class Usuarios : Controller
    {
        private readonly BibliotecaDbContext _context;
        private readonly UsuariosServices _usuariosServices;
        private int Status; 
        public Usuarios(BibliotecaDbContext context,UsuariosServices usuariosServices)
        {
            _context = context;
            _usuariosServices = usuariosServices;
        }
      
        [AllowAnonymous]
        public IActionResult login()
        {
            return View();
        }

        [AllowAnonymous]
        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["Id_rol"] = new SelectList(_context.Rol, "Id", "Nombre");
            return View();
        }

        // GET: Usuarios
        public async Task<IActionResult> Index(string busqueda ,int pagina=1, int itemsPagina=6 )
        {
         
            var usuarios = await _usuariosServices.ObtenerUsuarios();
            if (busqueda != null){
                 busqueda = busqueda.ToLower();
                if(int.TryParse(busqueda,out int Numero_documento)){
                    usuarios = usuarios.Where (u=>u.Name.ToLower().Contains(busqueda) || u.Apellido.ToLower().Contains(busqueda) || u.Numero_documento.ToString().Contains(busqueda )).ToList();
                }else{
                     usuarios = usuarios.Where (u=>u.Name.Contains(busqueda) || u.Apellido.Contains(busqueda)).ToList();
                }
            }
            

            var totalUsuarios = usuarios.Count();
            var total = (totalUsuarios / 6)+1; 
       
            var usuariosPaginados = usuarios.Skip((pagina - 1) * itemsPagina).Take(itemsPagina).ToList();
           

            var model = new Paginacion<Usuario>(usuariosPaginados, total, pagina, itemsPagina);



            return View(model);
          
        }

     

 

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
           int status  = _usuariosServices.VistaEdit(User);
            if(status==200){
                var usuario = await _context.Usuarios.FindAsync(id);
                return View(usuario);
            }else{
                MensajeRespuestaValidacionPermiso(status);
                return RedirectToAction(nameof(Index));
            }
            
 
        }

        

      //cambiar if a switch
        private void MensajeRespuestaValidacionPermiso(int status)         
        {

            var resultado = new ResponseModel();

            if (status == 200)
            {       
                resultado.Mensaje  =  "La accion se ha realizado con exito";
                resultado.Icono  = "success";
                // TempData["Mensaje"] = "La accion se ha realizado con exito";
                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            }
            else if (status == 401)
            {  //si el permiso no lo puede realizar el usuario debido a que su rol no le permite realizar la accion ( status 401)
                resultado.Mensaje  =  "No tienes permiso para realizar esta accion";
                resultado.Icono  = "error";
                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            }
            else if (status == 402)
            {
                resultado.Mensaje  = "El permiso para realizar esta accion no se encuentra activo";
                resultado.Icono  = "info";
                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            }
            else
            {
                Console.WriteLine("i'm failing in the name of permission");
            }
            //return (string)TempData["Mensaje"];
        }

        public IActionResult Logout()
        {
            var n = true;
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (n == true)
            {
                ViewData["Cerrar"] = "true";
            }
            return View("Login");
        }
        
        [HttpGet]
        public IActionResult Inhabilitar(int id)
        {
            MensajeRespuestaValidacionPermiso(_usuariosServices.Inhabilitar(id, User));
          
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Suspender(int id)
        {
            MensajeRespuestaValidacionPermiso(_usuariosServices.Suspender(id, User));
          
            return RedirectToAction(nameof(Index));

        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([Bind("Numero_documento,Contraseña")] Usuario usuario)
        {
            Console.WriteLine("estamos en login");
            Console.WriteLine("hola");
            if (_context.Usuarios.FirstOrDefault(u => u.Numero_documento == usuario.Numero_documento) != null)
            {
                Console.WriteLine(usuario.Contraseña);
                var UserEncontrado = _usuariosServices.BuscarUsuario(usuario);
                if (UserEncontrado != null)
                {
                    if (UserEncontrado.Estado == "Inhabilitado")
                    {
                        TempData["ErrorMessage"] = "No puedes ingresar al aplicativo, te encuentras Inhabilitado";
                        return View();
                    }
                    else if (UserEncontrado.Estado == "Suspendido")
                    {
                        TempData["ErrorMessage"] = "No puedes ingresar al aplicativo, te encuentras Suspendido";
                        return View();
                    }
                    Console.WriteLine($"el nombre dle usuario a iniciar session es: {UserEncontrado.Name}");
                     Console.WriteLine($"el nombre del rol a iniciar session es: {UserEncontrado.Rol.Nombre}");
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,UserEncontrado.Name),
                           new Claim(ClaimTypes.Email,UserEncontrado.Correo),
                              new Claim(ClaimTypes.Role,UserEncontrado.Id_rol.ToString()),
                                new Claim(ClaimTypes.NameIdentifier,UserEncontrado.Id.ToString()),
                                   new(ClaimTypes.Role, UserEncontrado.Rol.Nombre),
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        // Puedes configurar propiedades de autenticación como la expiración de la cookie, etc.
                    };

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    ViewData["Iniciar"] = "true";
                }
                else
                {
                    TempData["Credencial"] = "Contrasena incorrecta, verifica el campo";
                }
            }
            else
            {
                TempData["Credencial"] = "documento incorrecto, verifique su campo";
                return View();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("Id,Name,Apellido,Correo,Numero_documento,Contraseña")] Usuario usuario)
        {
           
            MensajeRespuestaValidacionPermiso(await _usuariosServices.Edit(usuario));
            return RedirectToAction(nameof(Index));
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);

                // Verificar el dominio usando un servicio DNS
                var host = addr.Host;
                var mxRecords = System.Net.Dns.GetHostAddresses(host)
                    .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                return mxRecords.Any(); // Devolver true si hay registros MX para el dominio
            }
            catch
            {
                return false;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Id_rol,Numero_documento,Name,Apellido,Correo,Contraseña,Estado")] Usuario usuario)
        {
            var nopasar = false;
            if (_usuariosServices.validarCorreo(usuario.Correo))
            {
                nopasar = true;
                Console.WriteLine("Encontrado, no dejar entrar");
                ViewData["Encontrado"] = "True";
                ViewData["Id_rol"] = new SelectList(_context.Rol, "Id", "Nombre");
                return View("Create", usuario);
            }
            var nombreExistente = await _usuariosServices.ValidarNombreExistente(usuario.Numero_documento, usuario.Name);
            if (nombreExistente==true)
            {
                Console.WriteLine("ABRIR SWEET ALERT");
                ViewData["NombreExistente"] = "True";
                ViewData["Id_rol"] = new SelectList(_context.Rol, "Id", "Nombre");
                return View("Create", usuario);
            }
            var validarApellido = await _usuariosServices.ValidarApellidoExistente(usuario.Numero_documento, usuario.Apellido);
            if (validarApellido==true)
            {
                Console.WriteLine("ABIRR SWEET ALERT APELLIDO");
                ViewData["ApellidoExistente"] = "True";
                ViewData["Id_rol"] = new SelectList(_context.Rol, "Id", "Nombre");
                return View("Create", usuario);
            }
           
            if (!IsValidEmail(usuario.Correo))
            {
                nopasar = true;
                Console.WriteLine("Correo electrónico no válido, no dejar entrar");
                ViewData["CorreoInvalido"] = "True";
                ViewData["Id_rol"] = new SelectList(_context.Rol, "Id", "Nombre");
                return View("Create", usuario);
            }

          
            BigInteger bigintV = BigInteger.Parse(usuario.Numero_documento.ToString());
            bool esMatriculadoe = _usuariosServices.validarDocumento(bigintV);
            if (esMatriculadoe)
            {
                nopasar = true;
                Console.WriteLine("Encontrado, no dejar entrar");
                ViewData["Encontrados"] = "True";
                 ViewData["Id_rol"] = new SelectList(_context.Rol, "Id", "Nombre");
                return View("Create", usuario);
            }
            Console.WriteLine("ESTAMOS VALIDANDO TODO");
            BigInteger bigint = BigInteger.Parse(usuario.Numero_documento.ToString());
            bool esMatriculado = await _usuariosServices.ValidacionMatriculado(bigint);
            if (esMatriculado)
            {
                if (nopasar == false)
                {
                    Console.WriteLine("Se encontró el documento dejar entrar");

                    if (_context.Rol.Any(r => r.Id == usuario.Id_rol))
                    {

                        await this._usuariosServices.Create(usuario);
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("Id_rol", "El rol seleccionado no es válido.");
                    }
                    ViewData["Id_rol"] = new SelectList(_context.Rol, "Id", "Nombre");
                }
                return View();
            }
            else
            {
                nopasar = true;
                Console.WriteLine("No se encontró el documento en matriculado, no dejar entrar");
                ViewData["Nomatriculado"] = "True";
                return View("Create", usuario);
            }
        }
        public IActionResult eliminarsan(int id)
        {
            var usuarios = _context.Usuarios.Find(id);

            if (usuarios != null)
            {
                usuarios.Estado = "ACTIVO";
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> buscar(string filtro)
        {
            var buscar = await _context.Usuarios
                .Where(u => u.Name.Contains(filtro) || u.Apellido.Contains(filtro) || u.Correo.Contains(filtro))
                .ToListAsync();

            return View("Index", buscar);

        }
        public async Task<IActionResult> Informacion(int id)
        {
            var buscar = _context.Peticiones.FirstOrDefault(k => k.Id_usuario == id);
            var buscusu = _context.Usuarios.FirstOrDefault(t => t.Id == id);
            if (buscar != null)
            {
                if (buscar.Estado == "ACEPTADA")
                {
                    var libro = buscar.Id_ejemplar;
                    var idejemplar = _context.Ejemplares.FirstOrDefault(r => r.Id == libro);
                    var idfin = idejemplar.Id_libro;
                    var idlibro = _context.Libros.FirstOrDefault(l => l.Id == idfin);
                    if (idlibro != null)
                    {
                        var nombre = idlibro.Nombre;
                        var fecha = _context.Prestamos.FirstOrDefault(k => k.Id_peticion == buscar.Id);
                        var usuario = new usuarioo
                        {
                            Mensaje = "Tienes un prestamo pendiente ",
                            NombreLibro = $"Nombre del libo en prestamo: {nombre}",
                            fechaa = $"Tiene una fecha de entrega para el {fecha.Fecha_fin}"
                        };                    
                        usuario.Name = buscusu.Name;
                        usuario.Apellido = buscusu.Apellido;
                        usuario.Correo = buscusu.Correo;                    
                        return View("Informacion", usuario);
                    }
                }
                else if (buscar.Estado == "EN ESPERA")
                {
                    var usuarioa = new usuarioo { Mensaje = $"tienes una petición en espera" };
                    usuarioa.Name = buscusu.Name;
                    usuarioa.Apellido = buscusu.Apellido;
                    usuarioa.Correo = buscusu.Correo;
                    return View("Informacion", usuarioa);
                }
            }
            else
            {
                var usuarioa = new usuarioo { Mensaje = "No tienes ninguna petición  pendiente" };
                usuarioa.Name = buscusu.Name;
                usuarioa.Apellido = buscusu.Apellido;
                usuarioa.Correo = buscusu.Correo;
                return View("Informacion", usuarioa);
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Recuperar()
        {
            Console.WriteLine("Está entrando");
            return View("RecuperarContraseña");
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RecuperarContraseña()
        {
            int codigo =0;
           
            ResponseModel resultado = new();
            string numero_documento = Request.Form["Numero_documento"];
            Console.WriteLine(numero_documento);
            if (Int32.TryParse(numero_documento, out Int32 numero_documento_int))
            {
                Console.WriteLine("vamos a parsear el numero de documento");
                var (codigoServicio,mensajeError,UsuarioServicios) =  _usuariosServices.RecuperarContraseña(numero_documento_int);
                codigo = codigoServicio;
                Usuario usuario = UsuarioServicios;
                if (!string.IsNullOrEmpty(mensajeError))
                {
                    ViewData["ErrorMessage"] = mensajeError;
                    return View();
                }
                else
                {
                    HttpContext.Session.SetString("CodigoFinal", codigo.ToString());
                    // Serializar el objeto a JSON
                    var usuarioJson = JsonConvert.SerializeObject(usuario);

                    // Almacenar el JSON en la sesión
                    HttpContext.Session.SetString("Usuario", usuarioJson);

                }
            }  
            else
            {
                Console.WriteLine("no esta parseando el numero de documento");
                ViewData["ErrorMessage"] = "El número de documento no es válido.";
                return View();
            }
           
            Console.WriteLine($"Este es el codigo que genera{codigo}");
           
            return View("ConfirmarCodigo");
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ConfirmarCodigo()
        {
           
            ResponseModel resultado = new();
            string codigo = Request.Form["codigo"];
            Console.WriteLine(codigo);
            if (Int32.TryParse(codigo, out Int32 codigo_int))
            {
                int codigoGenerado = Convert.ToInt32(HttpContext.Session.GetString("CodigoFinal"));
                Console.WriteLine($"Este es el codigo que ingrese en el formulario{codigo_int}");
                Console.WriteLine($"Este es el codigo que genera los servicios{codigoGenerado}");
                if (codigo_int == codigoGenerado)
                {
                    Console.WriteLine("LISTO, NOS VAMOS A RESTABLECER");
                    return View("RestablecerContraseña");
                }

            }
            ViewData["ErrorMessage"] = "No coinciden los Numeros, Revisa de nuevo por favor";
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> restablecer(string contraseña)
        {
            //para que pruebe de una JAJAJAJA
            Console.WriteLine(contraseña);

            if(!_usuariosServices.ValidarPassword(contraseña)){
                ViewData["ErrorMessage"] = "La contraseña debe contener mayuscula, minuscula, numeros y caracteres especiales";
                return View("restablecerContraseña");
            }

            var usuarioJson = HttpContext.Session.GetString("Usuario");
            // Deserializar el JSON a tu objeto de usuario
            var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
            usuario.Contraseña = contraseña;

            Console.WriteLine(usuario.Contraseña);

           
            TempData["succesfullMessage"] = "Tu contraseña se ha restablecido exitosamente, puedes iniciar session con la nueva contraseña";
            //aca´se acaba todo juan, acá ya mandamos para login, que inicie sesión de nuevo
            return RedirectToAction("login");
        }

       


    }
}
public class Paginacionnn<Usuario>{
    public List<Usuario> Usuarios { get; }
    public int TotalItems { get; }
    public int PageNumber { get; }
    public int PageSize { get; }

    public Paginacionnn(List<Usuario> usuarios, int totalItems, int pageNumber, int pageSize)
    {
        Usuarios  = usuarios;
        TotalItems = totalItems;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

