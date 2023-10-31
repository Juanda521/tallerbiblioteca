using tallerbiblioteca.Models;
namespace tallerbiblioteca.Models{

public class PeticionesViewModel{
    public Prestamo Prestamo {get; set;}  = new();

    public List<Peticiones> Peticiones {get; set;} = new();
}
}
