
namespace tallerbiblioteca.Models;
public class LibroViewModel
{

    public List<Libro> Libros {get; set;} 
    public Libro Libro {get; set;}  =new Libro();
    public List<Ejemplar> Ejemplares {get; set;}
    public Ejemplar Ejemplar {get; set;} = new();
  


}