using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using tallerbiblioteca.Context;
using tallerbiblioteca.Migrations;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;

namespace tallerbiblioteca.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {

        private readonly BibliotecaDbContext _context;
        private readonly ReservasServices _ReservasServices;


        public ReservasController(BibliotecaDbContext context, ReservasServices reservasServices)
        {
            _context = context;
            _ReservasServices = reservasServices;
        }
        private void MensajeRespuestaValidacionPermiso(ResponseModel resultado)
        {

            TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
        }

        // GET: Reserva
        public async Task<IActionResult> Index(string? buscar, DateTime? fecha, int pagina = 1, int itemsPagina = 5, int? id = null)
        {
            var reservas = await _ReservasServices.ObtenerReservas();
            if (id != null)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (User.IsInRole("2") && userIdClaim != null)
                {
                    int userId = Convert.ToInt32(userIdClaim.Value);
                    reservas = await _ReservasServices.BuscarR(userId);
                }
            }
            if (buscar != null)
            {
                reservas = await _ReservasServices.Buscar(buscar);
            }
            else if (fecha.HasValue)
            {
                reservas = await _ReservasServices.Buscarporfecha(fecha.Value.Date);
            }
            int totalItems = reservas.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalItems / itemsPagina);

            int indiceInicio = (pagina - 1) * itemsPagina;

            var reservacionesPaginadas = reservas
                .Skip(indiceInicio)
                .Take(itemsPagina)
                .ToList();


            var paginacion = new Paginacion<Reserva>(reservacionesPaginadas, totalPaginas, pagina, itemsPagina);

            return View(paginacion);
        }
        public async Task<IActionResult> Create()
        {
            var ejemplares = await _ReservasServices.buscarEj();
            var usuarios = await _ReservasServices.buscarUs();

            var ejemplaresSelect = new SelectList(ejemplares, "Id", "Libro.Nombre");
            var usuariosSelectList = new SelectList(usuarios, "Id", "Name");
            ViewBag.Usuarios = usuariosSelectList;
            ViewBag.ejemplares = ejemplaresSelect;

            return View();
        }


        // POST: Reserva/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva)
        {
            bool buscreserva = await _ReservasServices.Encontrarreserva(reserva.IdUsuario);
            if (buscreserva)
            {
                TempData["ReservaPendiente"] = "True";
                return RedirectToAction("Catalog", "Libros");
            }
            MensajeRespuestaValidacionPermiso(await _ReservasServices.Crear(reserva, User));
            return RedirectToAction(nameof(Index));


        }

        // GET: Reserva/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var reserva = await _ReservasServices.enviarR(id);
            return View(reserva);
        }

        // POST: Reserva/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Eliminar = await _ReservasServices.Cambiarestado(id, User);
            MensajeRespuestaValidacionPermiso(await _ReservasServices.Cambiarestado(id, User));
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Rechazadas(int pagina = 1, int itemsPagina = 5)
        {
            var eliminadas = await _ReservasServices.Rechazadas();
            int totalItems = eliminadas.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalItems / itemsPagina);

            int indiceInicio = (pagina - 1) * itemsPagina;

            var reservacionesPaginadas = eliminadas
                .Skip(indiceInicio)
                .Take(itemsPagina)
                .ToList();
            var paginacion = new Paginacion<Reserva>(reservacionesPaginadas, totalPaginas, pagina, itemsPagina);

            return View(paginacion);

        }
    }
}
