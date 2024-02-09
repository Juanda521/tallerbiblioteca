using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;
using Microsoft.AspNetCore.Authorization;

namespace tallerbiblioteca.Controllers
{
    [Authorize]
    public class PublicacionesController : Controller
    {

        private readonly BibliotecaDbContext _context;

        private readonly PublicacionesServices _PublicacionesServices;

        public PublicacionesController(BibliotecaDbContext context, PublicacionesServices publicacionesservices)
        {
            _context = context;
            _PublicacionesServices = publicacionesservices;

        }

        // GET: Publicaciones


        public async Task<IActionResult> Index(int pagina = 1, int itemsPagina = 3)
        {
            var publicaciones = await _PublicacionesServices.ObtenerPublicaciones();
            publicaciones = publicaciones.OrderBy(p => p.FechaFin).ToList();
            
            int indiceInicio = (pagina - 1) * itemsPagina;
            int totalItems = publicaciones.Count;
            if (totalItems % 3 != 0)
            {
                totalItems = (totalItems / 3) + 1;
            }
            else
            {
                totalItems = (totalItems / 3);
            }

            var publicacionesPaginadas = publicaciones.Skip(indiceInicio).Take(itemsPagina).ToList();


            var paginacion = new PaginacionPubli<Publicaciones>(publicacionesPaginadas, totalItems, pagina, itemsPagina);

            return View(paginacion);
        }


        // GET: Publicaciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Publicaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo,Nombre,Descripcion,FechaInicio,FechaFin,Imagen,Estado")] IFormFile? Imagen, Publicaciones publicaciones)
        {
            var fechaini = publicaciones.FechaInicio;
            var fechafin = publicaciones.FechaFin;

            if (fechaini > fechafin)
            {
                ViewBag.FechaMayor = true;
                return View(publicaciones);
            }
            else
            {
                ViewData["Iniciar"] = "true";
                if (Imagen != null && Imagen.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Imagen.CopyTo(ms);
                        publicaciones.Imagen = ms.ToArray();
                    }
                }
                await _PublicacionesServices.Crear(publicaciones);
                   
                return RedirectToAction(nameof(Index));
            }
        }



        // GET: Publicaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var publicaciones = await _context.Publicaciones.FindAsync(id);
            if (publicaciones == null)
            {
                return NotFound();
            }
            return View(publicaciones);
        }

        // POST: Publicaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Nombre,Descripcion,FechaInicio,FechaFin")] Publicaciones publicaciones)
        {
            try
            {
                var result = await _PublicacionesServices.Editar(publicaciones);

                if (result)
                {
                    ViewData["CreadaConExito"] = "True";
                }
                else
                {
                    TempData["ErrorCrear"] = "True";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                
                TempData["ErrorCrear"] = "True";
                TempData.Keep();
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Publicaciones/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var resultado = await _PublicacionesServices.enviarcambiar(id);
            return View(resultado);
        }

        // POST: Publicaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cambiar = await _PublicacionesServices.CambiarEstado(id);
            if (cambiar == 200)
            {
                ViewData["Desactivado"] = "True";

            }else if(cambiar == 202)
            {

            }
            return RedirectToAction(nameof(Index));
        }

        private bool PublicacionesExists(int id)
        {
            return (_context.Publicaciones?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
    public class PaginacionPubli<Publicacion>
    {
        public List<Publicacion> Publicaciones { get; }
        public int TotalItems { get; }
        public int PageNumber { get; }
        public int PageSize { get; }

        public PaginacionPubli(List<Publicacion> publicaciones, int totalItems, int pageNumber, int pageSize)
        {
            Publicaciones = publicaciones;
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}

