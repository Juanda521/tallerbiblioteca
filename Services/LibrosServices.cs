using tallerbiblioteca.Models;
using Microsoft.Build.Framework;
using tallerbiblioteca.Views.Usuarios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using System.Security.Claims;

namespace tallerbiblioteca.Services
{
    public class LibrosServices
    {
        private readonly BibliotecaDbContext _context;
        private int Status;
        private ConfiguracionServices _configuracionServices;
        public LibrosServices(BibliotecaDbContext bibliotecaDbContext)
        {

            _context = bibliotecaDbContext;
            _configuracionServices = new ConfiguracionServices(_context);
        }

        public async Task<bool> Registrar(Libro libro,ClaimsPrincipal User)
        {

            var Id_rolString = User.FindFirst(ClaimTypes.Role)?.ToString();
            if(Id_rolString!=null){
                int Id_rol_User = Int32.Parse(Id_rolString);
                if(_configuracionServices.ValidacionConfiguracionActiva("Registrar Libro",Id_rol_User)==200){
                    
                }Status = _configuracionServices.ValidacionConfiguracionActiva("Registrar Libro",Id_rol_User);
                _context.Add(libro);
                 return (await _context.SaveChangesAsync() > 0) ? true : false;
                
            }
           
            _context.Add(libro);
            return (await _context.SaveChangesAsync() > 0) ? true : false;
        } 

        public async Task<List<Libro>>ObtenerLibros(){
            return await _context.Libros.ToListAsync();
        }

<<<<<<< Updated upstream
=======
        public List<Libro>busqueda(string busqueda ){

            var libros = _context.Libros.ToList();

          
            libros = libros.Where(l=>l.Nombre.ToLower().Contains(busqueda) || l.Id.ToString().Contains(busqueda) || l.Estado.ToLower().Contains(busqueda) || l.CantidadLibros.ToString().Contains(busqueda)).ToList();
            
            // Console.WriteLine("la busqueda no esta relacionada con numeros");
             
            return libros;
        }

          public async Task<List<AutorLibro>>ObtenerAutoresRelacionados(){
            return await _context.AutoresLibros.Include(a=>a.Autor).ToListAsync();
        }

         public async Task<List<Autor>>ObtenerAutores(){
            return await _context.Autores.ToListAsync();
        }

         public async Task<List<GeneroLibro>>ObtenerGenerosRelacionados(){
            return await _context.GenerosLibros.Include(a=>a.Genero).ToListAsync();
        }

        public async Task<List<Genero>>ObtenerGeneros(){
            return await _context.Genero.ToListAsync();
        }

        
       
       //funcion para encontrar libros relacionados por generos
        public async Task<List<Libro>>BusquedaporGeneros(int id){

            //buscamos en la tabla GenerosLIbros los registros que coincidan el id del genero con el id que llega a la funcion, y ademas incluimos su respectivo genero y libro
            var generos =   await _context.GenerosLibros.Where(g=>g.Id_genero == id).Include(g=>g.Genero).Include(g=>g.Libro).ToListAsync();
            List<Libro> Libros  = new List<Libro>(); //creamos e inicializamos lista para guardar los libros relacionados
            foreach (var item in generos)
            {
                //añadimos a la lista el libro que le hemos incluido a cada item encontrado.
                Libros.Add(item.Libro);
            }
            return Libros; //retornamos ya la lista de libros para utilizarla en el controlador
           
        }

        public async Task<List<Libro>>BusquedaporAutores(int id){

            var autores =   await _context.AutoresLibros.Where(a=>a.Id_autor == id).Include(a=>a.Autor).Include(a=>a.Libro).ToListAsync();
            List<Libro> Libros  = new List<Libro>();
            foreach (var item in autores)
            {
                Libros.Add(item.Libro);
            }
            return Libros;
           
        }
        

        

        public async Task<Libro> BuscarLibroAsync(int id)
        {
            var libro = await _context.Libros.SingleOrDefaultAsync(p => p.Id == id);

            if (libro != null)
            {
            
                return libro;
            }
             
            return new Libro(); // Crear una nueva instancia de Libro si no se encuentra.
        }

        public async Task<List<Genero>> RelacionarGeneros  (int id){
             var GenerosRelacionados = await _context.GenerosLibros.Where(a=>a.Id_libro  == id).ToListAsync();
            int numeroLibros = GenerosRelacionados.Count;
           
            var generosMandar = new List<Genero>();
            foreach (var item in GenerosRelacionados)
            {
               
                var genero = await _context.Genero.FirstOrDefaultAsync(g => g.Id == item.Id_genero);

                if (genero != null)
                {
                    generosMandar.Add(genero);
                  
                };
            };
            return generosMandar;
        }

         public async Task<List<Autor>> RelacionarAutores  (int id){
             var AutoresRelacionados = await _context.AutoresLibros.Where(a=>a.Id_libro  == id).ToListAsync();
            int Numero_autores = AutoresRelacionados.Count;
           
            var autoresMandar = new List<Autor>();
            foreach (var item in AutoresRelacionados)
            {
               
                var autor = await _context.Autores.FirstOrDefaultAsync(a => a.Id == item.Id_autor);

                if (autor != null)
                {
                    autoresMandar.Add(autor);
                }
            };
            return autoresMandar;
        }

        public async Task<string> CambiarImagen(Libro libro){

            //el asnotracking() nos sirve para hacer consulta temporal y no salga error por la instancia doble de un objeto ya encontrado
            var libroExistente = await _context.Libros.AsNoTracking().FirstOrDefaultAsync(l => l.Id == libro.Id);

            libro.ImagenLibro = libroExistente.ImagenLibro;

            return libro.ImagenLibro;
        }

        public async Task<List<Ejemplar>> RelacionarEjemplares  (int id){
            var EjemplaresRelacionados = await _context.Ejemplares.Where(e=>e.Id_libro  == id).ToListAsync();
            return EjemplaresRelacionados;
        }

>>>>>>> Stashed changes
       
        
    }
}