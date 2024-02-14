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
       // GET: Prestamos
        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin,string busqueda,int Numero_pagina = 1, int itemsPagina = 10, int? id = null)
        {
            try
            {
                var prestamos = await _prestamosServices.ObtenerPrestamos();
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

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
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error en la función Index: {ex.Message}");
                return RedirectToAction("Error", "Home"); // Redirigir a una página de error
            }
        }

        
        public  IActionResult Calendario()
        {
            return View();
        }

       
        [HttpGet]
        [Route("/api/Calendario")]
        public async Task<IActionResult> Prestamos(){
            try
            {
                var prestamos  = await _prestamosServices.ObtenerPrestamos();
                return Json(prestamos);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error en la función Prestamos: {ex.Message}");
                return BadRequest(); // Retornar un código de error 400
            }
        }

        [HttpGet]
        [Route("/api/Graficas")]
        public async Task<IActionResult> PrestamosGrafica(){
           try
            {
                var prestamos  = await _prestamosServices.ObtenerPrestamos();

                var prestamosPorLibro = prestamos
                    .GroupBy(p => p.Peticion.Ejemplar.Libro.Nombre)
                    .Select(g => new { Libro = g.Key, Cantidad = g.Count() })
                    .ToList();

                // Construir la lista en el formato deseado
                var resultado = prestamosPorLibro
                    .Select(p => new { Libro = p.Libro, Cantidad = p.Cantidad })
                    .ToList();
                return Json(resultado);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error en la función PrestamosGrafica: {ex.Message}");
                return BadRequest(); // Retornar un código de error 400
            }
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
            try
            {
                Console.WriteLine(id);
                Prestamo prestamo = new Prestamo
                {
                    Id_peticion = id // este sera el id de la peticion la cual estamos aceptando
                };

                var peticion = await _prestamosServices.BuscarPeticion(id);
                if (peticion == null)
                {
                    Console.WriteLine("No se encontró la petición");
                    return NotFound(); // Retorna un código de error 404
                }

                switch (peticion.Estado)
                {
                    case "ACEPTADA":
                        Console.WriteLine("La petición ya ha sido aceptada");
                        return RedirectToAction("Index", "Peticiones");
                    case "RECHAZADA":
                        Console.WriteLine("La petición ya ha sido rechazada");
                        return RedirectToAction("Rechazadas", "Peticiones");
                }

                MensajeRespuestaValidacionPermiso(await _prestamosServices.Registrar(prestamo, User));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error en la función Created: {ex.Message}");
                return BadRequest(); // Retornar un código de error 400
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Id_peticion,Fecha_inicio,Fecha_fin,Estado")] Prestamo prestamo)
        {
            try
            {
                Console.WriteLine("estamos en create");
                Console.WriteLine(prestamo.Fecha_inicio);
                var resultado = await _prestamosServices.Registrar(prestamo, User);
                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error en la función Create: {ex.Message}");
                return BadRequest(); // Retornar un código de error 400
            }
            
        }

        private void MensajeRespuestaValidacionPermiso(ResponseModel resultado){
            
            TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            
        }   
       // GET: Prestamos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if (id == null)
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
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error en la función Edit (GET): {ex.Message}");
                return BadRequest(); // Retornar un código de error 400
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Id_peticion,Fecha_inicio,Fecha_fin,Estado")] Prestamo prestamo)
        {
            try
            {
                if (id != prestamo.Id)
                {
                    return NotFound();
                }

                // Inicializar el objeto resultado para almacenar el mensaje que se mostrará en la alerta
                ResponseModel resultado = new ResponseModel();

                // Buscar el prestamo actual al que pertenece el prestamo a editar
                var PrestamoExistente = await _prestamosServices.Buscar(prestamo.Id);

                if (PrestamoExistente != null)
                {
                    if (PrestamoExistente.Estado == "Devuelto")
                    {
                        resultado.Mensaje = "No puedes renovar un préstamo que ya ha sido finalizado";
                        resultado.Icono = "error";
                    }
                    else
                    {
                        Console.WriteLine("El prestamo está en curso");
                        
                        if (PrestamoExistente.Fecha_fin >= prestamo.Fecha_fin)
                        {
                            resultado.Mensaje = "No puedes poner la fecha final del préstamo antes de la actual registrada.";
                            resultado.Icono = "error";
                            TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                            return RedirectToAction(nameof(Index));
                        }

                        resultado = await _prestamosServices.Editar(PrestamoExistente, User, prestamo.Fecha_fin);
                    }
                }
                else
                {
                    Console.WriteLine("No encontró el préstamo sabiendo que es el mismo id");
                }

                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error en la función Edit (POST): {ex.Message}");
                return BadRequest(); // Retornar un código de error 400
            }
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
