using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;

namespace tallerbiblioteca.Controllers
{
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
        public async Task<IActionResult> Index()
        {

            return View(await _prestamosServices.ObtenerPrestamos());
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Id_peticion")] Prestamo prestamo)
        {
            MensajeRespuestaValidacionPermiso(await _prestamosServices.Registrar(prestamo,User));
            return RedirectToAction(nameof(Index));

        }

         private void MensajeRespuestaValidacionPermiso(ResponseModel resultado){
            
            TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            
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

//         // GET: Prestamos/Edit/5
//         public async Task<IActionResult> Edit(int? id)
//         {
//             if (id == null || _context.Prestamos == null)
//             {
//                 return NotFound();
//             }

//             var prestamo = await _context.Prestamos.FindAsync(id);
//             if (prestamo == null)
//             {
//                 return NotFound();
//             }
//             ViewData["Id_libro"] = new SelectList(_context.Libros, "Id", "Id", prestamo.Id_ejemplar);
//             ViewData["Id_usuario"] = new SelectList(_context.Usuarios, "Id", "Id", prestamo.Id_usuario);
//             return View(prestamo);
//         }

//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Edit(int id, [Bind("Id,Id_usuario,Id_libro,Fecha_inicio,Fecha_fin")] Prestamo prestamo)
//         {
//             if (id != prestamo.Id)
//             {
//                 return NotFound();
//             }
//                 try
//                 {
//                     _context.Update(prestamo);
//                     await _context.SaveChangesAsync();
//                 }
//                 catch (DbUpdateConcurrencyException)
//                 {
//                     if (!PrestamoExists(prestamo.Id))
//                     {
//                         return NotFound();
//                     }
//                     else
//                     {
//                         throw;
//                     }
//                 }
//                 return RedirectToAction(nameof(Index));
            
//             //ViewData["Id_libro"] = new SelectList(_context.Libros, "Id", "Id", prestamo.Id_libro);
//             //ViewData["Id_usuario"] = new SelectList(_context.Usuarios, "Id", "Id", prestamo.Id_usuario);
//             //return View(prestamo);
//         }

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
