// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using tallerbiblioteca.Context;
// using tallerbiblioteca.Models;
// using tallerbiblioteca.Services;

// namespace tallerbiblioteca.Controllers
// {
//     public class ReservasController : Controller
//     {
//         private readonly BibliotecaDbContext _context;
//         private readonly ReservasServices _ReservasServices;

//         public ReservasController(BibliotecaDbContext context, ReservasServices reservasServices)
//         {
//             _context = context;
//             _ReservasServices = reservasServices;
//         }

//         // GET: Reservas
//         public async Task<IActionResult> Index(int pagina = 1, int itemsPagina = 5)
//         {
//             var reservas = await _ReservasServices.ObtenerReservas();

//             int totalItems = reservas.Count;
//             int totalPaginas = (int)Math.Ceiling((double)totalItems / itemsPagina);

//             int indiceInicio = (pagina - 1) * itemsPagina;

//             var reservacionesPaginadas = reservas
//                 .Skip(indiceInicio)
//                 .Take(itemsPagina)
//                 .ToList();

            
//             var paginacion = new PagiReservas<Reserva>(reservacionesPaginadas, totalPaginas, pagina, itemsPagina);

//             return View(paginacion);
//         }
//         public async Task<IActionResult> Create()
//         {
//             var ejemplares = await _ReservasServices.buscarEj();
//             var usuarios = await _ReservasServices.buscarUs();

//             var ejemplaresSelect = new SelectList(ejemplares, "Id", "Libro");
//             var usuariosSelectList = new SelectList(usuarios, "Id", "Name");
//             ViewBag.Usuarios = usuariosSelectList;

//             return View();
//         }


//         // POST: Reservas/Create
//         // To protect from overposting attacks, enable the specific properties you want to bind to.
//         // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create(Reserva reserva)
//         {
//             bool buscreserva = await _ReservasServices.Encontrarreserva(reserva.IdUsuario);

//             if (buscreserva)
//             {
//                 TempData["ReservaPendiente"] = "True";
//                 return RedirectToAction("Catalog", "Libros");
//             }
//             else
//             {
//                 Console.WriteLine("ACABA DE LLEGAR", reserva.IdEjemplar);
//                 _ReservasServices.Crear(reserva.IdEjemplar, reserva.IdUsuario);
//                 return RedirectToAction(nameof(Index));
//             }
//         }

//     // GET: Reservas/Delete/5
//     public async Task<IActionResult> Delete(int id)
//         {
//             var reserva = await _ReservasServices.enviarR(id);
//             return View(reserva);
//         }

//         // POST: Reservas/Delete/5
//         [HttpPost, ActionName("Delete")]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> DeleteConfirmed(int id)
//         {
//             bool Eliminar = await _ReservasServices.Cambiarestado(id);
//             if(Eliminar == true)
//             {
//                 Console.WriteLine("ESTADO CAMBIADO CON EXITO");
//             }
//             else
//             {
//                 Console.WriteLine("No se pudo ");
//                 return View("Index");
//             }
//             return RedirectToAction(nameof(Index));
//         }

//         // private bool ReservaExists(int id)
//         // {
//         //     return (_context.Reserva?.Any(e => e.IdReserva == id)).GetValueOrDefault();
//         // }
//     }

//     public class PagiReservas<Reserva>
//     {
//         public List<Reserva> Reservas { get; }
//         public int TotalItems { get; }
//         public int PageNumber { get; }
//         public int PageSize { get; }

//         public PagiReservas(List<Reserva> publicaciones, int totalItems, int pageNumber, int pageSize)
//         {
//             Reservas = publicaciones;
//             TotalItems = totalItems;
//             PageNumber = pageNumber;
//             PageSize = pageSize;
//         }
//     }
// }
