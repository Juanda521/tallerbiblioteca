using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;

namespace tallerbiblioteca.Controllers
{
    public class LibrosController : Controller
    {
        private readonly BibliotecaDbContext _context;
        private readonly IWebHostEnvironment _hostingEnviroment;

        public LibrosController(BibliotecaDbContext context,IWebHostEnvironment hostingEnviroment)
        {
            _context = context;
            _hostingEnviroment= hostingEnviroment;
        }

        public async Task<IActionResult> Index(string busqueda,int pagina = 1, int itemsPagina  = 6)
        {

            LibroViewModel libroViewModel = new()
            {
                Libros = await _context.Libros.ToListAsync()
            };
            //   var libro = await _context.Libros.ToListAsync();
            if (busqueda!=null)
            {
                busqueda.ToLower();
                libroViewModel.Libros=_context.Libros.Where(l=>l.Nombre.ToLower().Contains(busqueda)).ToList();
            }
            int totalLibros = libroViewModel.Libros.Count;
<<<<<<< Updated upstream
=======

            int total  = (totalLibros/itemsPagina)+1;
            

            foreach (var item in libroViewModel.Libros)
            {
                item.Generos =  await _librosServices.RelacionarGeneros(item.Id);
                item.Autores = await _librosServices.RelacionarAutores(item.Id);
                item.Ejemplares = await _librosServices.RelacionarEjemplares(item.Id);
            }

>>>>>>> Stashed changes
      
            var LibrosPaginados = libroViewModel.Libros.Skip((pagina - 1) * itemsPagina).Take(itemsPagina).ToList();

        
            Paginacion<Libro> paginacion = new Paginacion<Libro>(LibrosPaginados, total, pagina, itemsPagina)
            {
                LibroViewModel = libroViewModel
            };


            return View(paginacion);
        }



        public async Task<IActionResult> Catalog(int pagina  = 1,int itemsPagina  = 8)
        {   
          
            var libros = await _context.Libros.ToListAsync();

            var Ejemplares  = await _context.Ejemplares.ToListAsync();

            var autorLibro  = await _context.AutoresLibros.Include(a=>a.Autor).ToListAsync();

            var generoLibro  = await _context.GenerosLibros.Include(a=>a.Genero).ToListAsync();
        
            CatalogoViewModel catalogoViewModel = new()
            {
                Ejemplares = Ejemplares,
                Libros = libros,
                Autores =  autorLibro,
                Generos = generoLibro
              
            };

            var cantidadLibros = catalogoViewModel.Libros.Count;

            var catalogoPaginado = catalogoViewModel.Libros.Skip((pagina-1)*itemsPagina).Take(itemsPagina).ToList();

            foreach (var item in catalogoPaginado)
            {
                Console.WriteLine(item);
            }

            Paginacion<Libro> paginacionCatalogo = new Paginacion<Libro>(catalogoPaginado,cantidadLibros,pagina,itemsPagina){
                CatalogoViewModel  = catalogoViewModel
            };
            return View(paginacionCatalogo);
        }

        public List<Libro> LibrosRelacionadosPorGenero(int idLibro)
        {
            var generosDelLibroActual = _context.GenerosLibros
                .Where(gl => gl.Id_libro == idLibro)
                .Select(gl => gl.Id_genero)
                .ToList();

            var librosRelacionados = _context.GenerosLibros
                .Where(gl => generosDelLibroActual.Contains(gl.Id_genero) && gl.Id_libro != idLibro)
                .Select(gl => gl.Libro)
                .Distinct()
                .ToList();

                Console.WriteLine("hola");
                return librosRelacionados;
        }

        // GET: Libros/Create
        public IActionResult Create()
        {

            
            ViewData["Generos"]  = new SelectList(_context.Genero,"Id","NombreGenero");
            ViewData["Autores"]  = new SelectList(_context.Autores,"Id","NombreAutor");
            return View();
        }

        // POST: Libros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Cantidad,Descripcion,AutorIds,GeneroIds,ImagenLibro")] Libro libro,IFormFile ? ImagenLibro)
        {
            byte[] LibroImagen= new byte[0];
            if(ImagenLibro !=null  && ImagenLibro.Length >0 )
            {
                using (var binaryReader = new BinaryReader(ImagenLibro.OpenReadStream()))
                {
                    LibroImagen=binaryReader.ReadBytes((int)ImagenLibro.Length);
                }
                libro.ImagenLibro=Convert.ToBase64String(LibroImagen);

            }else{
                string rutaAlterna= Path.Combine(_hostingEnviroment.WebRootPath,"Images","fotopredeterminada.png");
                LibroImagen= System.IO.File.ReadAllBytes(rutaAlterna);
                libro.ImagenLibro= Convert.ToBase64String(LibroImagen);
            }
            _context.Add(libro);
            await _context.SaveChangesAsync();
            if (libro.GeneroIds!= null){
                foreach (var generoId in libro.GeneroIds)
                {
                    var genero = _context.Genero.Find(generoId);
                    if (genero != null)
                    {
                        var GeneroLibro = new GeneroLibro
                        {
                            Id_genero = genero.Id,
                            Genero = genero,
                            Libro = libro,
                            Id_libro = libro.Id
                        };

                        _context.GenerosLibros.Add(GeneroLibro);

                    }
                }
            }
            if (libro.AutorIds != null) {
            foreach (var autorId in libro.AutorIds)
            {
                var autor = _context.Autores.Find(autorId);
                if (autor != null)
                {
                    var autorLibro = new AutorLibro
                    {
                        Id_autor = autor.Id,
                        Autor = autor,
                        Libro  = libro,
                        Id_libro = libro.Id
                    };

                    _context.AutoresLibros.Add(autorLibro);
                }
            }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
             
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,CantidadLibros,Descripcion")] Libro libro)
        {
            if (id != libro.Id)
            {
                return NotFound();
            }
            try
            {
<<<<<<< Updated upstream
                _context.Update(libro);
                await _context.SaveChangesAsync();
=======
                Console.WriteLine("hablalo");
                byte[] LibroImagen= new byte[0];
                if(ImagenLibro !=null  && ImagenLibro.Length >0 )
                {
                    libro = _librosServices.ConvertirImagen(LibroImagen,libro,ImagenLibro);
                }else{
                    libro.ImagenLibro = await _librosServices.CambiarImagen(libro);
                    Console.WriteLine("va actualizar sin imagen");
                }
               MensajeRespuestaValidacionPermiso(_librosServices.MensajeRespuestaValidacionPermiso(await _librosServices.Editar(libro, User)));
>>>>>>> Stashed changes
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibroExists(libro.Id))
                {
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

