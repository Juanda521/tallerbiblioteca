
namespace tallerbiblioteca.Models;
public class CatalogoViewModel
{

    public List<Libro> Libros {get; set;}

    public Usuario Usuario {get; set;}  = new();

    public List<Ejemplar> Ejemplares {get; set;}

    public Peticiones Peticiones {get; set;}  = new();

    public List<AutorLibro> Autores  {get; set;} = new();

    public List<GeneroLibro> Generos  {get; set;}  = new();

   




}