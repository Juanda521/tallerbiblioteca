using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using System.Data;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;
using Microsoft.AspNetCore.Mvc;
using tallerbiblioteca.Migrations;
using Newtonsoft.Json;

namespace tallerbiblioteca.Controllers
{

    public class PublicacionesController : Controller
    {

        private readonly BibliotecaDbContext _context;

        private readonly PublicacionesServices _PublicacionesServices;
        private ConfiguracionServices _configuracionServices;
        public PublicacionesController(BibliotecaDbContext context, PublicacionesServices publicacionesservices, ConfiguracionServices configuracionServices)
        {
            _context = context;
            _PublicacionesServices = publicacionesservices;
            _configuracionServices = configuracionServices;
        }
        // GET: Publicaciones
        private void MensajeRespuestaValidacionPermiso(ResponseModel resultado)
        {

            TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
        }
        public async Task<IActionResult> Index(string? busqueda, DateTime? fechaini, DateTime? fechafin, int pagina = 1, int itemsPagina = 3)
        {
            var publicaciones = await _PublicacionesServices.ObtenerPublicaciones();
            if (busqueda != null)
            {
                publicaciones = await _PublicacionesServices.Buscar(busqueda);
            }
            else if (fechafin != null || fechafin != null)
            {
                publicaciones = await _PublicacionesServices.buscarfecha(fechaini, fechafin);
            }
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


            var paginacion = new Paginacion<Models.Publicaciones>(publicacionesPaginadas, totalItems, pagina, itemsPagina);

            return View(paginacion);
        }
        // GET: Publicaciones/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo,Nombre,Descripcion,FechaInicio,FechaFin,Imagen,Estado")] IFormFile? Imagen, Models.Publicaciones publicaciones)
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
                MensajeRespuestaValidacionPermiso(await _PublicacionesServices.Crear(publicaciones, User));
                return RedirectToAction(nameof(Index));
            }
        }
        // GET: Publicaciones/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var publicacionesTask = _PublicacionesServices.ObtenerEditar(id);
                var publicaciones = await publicacionesTask;

                if (publicaciones == null)
                {

                    return NotFound();
                }

                return View(publicaciones);
            }
            catch
            {
                return RedirectToAction("Catalog", "Libros");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Nombre,Descripcion,FechaInicio,FechaFin")] Models.Publicaciones publicaciones)
        {
            try
            {
                MensajeRespuestaValidacionPermiso(await _PublicacionesServices.Editar(publicaciones, User));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Catalog", "Libros");
            }
        }
        // GET: Publicaciones/Delete/5
        public async Task<IActionResult> Delete(int id, int pagina = 1, int itemsPagina = 3)
        {
            var resultado = await _PublicacionesServices.enviarcambiar(id);
            int indiceInicio = (pagina - 1) * itemsPagina;
            int totalItems = resultado.Count;
     

            var publicacionesPaginadas = resultado.Skip(indiceInicio).Take(itemsPagina).ToList();


            var paginacion = new Paginacion<Models.Publicaciones>(publicacionesPaginadas, totalItems, pagina, itemsPagina);

            return View(paginacion);
        }
        // POST: Publicaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            MensajeRespuestaValidacionPermiso(await _PublicacionesServices.CambiarEstado(id, User));
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Desactivadas(int pagina = 1, int itemsPagina = 3)
        {
            var publicaciones = await _PublicacionesServices.Desactivadas();
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


            var paginacion = new Paginacion<Models.Publicaciones>(publicacionesPaginadas, totalItems, pagina, itemsPagina);

            return View(paginacion);
        }

    }


}

