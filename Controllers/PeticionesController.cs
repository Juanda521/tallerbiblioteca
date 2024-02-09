
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;


using tallerbiblioteca.Models;
using tallerbiblioteca.Services;
namespace tallerbiblioteca.Controllers
{
    [Authorize]
    public class PeticionesController : Controller
    {

        private PeticionesServices _peticionesServices;
        private UsuariosServices _usuariosServices;
        private EjemplarServices _ejemplaresServices;
        

        public PeticionesController(UsuariosServices usuariosServices, PeticionesServices peticionesServices, EjemplarServices ejemplarServices)
        {
            _peticionesServices = peticionesServices;
            _usuariosServices = usuariosServices;
            _ejemplaresServices = ejemplarServices;

        }

        // GET: Peticiones
        public async Task<IActionResult> Rechazadas(int itemsPagina = 5, int pagina = 1)
        {
            var peticionesRechazadas = await _peticionesServices.Rechazadas();

            int totalPeticionesRechazadas = peticionesRechazadas.Count;
            var total = (totalPeticionesRechazadas / 6) + 1;

            var peticionesRechazadasPaginadas = peticionesRechazadas
                .Skip((pagina - 1) * itemsPagina)
                .Take(itemsPagina)
                .ToList();

            Paginacion<Peticiones> paginacionRechazadas = new(peticionesRechazadasPaginadas, total, pagina, itemsPagina)
            {
                PeticionesViewModel = new PeticionesViewModel
                {
                    Peticiones = peticionesRechazadas
                }
            };

            return View("Rechazadas", paginacionRechazadas);
        }
        public async Task<IActionResult> BuscarFiltro(int itemsPagina = 5, int pagina = 1, string busqueda = null, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            PeticionesViewModel peticionesViewModel = new()
            {
                Peticiones = await _peticionesServices.BuscarObtener(busqueda, fechaInicio, fechaFin)
            };
            int totalPeticiones = peticionesViewModel.Peticiones.Count;
            var total = (totalPeticiones / 6) + 1;

            var peticionesPaginadas = peticionesViewModel.Peticiones.Skip((pagina - 1) * itemsPagina).Take(itemsPagina).ToList();

            Paginacion<Peticiones> paginacion = new(peticionesPaginadas, total, pagina, itemsPagina)
            {
                PeticionesViewModel = peticionesViewModel
            };


            return View("Index", paginacion);
        }
        public async Task<IActionResult> Index(int itemsPagina = 5, int pagina = 1)
        {

            PeticionesViewModel peticionesViewModel = new()
            {
                Peticiones = await _peticionesServices.Obtenerpeticiones()
            };

            int totalPeticiones = peticionesViewModel.Peticiones.Count;
            var total = (totalPeticiones / 6) + 1;

            var peticionesPaginadas = peticionesViewModel.Peticiones.Skip((pagina - 1) * itemsPagina).Take(itemsPagina).ToList();

            Paginacion<Peticiones> paginacion = new(peticionesPaginadas, total, pagina, itemsPagina)
            {
                PeticionesViewModel = peticionesViewModel
            };


            return View(paginacion);
        }
        //este es el metodo que nos devuelve las notificaciones que van aparecer en la campana del aplicativo
        [HttpGet]
        [Route("api/Notificaciones")]
        public async Task<List<Peticiones>> Notificaciones()
        {
            return await _peticionesServices.ObtenerpeticionesEnEspera();
        }


        private void MensajeRespuestaValidacionPermiso(ResponseModel resultado, int status)
        {
            Console.WriteLine("con este status vamos a mostrara la alerta:" + status);
            switch (status)
            {
                case 200:
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 401:
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 402:
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
            }
        }


        private void MensajeRespuestaPeticion(int status)
        {
            Console.WriteLine(status);
            var resultado = new ResponseModel();
            switch (status)
            {
                case 200:
                    string Mensaje = "";
                    if(_peticionesServices.ObtenerRolUserOnline(User)==1 ||_peticionesServices.ObtenerRolUserOnline(User)==3 ){
                        Mensaje = "La acción se ha realizado con exito, Puedes aceptar o rechazar esta petición en el apartado de peticiones";
                    }else{
                        Mensaje = "La accion se ha realizado con exito, Revise el correo en un lapso de 24 horas para la confirnmacion de su solicitud";
                    }
                    resultado.Mensaje = Mensaje;
                    resultado.Icono = "success";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 500:
                    resultado.Mensaje = "No puedes realizar esta acción ya que tienes una solicitud pendiente";
                    resultado.Icono = "info";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 402:
                    resultado.Mensaje = "El permiso para realizar esta accion no se encuentra activo";
                    resultado.Icono = "info";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 502:
                    resultado.Mensaje = "El libro solicitado no se encuentra Disponible, puedes reservarlo para que puedad disfrutar de el cuando esté disponible de nuevo en la biblioteca";
                    resultado.Icono = "info";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 503:
                    resultado.Mensaje = "No puedes realizar esta acción ya que tienes un prestamo en curso";
                    resultado.Icono = "info";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                 case 504:
                    resultado.Mensaje = "No puedes realizar esta acción ya que este usuario tiene un prestamo en curso";
                    resultado.Icono = "info";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 505:
                    resultado.Mensaje = "No puedes realizar esta acción ya que la peticion ya ha sido aceptada";
                    resultado.Icono = "error";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                     break;
                case 506:
                    resultado.Mensaje = "No puedes realizar esta acción porque  la peticion ya ha sido Rechazada";
                    resultado.Icono = "error";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                     break;
                default:
                    Console.WriteLine("i'm failing in the name of permission");
                    break;
            }

        }

        // GET: Peticiones/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (await _peticionesServices.Obtenerpeticiones() == null)
            {
                return NotFound();
            }
            var peticion = await _peticionesServices.Buscar(id);
            if (peticion == null)
            {
                return NotFound();
            }
            return View(peticion);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var ejemplares = await _ejemplaresServices.ObtenerEjemplares(); // Asegúrate de que tu servicio devuelva ejemplares con información de libro
            ViewData["Id_ejemplar"] = new SelectList(ejemplares, "Id", "Libro.Nombre"); // Usa el nombre de la propiedad que contiene el nombre del libro
            ViewData["Id_usuario"] = new SelectList(await _usuariosServices.ObtenerUsuarios(), "Id", "Name");
            return View();
        }
        // POST: Peticiones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Peticiones peticiones)
        {

            Console.WriteLine("id del ejemplar a registrar. vamos a registrar una fokin petición ", peticiones.Id_ejemplar);

            if(_usuariosServices.ValidarUsuarioEnPrestamo(peticiones.Id_usuario)){

                Console.WriteLine("Esta usuario tiene un prestamo en curso");
                //codigo 504 declarado para usuarios con prestamos en curso (desde la vista del administrador)
                 MensajeRespuestaPeticion(504);
                 return RedirectToAction("Index", "Peticiones");
            }
            int status = await _peticionesServices.Registrar(User, peticiones);

            if (status == 401 || status == 402)
            {
                var resultado = new ResponseModel();
                MensajeRespuestaValidacionPermiso(_peticionesServices.MensajeRespuestaValidacionPermiso(status), status);
            }
            else
            {
                MensajeRespuestaPeticion(status);
            }
            return RedirectToAction("Catalog", "Libros");
        }

        public async Task<IActionResult> Registrar()
        {

            Console.WriteLine("hablalo desde registrar");
            string idEjemplar = Request.Form["Id_ejemplar"];
            string motivo = Request.Form["Motivo"];
            string idUsuario = Request.Form["Id_usuario"];
            Console.WriteLine("aca deberia copier el id: ", idEjemplar);


            if (int.TryParse(idEjemplar, out int idEjemplarInt))
            {
                Console.WriteLine("id del ejemplar a registrar: {0}", idEjemplarInt);
            }
            else
            {
                //el codigo 502 lo hemos declarado como ejemplar no disponible
                MensajeRespuestaPeticion(502);
                Console.WriteLine("no esta parseando el ejemplar");
                return RedirectToAction("Catalog", "Libros");
            }
            if (int.TryParse(idUsuario, out int idUsuarioInt))
            {

                Console.WriteLine("id del usuario a registrar: {0}", idUsuarioInt);
            }
            if(_usuariosServices.ValidarUsuarioEnPrestamo(idUsuarioInt)){

                Console.WriteLine("Esta usuario tiene un prestamo en curso");
                //codigo 503 declarado para usuarios con prestamos en curso 
                 MensajeRespuestaPeticion(503);
                 return RedirectToAction("Catalog", "Libros");
            }

            Peticiones peticiones = new Peticiones();
            peticiones.Id_ejemplar = idEjemplarInt;
            peticiones.Id_usuario = idUsuarioInt;
            peticiones.Motivo = motivo;

           

            int status = await _peticionesServices.Registrar(User, peticiones);
            Console.WriteLine(status);

            if (status == 401 || status == 402)
            {
                var resultado = new ResponseModel();
                MensajeRespuestaValidacionPermiso(_peticionesServices.MensajeRespuestaValidacionPermiso(status), status);
            }
            else
            {
                MensajeRespuestaPeticion(status);
            }
            return RedirectToAction("Catalog", "Libros");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Console.WriteLine("vamos a cambiar el estado de la petición");

            var peticion  = await _peticionesServices.Buscar(id);

            if(peticion.Estado == "ACEPTADA"){
                Console.WriteLine($"La peticion ya ha sido aceptada");
                MensajeRespuestaPeticion(505);
                return RedirectToAction(nameof(Index));
            }

            if(peticion.Estado == "RECHAZADA"){
                Console.WriteLine($"La peticion ya ha sido rechazada");
                MensajeRespuestaPeticion(506);
                return RedirectToAction(nameof(Rechazadas));
            }
            

            int status = await _peticionesServices.EliminarPeticion(User, id);
            MensajeRespuestaValidacionPermiso(_peticionesServices.MensajeRespuestaValidacionPermiso(status), status);
            return RedirectToAction(nameof(Index));
        }

    }
}

