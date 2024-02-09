using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using tallerbiblioteca.Context;
using tallerbiblioteca.Migrations;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;

namespace tallerbiblioteca.Controllers
{
    public class DevolucionesController : Controller
    {
        private readonly BibliotecaDbContext _context;
        private readonly DevolucionesServices _devolucionesServices;

        public DevolucionesController(BibliotecaDbContext context, DevolucionesServices devolucionesServices)
        {
            _context = context;
            _devolucionesServices = devolucionesServices;
        }

        // GET: Devoluciones
        // public async Task<IActionResult> Index(int busqueda)
        // {
        //     // return View(await _devolucionesServices.BarraBusqueda(busqueda));
        //     return View(await _devolucionesServices.ObtenerDevoluciones());
        // }

        public async Task<IActionResult> Index(string busqueda,int pagina = 1, int itemsPagina = 10)
        {


            var devolucion = await _devolucionesServices.ObtenerDevoluciones();


            if (busqueda!=null)
            {
                busqueda = busqueda.ToLower();
                if (int.TryParse(busqueda, out int Id))
                {
                    // devolucion = devolucion.Where(u => u.Observaciones.ToLower().Contains(busqueda) || u => u.Name.ToLower().Contains(busqueda)  || u.Id.ToString().Contains(busqueda)).ToList();
                    devolucion = devolucion.Where(u => u.Observaciones.ToLower().Contains(busqueda) || u.Prestamo.Peticion.Usuario.Name.ToLower().Contains(busqueda) || u.Id.ToString().Contains(busqueda)).ToList();

                }
                else
                {
                    devolucion = devolucion.Where(u => u.Observaciones.Contains(busqueda)).ToList();

                }
            }
            // Realiza la paginación
            int totalDevoluciones = devolucion.Count;
            var devolucionesPaginadas = devolucion.Skip((pagina - 1) * itemsPagina).Take(itemsPagina).ToList();
            // Crea el objeto de paginación
            Paginacion<Devolucion> paginacion = new Paginacion<Devolucion>(devolucionesPaginadas, totalDevoluciones, pagina, itemsPagina);

            return View(paginacion);
        }


        // GET: Devoluciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Devoluciones == null)
            {
                return NotFound();
            }

            var devolucion = await _context.Devoluciones
                .Include(d => d.Prestamo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (devolucion == null)
            {
                return NotFound();
            }

            return View(devolucion);
        }

        // GET: Devoluciones/Create
        public IActionResult Create()
        {
            ViewData["Id_prestamo"] = new SelectList(_context.Prestamos, "Id", "Id");
            return View();
        }

        private void MensajeRespuestaDevolucion(int status)
        {
            Console.WriteLine(status);
            var resultado = new ResponseModel();
            switch (status)
            {
                case 200:
                    resultado.Mensaje = "La accion se ha realizado con exito";
                    resultado.Icono = "success";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 401:
                    resultado.Mensaje = "El permiso para realizar esta accion no se encuentra activo";
                    resultado.Icono = "info";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 402:
                    resultado.Mensaje = "El permiso para realizar esta accion no se encuentra activo";
                    resultado.Icono = "info";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                    case 500:
                     resultado.Mensaje = "ya se ha registrado la devolucion de este prestamo";
                    resultado.Icono = "error";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                default:
                    Console.WriteLine("i'm failing in the name of permission");
                    break;
            }

        }

        // POST: Devoluciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("Id,Id_prestamo,Observaciones,Fecha_devolucion")] Devolucion devolucion)
        // {

        //     Console.WriteLine("hola desdde registrar devoluciones");
        //     Console.WriteLine($"id: {devolucion.Id}");
        //     Console.WriteLine($"id: {devolucion.Id_prestamo}");
        //     Console.WriteLine($"id{devolucion.Observaciones}");
        //     Console.WriteLine($"id{devolucion.Fecha_devolucion}");

        //     int status = await _devolucionesServices.Registrar(devolucion, User);

        //     MensajeRespuestaValidacionPermiso(status);

        //     return RedirectToAction(nameof(Index));
        // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(){
                string id_prestamo = Request.Form["id_prestamo"];
                string observacion = Request.Form["observaciones"];
                Console.WriteLine("aca deberia copier el id del prestamo: {0} ", id_prestamo);
                Console.WriteLine("aca deberia copiar la observacion que se le ha hecho al libro: {0} ", observacion);

            Devolucion devolucion = new()
            {
                Observaciones = observacion,
                Fecha_devolucion = _devolucionesServices.obtenerFechaActual()
            };
            Console.WriteLine(devolucion.Fecha_devolucion);
                

                if (int.TryParse(id_prestamo, out int idPrestamoInt)){
                      devolucion.Id_prestamo = idPrestamoInt;
                    Console.WriteLine("id del prestamo a registrar: {0}", idPrestamoInt);
                    Console.WriteLine("vamos a validar la devolcion existente con el prestamo");
                    var devolucionExistente = await _devolucionesServices.BuscarDevolucionExistente(idPrestamoInt);

                    if(devolucionExistente){
                        Console.WriteLine("ya el prestamo se ha devuelto");
                        MensajeRespuestaDevolucion(500);
                        return RedirectToAction("Index","Prestamos");
                    }else{
                        Console.WriteLine("no se encontraron devoluciones con ese prestamo");
                    }
                }

                MensajeRespuestaValidacionPermiso(await _devolucionesServices.Registrar(devolucion,User));
                return RedirectToAction(nameof(Index));
        }

        private void MensajeRespuestaValidacionPermiso(int status)
        {

            var resultado = new ResponseModel();

            if (status == 200)
            {
                resultado.Mensaje = "La accion se ha realizado con exito";
                resultado.Icono = "success";
                // TempData["Mensaje"] = "La accion se ha realizado con exito";
                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            }
            else if (status == 401)
            {  //si el permiso no lo puede realizar el usuario debido a que su rol no le permite realizar la accion ( status 401)
                resultado.Mensaje = "No tienes permiso para realizar esta accion";
                resultado.Icono = "error";
                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            }
            else if (status == 402)
            {
                resultado.Mensaje = "El permiso para realizar esta accion no se encuentra activo";
                resultado.Icono = "info";
                TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            }
            else
            {
                Console.WriteLine("i'm failing in the name of permission");
            }
            //return (string)TempData["Mensaje"];
        }

        // GET: Devoluciones/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Devoluciones == null)
            {
                return NotFound();
            }

            var devolucion = await _context.Devoluciones.FindAsync(id);
            if (devolucion == null)
            {
                return NotFound();
            }
            ViewData["Id_prestamo"] = new SelectList(_context.Prestamos, "Id", "Id", devolucion.Id_prestamo);
            return View(devolucion);
        }

        // POST: Devoluciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Id_prestamo,Observaciones,Fecha_devolucion")] Devolucion devolucion)
        {
            Console.WriteLine("hola desdde registrar devoluciones");
            Console.WriteLine($"id: {devolucion.Id}");
            Console.WriteLine($"id: {devolucion.Id_prestamo}");
            Console.WriteLine($"id{devolucion.Observaciones}");
            Console.WriteLine($"id{devolucion.Fecha_devolucion}");

            int status = await _devolucionesServices.Editar(devolucion, User);

            MensajeRespuestaValidacionPermiso(status);

            return RedirectToAction(nameof(Index));
        }

        // GET: Devoluciones/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Devoluciones == null)
            {
                return NotFound();
            }

            var devolucion = await _context.Devoluciones
                .Include(d => d.Prestamo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (devolucion == null)
            {
                return NotFound();
            }

            return View(devolucion);
        }

        // POST: Devoluciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            int status = await _devolucionesServices.Eliminar(id,User);

            MensajeRespuestaValidacionPermiso(status);

            return RedirectToAction(nameof(Index));
        }

        private bool DevolucionExists(int id)
        {
          return (_context.Devoluciones?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
