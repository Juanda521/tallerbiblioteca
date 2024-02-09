
namespace tallerbiblioteca.Models;
public class CatalogoViewModel
{

    public List<Libro> Libros {get; set;}

    public Usuario Usuario {get; set;}  = new();

    public List<Ejemplar> Ejemplares {get; set;}

    public Peticiones Peticiones {get; set;} 

    public List<AutorLibro> AutoresRelacionados  {get; set;} = new();

    public List<GeneroLibro> GenerosRelacionados  {get; set;}  = new();

    public List<Genero> Generos {get; set;}  =  new();

    public List<Autor> Autores {get; set;} = new();

}