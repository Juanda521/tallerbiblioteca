
namespace tallerbiblioteca.Models{


    public class Pginacion<T>{
        //items con los que va trabajar, ya sea usuarios, libros, prestamos etc...
        public List<T> Items { get; }
        //cantidad total de registros en la base de datos de la tabla a mostrar
        public int TotalItems { get; }
        public int Numero_pagina { get; }
        //cantidad de items que van aparecer en la tabla
        public int ItemsPagina { get; }

        public Pginacion(List<T> items, int totalItems, int numero_pagina, int itemsPagina)
        {
            Items  = items;
            TotalItems = totalItems;
            Numero_pagina = numero_pagina;
            ItemsPagina = itemsPagina;
        }
    }
}