using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;
using Microsoft.AspNetCore.Authorization;

namespace tallerbiblioteca.Controllers
{
    [Authorize]
    public class PrestamosController : Controller
    {
        private readonly BibliotecaDbContext _context;
        private readonly PrestamosServices _prestamosServices;
        

        public PrestamosController(BibliotecaDbContext context,PrestamosServices prestamosServices)
        {
            _context = context;
            _prestamosServices = prestamosServices;
            
        }

        // GET: Prestamos
        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin,string busqueda,int Numero_pagina = 1, int itemsPagina = 10)
        {
            
            var prestamos = await _prestamosServices.ObtenerPrestamos(); 

            var rolUsuario = User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"el rol del usuario en linea es: {rolUsuario}");

            var idUsuarioOnline  = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Console.WriteLine($"el documento del usuario en linea es: {idUsuarioOnline}");

            if (rolUsuario!="1" && rolUsuario!="3"){
                Console.WriteLine("hay en linea un usuario");
                prestamos =  prestamos.Where(p=>p.Peticion.Usuario.Id.ToString()==idUsuarioOnline).ToList();
            }else{
                Console.WriteLine("hay en linea un administrador o un alfabetizador");
            }

            if (busqueda!=null || fechaInicio!=null || fechaFin !=null){
                Console.WriteLine("vamos a buscar");
                prestamos =  _prestamosServices.BuscarPrestamos(busqueda,fechaInicio,fechaFin);
            }

            var PrestamosPaginados = prestamos.OrderBy(p => p.Estado == "En curso" ? 0 : 1).Skip((Numero_pagina - 1) * itemsPagina).Take(itemsPagina).ToList();
            
            
            int total_prestamos  = prestamos.Count(); 

            Paginacion<Prestamo> paginacion  =new Paginacion<Prestamo>(PrestamosPaginados,total_prestamos,Numero_pagina,itemsPagina);

            return View(paginacion);
        }
        
        public  IActionResult Calendario()
        {
            return View();
        }

       
        [HttpGet]
        [Route("/api/Calendario")]
        public async Task<IActionResult> Prestamos(){
           var prestamos  = await _prestamosServices.ObtenerPrestamos();
            return Json(prestamos);
        }

        //GET: Prestamos/Create
        public async Task<IActionResult> Create()
        {
            var peticiones = await _prestamosServices.ObtenerPeticiones();

            ViewBag.PeticionesCount  = peticiones.Count;

            foreach (var item in peticiones)
            {
                Console.WriteLine($"id peticion: {item.Id}");
            }
            ViewData["Peticion"] = new SelectList( peticiones, "Id", "Usuario.Name");
            ViewData["Id_usuario"] = new SelectList(_context.Usuarios, "Id", "Name");
            return View();
        }



    
        public async Task<IActionResult> Created(int id)
        {

            Console.WriteLine(id);
            Prestamo prestamo = new Prestamo
            {
                Id_peticion = id // este sera el id de la peticion la cual estamos aceptando
            };

            var peticion = await _prestamosServices.BuscarPeticion(id);
            if (peticion!=null){

                if(peticion.Estado == "ACEPTADA"){
                Console.WriteLine("la peticion ya ha sido aceptada");
                ResponseModel resultado = new()
                {
                    Mensaje = "No puedes realizar esta accion ya que la peticion ya ha sido aceptada",
                    Icono = "info"
                };

                MensajeRespuestaValidacionPermiso(resultado);
                return RedirectToAction("Index","Peticiones");
                }else if(peticion.Estado == "RECHAZADA"){
                     Console.WriteLine("la peticion ya ha sido aceptada");
                ResponseModel resultado = new()
                {
                    Mensaje = "No puedes realizar esta accion ya que la peticion ya ha sido rechazada",
                    Icono = "info"
                };

                MensajeRespuestaValidacionPermiso(resultado);
                return RedirectToAction("Rechazadas","Peticiones");
                }
            
         
            }else{
                Console.WriteLine("no esta encontrando la peticion aceptada");
            }

            MensajeRespuestaValidacionPermiso(await _prestamosServices.Registrar(prestamo,User));
            return RedirectToAction(nameof(Index));

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Id_peticion,Fecha_inicio,Fecha_fin,Estado")] Prestamo prestamo)
        {
            Console.WriteLine("estamos en create");
            Console.WriteLine(prestamo.Fecha_inicio);
            var resultado = await  _prestamosServices.Registrar(prestamo,User);
            TempData["Mensaje"] =JsonConvert.SerializeObject(resultado);
            return RedirectToAction(nameof(Index));
            
        }

         private void MensajeRespuestaValidacionPermiso(ResponseModel resultado){
            
            TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            
        }   
         //GET: Prestamos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Prestamos == null)
            {
                return NotFound();
            }

            var prestamo = await _prestamosServices.ObtenerPrestamo(id);
            if (prestamo == null)
            {
                return NotFound();
            }
           
            return View(prestamo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Id_peticion,Fecha_inicio,Fecha_fin,Estado")] Prestamo prestamo)
        {


            if (id != prestamo.Id)
            {
                return NotFound();
            }
            Console.WriteLine($"asi esta llegando la fecha inicio al controlador: {prestamo.Fecha_inicio}");
            Console.WriteLine($"asi esta llegando la fecha fin al controlador: {prestamo.Fecha_fin}");

            //inicilizamos el objeto resultado para almacenar el mensaje que se va mostrar en la alerta
             ResponseModel resultado = new ResponseModel();

            //buscamos el prestamo actual al que pertenece el prestamo a editar
            var PrestamoExistente = await _prestamosServices.Buscar(prestamo.Id);

            if(PrestamoExistente!=null){
                if(PrestamoExistente.Estado == "Devuelto"){
                    resultado.Mensaje = "No puedes renovar un préstamo que ya ha sido finalizado";
                    resultado.Icono = "error";
                }else{
                    Console.WriteLine("el prestamo esta en curso");
                    
                    Console.WriteLine($"asi esta  la fecha fin del existente: {PrestamoExistente.Fecha_fin}");
                    Console.WriteLine($"asi esta llegando la fecha fin al controlador para actualizar: {prestamo.Fecha_fin}");

                    if (PrestamoExistente.Fecha_fin>=prestamo.Fecha_fin){
                        Console.WriteLine("no se puede actualizar el prestamo con una fecha antes de la que tiene registrada");
                        resultado.Mensaje = "No puedes poner la fecha final del prestamo antes de la actual registrada. ";
                        resultado.Icono = "error";
                        TempData["Mensaje"] =JsonConvert.SerializeObject(resultado);
                        return RedirectToAction(nameof(Index));
                    }

                    resultado = await  _prestamosServices.Editar(PrestamoExistente,User,prestamo.Fecha_fin);
                }

            }else{
                Console.WriteLine("no encontro el prestamo sabiendo que es el mismo id");
            }
            TempData["Mensaje"] =JsonConvert.SerializeObject(resultado);
            return RedirectToAction(nameof(Index));
            
        }
 


    }
}

//         // GET: Prestamos/Details/5
//         public async Task<IActionResult> Details(int? id)
//         {
//             if (id == null || _context.Prestamos == null)
//             {
//                 return NotFound();
//             }

//             var prestamo = await _context.Prestamos
//                 .Include(p => p.Ejemplar)
//                 .Include(p => p.Usuario)
//                 .FirstOrDefaultAsync(m => m.Id == id);
//             if (prestamo == null)
//             {
//                 return NotFound();
//             }

//             return View(prestamo);
//         }

//         // GET: Prestamos/Create
//         public IActionResult Create()
//         {
//             // ViewData["Id_libro"] = new SelectList(_context.Libros, "Id", "Nombre");
//             ViewData["Id_usuario"] = new SelectList(_context.Usuarios, "Id", "Name");
//             return View();
//         }

//         // POST: Prestamos/Create
//         // To protect from overposting attacks, enable the specific properties you want to bind to.
//         // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create([Bind("Id,Id_usuario,Id_ejemplar")] Prestamo prestamo)
//         {
//             //if (!ModelState.IsValid)
//             //{

//             //    ViewData["Id_libro"] = new SelectList(_context.Libros, "Id", "Nombre", prestamo.Id_libro);
//             //    ViewData["Id_usuario"] = new SelectList(_context.Usuarios, "Id", "Name", prestamo.Id_usuario);
//             //    return View(prestamo);
//             //}
//             prestamo.Fecha_inicio = DateTime.Now;
//             prestamo.Fecha_fin = prestamo.Fecha_inicio.AddDays(15);
//             Console.WriteLine(prestamo.Fecha_inicio);
//             Console.WriteLine(prestamo.Fecha_fin);
//             Console.WriteLine(prestamo.Id_ejemplar);
//             Console.WriteLine(prestamo.Id_usuario);
//             var Ejemplar = await _context.Ejemplares.FindAsync(prestamo.Id_ejemplar);
//             var Usuario  = await _context.Usuarios.FindAsync(prestamo.Id_usuario);


//             prestamo.Ejemplar = Ejemplar;
//             prestamo.Usuario = Usuario;
           
            

            
//             _context.Prestamos.Add(prestamo);
//             // _context.Prestamos.Add(prestamo);
//             await _context.SaveChangesAsync();
//             return RedirectToAction(nameof(Index));

//         }

//        

//       

//         // GET: Prestamos/Delete/5
//         public async Task<IActionResult> Delete(int? id)
//         {
//             if (id == null || _context.Prestamos == null)
//             {
//                 return NotFound();
//             }

//             var prestamo = await _context.Prestamos
//                 .Include(p => p.Ejemplar)
//                 .Include(p => p.Usuario)
//                 .FirstOrDefaultAsync(m => m.Id == id);
//             if (prestamo == null)
//             {
//                 return NotFound();
//             }

//             return View(prestamo);
//         }

//         // POST: Prestamos/Delete/5
//         [HttpPost, ActionName("Delete")]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> DeleteConfirmed(int id)
//         {
//             if (_context.Prestamos == null)
//             {
//                 return Problem("Entity set 'BibliotecaDbContext.Prestamos'  is null.");
//             }
//             var prestamo = await _context.Prestamos.FindAsync(id);
//             if (prestamo != null)
//             {
//                 _context.Prestamos.Remove(prestamo);
//             }
            
//             await _context.SaveChangesAsync();
//             return RedirectToAction(nameof(Index));
//         }

//         private bool PrestamoExists(int id)
//         {
//           return (_context.Prestamos?.Any(e => e.Id == id)).GetValueOrDefault();
//         }
//     }
// }
