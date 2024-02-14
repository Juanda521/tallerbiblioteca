﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using Newtonsoft.Json;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;
using Microsoft.AspNetCore.Authorization;

namespace tallerbiblioteca.Controllers
{
    [Authorize]
    public class LibrosController : Controller
    {
        private readonly BibliotecaDbContext _context;
        private readonly IWebHostEnvironment _hostingEnviroment;
        private LibrosServices _librosServices;
        private EjemplarServices _ejemplarServices;

        public LibrosController(BibliotecaDbContext context,IWebHostEnvironment hostingEnviroment,LibrosServices librosServices,EjemplarServices ejemplarServices)
        {
            _context = context;
            _hostingEnviroment= hostingEnviroment;
            _librosServices = librosServices;
            _ejemplarServices  = ejemplarServices;
        }


        public async Task<IActionResult> Index(string busqueda, int pagina = 1, int itemsPagina = 6)
        {
            try
            {
                LibroViewModel libroViewModel = new()
                {
                    Libros = await _librosServices.ObtenerLibros()
                };

                if (!string.IsNullOrEmpty(busqueda))
                {
                    busqueda.ToLower();
                    libroViewModel.Libros = _librosServices.busqueda(busqueda);
                }

                int totalLibros = libroViewModel.Libros.Count;
                int total = (totalLibros / itemsPagina) + 1;

                foreach (var item in libroViewModel.Libros)
                {
                    item.Generos = await _librosServices.RelacionarGeneros(item.Id);
                    item.Autores = await _librosServices.RelacionarAutores(item.Id);
                    item.Ejemplares = await _librosServices.RelacionarEjemplares(item.Id);
                }

                var LibrosPaginados = libroViewModel.Libros.Skip((pagina - 1) * itemsPagina).Take(itemsPagina).ToList();

                Paginacion<Libro> paginacion = new Paginacion<Libro>(LibrosPaginados, total, pagina, itemsPagina)
                {
                    LibroViewModel = libroViewModel
                };

                return View(paginacion);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error en el método Index: {ex.Message}");
                // Puedes redirigir a una página de error o devolver una vista de error
                return View("Error");
            }
        }


       
       
       [AllowAnonymous]
        public async Task<IActionResult> Catalog(string busqueda, string[] generosSeleccionados, string[] autoresSeleccionados, int pagina = 1, int itemsPagina = 8)
        {   
            try
            {
                var libros = await _librosServices.ObtenerLibros();

                // Filtrar por búsqueda
                if (!string.IsNullOrEmpty(busqueda))
                {
                    busqueda.ToLower();
                    libros = _librosServices.busqueda(busqueda);
                }

                // Filtrar por géneros seleccionados
                if (generosSeleccionados != null && generosSeleccionados.Length > 0)
                {
                    var generoIds = generosSeleccionados.Select(int.Parse);
                    libros = libros.Where(libro => libro.Generos.Any(genero => generoIds.Contains(genero.Id))).ToList();
                }

                // Filtrar por autores seleccionados
                if (autoresSeleccionados != null && autoresSeleccionados.Length > 0)
                {
                    var autorIds = autoresSeleccionados.Select(int.Parse);
                    libros = libros.Where(libro => libro.Autores.Any(autor => autorIds.Contains(autor.Id))).ToList();
                }

                CatalogoViewModel catalogoViewModel = new()
                {
                    Ejemplares = await _ejemplarServices.ObtenerEjemplares(),
                    Libros = libros,
                    AutoresRelacionados =  await _librosServices.ObtenerAutoresRelacionados(),
                    GenerosRelacionados = await _librosServices.ObtenerGenerosRelacionados(),
                    Generos = await _librosServices.ObtenerGeneros(),
                    Autores = await _librosServices.ObtenerAutores(),
                };

                int totalLibros = catalogoViewModel.Libros.Count;
                int totalPaginas = (int)Math.Ceiling((double)totalLibros / itemsPagina);

                var catalogoPaginado = catalogoViewModel.Libros.Skip((pagina - 1) * itemsPagina).Take(itemsPagina).ToList();

                Paginacion<Libro> paginacionCatalogo = new(catalogoPaginado, totalPaginas, pagina, itemsPagina)
                {
                    CatalogoViewModel = catalogoViewModel
                };

                return View(paginacionCatalogo);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error en el método Catalog: {ex.Message}");
                // Puedes redirigir a una página de error o devolver una vista de error
                return View("Error");
            }
        }

        [AllowAnonymous]
        public IActionResult LibrosRelacionadosPorGenero(int idLibro)
        {
            try
            {
                var librosRelacionados = _librosServices.ObtenerLibrosRelacionadosPorGenero(idLibro);
                return Ok(librosRelacionados);
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error al obtener los libros relacionados por género desde el controlador: {ex.Message}");
                return StatusCode(500, "Error interno del servidor"); // O devuelve otro resultado de acuerdo a tus necesidades
            }
        }

        // GET: Libros/Create
        public IActionResult Create()
        {

            ViewData["Generos"]  = new SelectList(_context.Genero,"Id","NombreGenero");
            ViewData["Autores"]  = new SelectList(_context.Autores,"Id","NombreAutor");
            return View();
        }

       

        private async Task RegistrarGenerosYAutores(Libro libro)
        {
            Console.WriteLine("Registrando géneros y autores del libro...");

            if (libro.GeneroIds != null)
            {
                foreach (var generoId in libro.GeneroIds)
                {
                    var genero = await _context.Genero.FindAsync(generoId);
                    if (genero != null)
                    {
                        var generoLibro = new GeneroLibro
                        {
                            Id_genero = genero.Id,
                            Genero = genero,
                            Libro = libro,
                            Id_libro = libro.Id
                        };
                        _context.GenerosLibros.Add(generoLibro);
                    }
                }
            }

            if (libro.AutorIds != null)
            {
                foreach (var autorId in libro.AutorIds)
                {
                    var autor = await _context.Autores.FindAsync(autorId);
                    if (autor != null)
                    {
                        var autorLibro = new AutorLibro
                        {
                            Id_autor = autor.Id,
                            Autor = autor,
                            Libro = libro,
                            Id_libro = libro.Id
                        };
                        _context.AutoresLibros.Add(autorLibro);
                    }
                }
            }
        }

        // POST: Libros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Cantidad,Descripcion,AutorIds,GeneroIds,ImagenLibro")] Libro libro, IFormFile? ImagenLibro)
        {
            try
            {
                Console.WriteLine("Registrando libro...");

                 Console.WriteLine("------------------------------------------------------");
                byte[] LibroImagen = new byte[0];
                if (ImagenLibro != null && ImagenLibro.Length > 0)
                {
                    libro = _librosServices.ConvertirImagen(LibroImagen, libro, ImagenLibro);
                }
                else
                {
                    string rutaAlterna = Path.Combine(_hostingEnviroment.WebRootPath, "Images", "default.jpg");
                    LibroImagen = System.IO.File.ReadAllBytes(rutaAlterna);
                    libro.ImagenLibro = Convert.ToBase64String(LibroImagen);
                }
                Console.WriteLine("vamos a hacer la funcion registrar");
                int status = await _librosServices.Registrar(libro, User);

                if (status == 200)
                {
                    // Registrar los géneros y autores del libro
                    await RegistrarGenerosYAutores(libro);
                    
                    // Guardar los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    Console.WriteLine("Libro registrado exitosamente.");
                }
                else
                {
                    Console.WriteLine("No se pudo registrar el libro.");
                }

                // Mostrar mensaje de respuesta
                MensajeRespuestaValidacionPermiso(_librosServices.MensajeRespuestaValidacionPermiso(status));

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Manejo de la excepción
                Console.WriteLine($"Ocurrió un error al crear el libro: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado()
        {
            try
            {
                string idLibro = Request.Form["libroId"];
                if (int.TryParse(idLibro, out int idLibroInt))
                {
                    Console.WriteLine("ID del libro a actualizar: {0}", idLibroInt);

                    var libro = await _librosServices.BuscarLibroAsync(idLibroInt);
                    if (libro != null)
                    {
                        var status = await _librosServices.CambiarEstado(libro, User);
                        MensajeRespuestaValidacionPermiso(_librosServices.MensajeRespuestaValidacionPermiso(status));
                    }
                    else
                    {
                        Console.WriteLine("El libro no fue encontrado.");
                    }
                }
                else
                {
                    Console.WriteLine("No se pudo analizar el ID del libro.");
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Ocurrió un error al cambiar el estado del libro: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }

            return RedirectToAction("Index", "Libros");
        }


        private void MensajeRespuestaValidacionPermiso(ResponseModel resultado){
            
            TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            
        }    

        // GET: Libros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Libros == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        // POST: Libros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,CantidadLibros,Descripcion")] Libro libro,IFormFile ? ImagenLibro)
        {
            Console.WriteLine("estamos en editar");
            if (id != libro.Id)
            {
                Console.WriteLine("no esta coincidiendo el id");
                return NotFound();
            }
            try
            {
                Console.WriteLine("hablalo");
                byte[] LibroImagen= new byte[0];
                if(ImagenLibro !=null  && ImagenLibro.Length >0 )
                {
                    
                    libro = _librosServices.ConvertirImagen(LibroImagen,libro,ImagenLibro);
                }else{
                    libro.ImagenLibro = await _librosServices.DevolverImagen(libro.Id);
                    Console.WriteLine("va actualizar sin imagen");
                }
               MensajeRespuestaValidacionPermiso(_librosServices.MensajeRespuestaValidacionPermiso(await _librosServices.Editar(libro, User)));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibroExists(libro.Id))
                {
                    Console.WriteLine("estamos aca");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            
            // return View(libro);
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Libros == null)
            {
                return NotFound();
            }

            var libro = await _context.Libros
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Libros == null)
            {
                return Problem("Entity set 'BibliotecaDbContext.Libros'  is null.");
            }
            var libro = await _context.Libros.FindAsync(id);
            if (libro != null)
            {
            
                _context.Libros.Remove(libro);

            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibroExists(int id)
        {
          return (_context.Libros?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

