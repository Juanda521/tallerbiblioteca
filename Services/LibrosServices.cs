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
        public LibrosServices(BibliotecaDbContext bibliotecaDbContext,ConfiguracionServices configuracionServices)
        {

            _context = bibliotecaDbContext;
            _configuracionServices = configuracionServices;
        }

        public async Task<int> Registrar(Libro libro,ClaimsPrincipal User)
        {

            Status = _configuracionServices.ValidacionConfiguracionActiva("Registrar_Libro",_configuracionServices.ObtenerRolUserOnline(User));
            Console.WriteLine("estado de la validacion");
            Console.WriteLine(Status);
            if(Status==200){
                _context.Add(libro);
                await _context.SaveChangesAsync();
               Console.WriteLine("ACA DEBERIA AGREGARLO");
            }else{
                Console.WriteLine("no debio haber guardado");
            }
            return Status;

        } 

        public async Task<int> CambiarEstado(Libro libro,ClaimsPrincipal user)
        {
            Console.WriteLine($"asi esta llegando el estado del libro {libro.Estado}");
            if (libro.Estado == "ACTIVO")
            {
                libro.Estado = "INACTIVO";
            }else if(libro.Estado == "INACTIVO")
            {
                libro.Estado = "ACTIVO";
            }else{
                Console.WriteLine("no tiene ninguno de los dos?");
            }
            Console.WriteLine($"asi termina el estado del libro {libro.Estado}");
            return await Editar(libro,user);

        }


        public async Task<int> Editar(Libro libro,ClaimsPrincipal User)
        {

            Status = _configuracionServices.ValidacionConfiguracionActiva("Editar_Libro",_configuracionServices.ObtenerRolUserOnline(User));
            Console.WriteLine("estado de la validacion");
            Console.WriteLine(Status);
            if(Status==200){
                 _context.Update(libro);
                await _context.SaveChangesAsync();
               Console.WriteLine("Aca estamos editando desde los servicios");
            }else{
                Console.WriteLine("no debio haber editado debido a los permisos");
            }
            return Status;

        } 



        public Libro  ConvertirImagen(byte[] LibroImagen,Libro libro,IFormFile ? ImagenLibro){
            using (var binaryReader = new BinaryReader(ImagenLibro.OpenReadStream()))
                {
                    LibroImagen=binaryReader.ReadBytes((int)ImagenLibro.Length);
                }
                 libro.ImagenLibro=Convert.ToBase64String(LibroImagen);
            return libro;
        }

        public ResponseModel MensajeRespuestaValidacionPermiso(int status){
            return _configuracionServices.MensajeRespuestaValidacionPermiso(status);
        }

       

        public async Task<List<Libro>>ObtenerLibros(){
            return await _context.Libros.ToListAsync();
        }

        public List<Libro>busqueda(string busqueda ){

            var libros = _context.Libros.ToList();

          
            libros = libros.Where(l=>l.Nombre.ToLower().Contains(busqueda) || l.Id.ToString().Contains(busqueda) || l.CantidadLibros.ToString().Contains(busqueda)).ToList();
            
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

        public async Task<List<Ejemplar>> RelacionarEjemplares  (int id){
            var EjemplaresRelacionados = await _context.Ejemplares.Where(e=>e.Id_libro  == id).ToListAsync();
            return EjemplaresRelacionados;
        }

       
        
    }
}