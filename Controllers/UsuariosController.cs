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

namespace tallerbiblioteca.Controllers
{
    [Authorize]
    public class Usuarios : Controller
    {
        private readonly BibliotecaDbContext _context;
        private readonly UsuariosServices _usuariosServices;
        private int Status; //this will to be for the control of permissions
        public Usuarios(BibliotecaDbContext context,UsuariosServices usuariosServices)
        {
            _context = context;
            _usuariosServices = usuariosServices;
        }
      
        [AllowAnonymous]
        public IActionResult Login()
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
       
            var usuariosPaginados = usuarios.Skip((pagina - 1) * itemsPagina).Take(itemsPagina).ToList();
           

            var model = new Paginacionnn<Usuario>(usuariosPaginados, totalUsuarios, pagina, itemsPagina);


            return View(model);
          
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

     
            MensajeRespuestaValidacionPermiso(_usuariosServices.Edit(User));
            return RedirectToAction(nameof(Index));
            
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
            
            if (_context.Usuarios.FirstOrDefault(u => u.Numero_documento == usuario.Numero_documento) != null)
            {
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

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,UserEncontrado.Name),
                           new Claim(ClaimTypes.Email,UserEncontrado.Correo),
                              new Claim(ClaimTypes.Role,UserEncontrado.Id_rol.ToString()),
                                new Claim(ClaimTypes.NameIdentifier,UserEncontrado.Id.ToString())
                                //  new Claim("Id",UserEncontrado.Id.ToString()),
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

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Id_rol,Numero_documento,Name,Apellido,Correo,Contraseña,Estado")] Usuario usuario)
        {
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
            
            return View();
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

