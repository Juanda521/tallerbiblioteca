using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;

//uksl uqxi npnx lyde
namespace tallerbiblioteca.Controllers
{
     [Authorize] //anotacion para especificar que debe estar aunthenticado ya el usuario (logeado)
    public class EjemplarController : Controller
    {
        private readonly EjemplarServices _ejemplarServices;

          private LibrosServices _librosServices;

        public EjemplarController(EjemplarServices ejemplarServices,LibrosServices librosServices)
        {
            _ejemplarServices = ejemplarServices;
            _librosServices = librosServices;
        }

        public async Task<IActionResult> Index(string busqueda,int pagina = 1, int itemsPagina  = 8)
        {
            var ejemplares  = await _ejemplarServices.ObtenerEjemplares();
            if (busqueda != null){
                busqueda = busqueda.ToLower();
                ejemplares = ejemplares.Where (e=>e.Id.ToString().Contains(busqueda)  || e.Libro.Nombre.ToLower().Contains(busqueda)  || e.Isbn_libro.ToString().Contains(busqueda ) || e.EstadoEjemplar.ToString().Contains(busqueda )).ToList();
            }
            var totalEjemplares = ejemplares.Count;
            var ejemplaresPaginados = ejemplares.Skip((pagina - 1) * itemsPagina).Take(itemsPagina).ToList();
            Paginacion<Ejemplar> paginacion  = new(ejemplaresPaginados,totalEjemplares,pagina,itemsPagina);
             ViewData["libros"] = new SelectList(await _librosServices.ObtenerLibros(),"Id","Nombre");
            return View(paginacion);
                            
        }

        public async Task<IActionResult> Create()
        {
            ViewData["libros"] = new SelectList(await _librosServices.ObtenerLibros(),"Id","Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Id_libro,Isbn_libro,EstadoEjemplar")] Ejemplar ejemplar)
        {
            Console.WriteLine("aca debe escribir el ejemplar");
            Console.WriteLine($"esta llegando esto desde el formulario estado {ejemplar.EstadoEjemplar}");
            MensajeRespuestaValidacionPermiso( await _ejemplarServices.Registrar(ejemplar,User));
            return RedirectToAction("Index","Libros");
        } 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>CreateFromLibros(){
            
            Console.WriteLine("hablalo desde registrar ejempla desde la vista de libros");
                string id_libro = Request.Form["Id_libro"];
                string isbn_ejemplar = Request.Form["Isbn_ejemplar"];
                Console.WriteLine("aca deberia copier el id del libro: {0} ", id_libro);
                 Console.WriteLine("aca deberia copier el isbn del libro: {0} ", isbn_ejemplar);

            if (int.TryParse(id_libro, out int idLibroInt)){
                Console.WriteLine("id del ejemplar a registrar: {0}", idLibroInt);
            }else{
                Console.WriteLine("no esta parseando el ejemplar");
                return RedirectToAction("Index","Libros");
            }

           Ejemplar ejemplar = new();
           ejemplar.Id_libro = idLibroInt;
           ejemplar.Isbn_libro = isbn_ejemplar;
           ejemplar.EstadoEjemplar = "DISPONIBLE";
               
            Console.WriteLine($"ya va empezar a realizar los servicios");
            MensajeRespuestaValidacionPermiso( await _ejemplarServices.Registrar(ejemplar,User));
            return RedirectToAction("Index","Libros");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id,string Estado)
        {
            var ejemplar = await _ejemplarServices.BuscarEjemplar(Id);
            if(ejemplar!=null){
                ejemplar.EstadoEjemplar  = Estado;
                MensajeRespuestaValidacionPermiso(await _ejemplarServices.Edit(User,ejemplar));
            }else{
                Console.WriteLine("no se esta encontrando ejemplar con el id:"+Id);
            }
            return RedirectToAction(nameof(Index));
       
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id){
            Console.WriteLine("id:liminar",Id);
            MensajeRespuestaValidacionPermiso(await _ejemplarServices.Delete(Id,User));
            return RedirectToAction(nameof(Index));
            
        }
         private void MensajeRespuestaValidacionPermiso(int status)         
        {
            Console.WriteLine(status);
           var resultado = new ResponseModel();

            if (status == 200)
            {       
                resultado.Mensaje  =  "La accion se ha realizado con exito";
                resultado.Icono  = "success";
                // TempData["Mensaje"] = "La accion se ha realizado con exito";
                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            }
            else if (status == 401)
            {  //si el permiso no lo puede realizar el usuario debido a que su rol no le permite realizar la accion ( status 401)
                resultado.Mensaje  =  "No tienes permiso para realizar esta accion";
                resultado.Icono  = "error";
                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            }
            else if (status == 402)
            {
                resultado.Mensaje  = "El permiso para realizar esta accion no se encuentra activo";
                resultado.Icono  = "info";
                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            }
            else
            {
                Console.WriteLine("i'm failing in the name of permission");
            }
            //return (string)TempData["Mensaje"];
        }

        

    }
}
