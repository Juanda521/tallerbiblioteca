using System;
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

        public async Task<IActionResult> Index(string busqueda,int pagina = 1, int itemsPagina  = 4)
        {

            LibroViewModel libroViewModel = new()
            {
                Libros = await _librosServices.ObtenerLibros()
            };
            //   var libro = await _context.Libros.ToListAsync();
            if (busqueda!=null)
            {
                busqueda.ToLower();
                libroViewModel.Libros  = _librosServices.busqueda(busqueda);
            }
            int totalLibros = libroViewModel.Libros.Count;
            

            foreach (var item in libroViewModel.Libros)
            {
                item.Generos =  await _librosServices.RelacionarGeneros(item.Id);
                item.Autores = await _librosServices.RelacionarAutores(item.Id);
                item.Ejemplares = await _librosServices.RelacionarEjemplares(item.Id);
            }

      
            var LibrosPaginados = libroViewModel.Libros.Skip((pagina - 1) * itemsPagina).Take(itemsPagina).ToList();

        
            Paginacion<Libro> paginacion = new Paginacion<Libro>(LibrosPaginados, totalLibros, pagina, itemsPagina)
            {
                LibroViewModel = libroViewModel
            };


            return View(paginacion);
        }

       
        [AllowAnonymous]
        public async Task<IActionResult> Catalog(string busqueda,string[] generosSeleccionados,string[] autoresSeleccionados,int pagina  = 1,int itemsPagina  = 8)
        {   
         
            var libros = await _librosServices.ObtenerLibros();
            //si viene algo en el parametro busqueda procederemos a añadir a la lista de libros a mostrar los libros que coincidan con la busqueda
            if(busqueda!=null){
                busqueda.ToLower();
                libros = _librosServices.busqueda(busqueda);
            }

            if (generosSeleccionados!=null || generosSeleccionados.Length>0){
                Console.WriteLine("estan llegando los generos");
                foreach (var item in generosSeleccionados){
                    if (int.TryParse(item, out int generoId))
                    {
                        Console.WriteLine($"Género ID: {generoId}");
                        libros = await _librosServices.BusquedaporGeneros(generoId);
                    }
                    else
                    {
                        Console.WriteLine($"No se pudo convertir {item} a entero.");
                        // Puedes manejar el caso en el que la conversión no sea exitosa
                    }
                }
            }

            if (autoresSeleccionados!=null || autoresSeleccionados.Length>0){
                Console.WriteLine("estan llegando los autores");
                foreach (var item in autoresSeleccionados)
                {
                    if(int.TryParse(item,out int autorId)){
                        
                        libros = await  _librosServices.BusquedaporAutores(autorId);
                    }else{
                        Console.WriteLine($"No se pudo convertir {item} a entero.");
                    }
                }
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

            var cantidadLibros = catalogoViewModel.Libros.Count;
            var libross = (cantidadLibros) / 8; 

            var catalogoPaginado = catalogoViewModel.Libros.Skip((pagina-1)*itemsPagina).Take(itemsPagina).ToList();

            Paginacion<Libro> paginacionCatalogo = new Paginacion<Libro>(catalogoPaginado,libross,pagina,itemsPagina){
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
            Console.WriteLine("hola desde registrar");
            Console.WriteLine("------------------------------------------------------");
            byte[] LibroImagen= new byte[0];
            if(ImagenLibro !=null  && ImagenLibro.Length >0 )
            {
                libro = _librosServices.ConvertirImagen(LibroImagen,libro,ImagenLibro);
            }else{
                string rutaAlterna= Path.Combine(_hostingEnviroment.WebRootPath,"Images","default.jpg");
                LibroImagen= System.IO.File.ReadAllBytes(rutaAlterna);
                libro.ImagenLibro= Convert.ToBase64String(LibroImagen);
            }
            Console.WriteLine("vamos a hacer la funcion registrar");
            int status = await _librosServices.Registrar(libro,User);
           if (status == 200)
           {
             Console.WriteLine("vamos a registrar generos y autores");
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
            
             
           } else{
            Console.WriteLine("no debio haber hecho nada");
           }
           MensajeRespuestaValidacionPermiso(_librosServices.MensajeRespuestaValidacionPermiso(status));
           return RedirectToAction(nameof(Index));
         
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(){

            string idLibro = Request.Form["libroId"];
            if (int.TryParse(idLibro, out int idLibroInt)){
                Console.WriteLine("id del libro a actualizar: {0}", idLibroInt);
                
                MensajeRespuestaValidacionPermiso(_librosServices.MensajeRespuestaValidacionPermiso(await _librosServices.CambiarEstado(await _librosServices.BuscarLibroAsync(idLibroInt),User)));
                return RedirectToAction("Index","Libros");
            }else{
                Console.WriteLine("no esta parseando el libro");
                return RedirectToAction("Index","Libros");
            }
            // return RedirectToAction(nameof(index));
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

