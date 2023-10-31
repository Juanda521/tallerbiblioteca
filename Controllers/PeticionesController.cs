
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

        public PeticionesController(UsuariosServices usuariosServices,PeticionesServices peticionesServices,EjemplarServices ejemplarServices)
        {
            _peticionesServices  = peticionesServices;
            _usuariosServices  = usuariosServices;
            _ejemplaresServices  = ejemplarServices;

        }

        // GET: Peticiones
        public async Task<IActionResult> Index()
        {
            
            PeticionesViewModel peticionesViewModel = new()
            {
                Peticiones = await _peticionesServices.Obtenerpeticiones()
            };

            

        foreach (var item in peticionesViewModel.Peticiones)
        {
            Console.WriteLine(item.Usuario.Name);
            Console.WriteLine(item.FechaPeticion);

        }
            return View(peticionesViewModel);
        }

        [HttpGet]
        [Route("api/notificaciones")]
        public async Task<List<Peticiones>> Notificaciones()
        {
            return  await _peticionesServices.Obtenerpeticiones();
        }


         private void MensajeRespuestaValidacionPermiso(ResponseModel resultado,int status){
            Console.WriteLine("con este status vamos a mostrara la alerta:" +status);
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
                    resultado.Mensaje  =  "La accion se ha realizado con exito, Revise el correo en un lapso de 24 horas para la confirnmacion de su solicitud";
                    resultado.Icono  = "success";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 500:
                    resultado.Mensaje  =  "No puedes realizar esta acción ya que tienes una solicitud pendiente";
                    resultado.Icono  = "info";
                    TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
                    break;
                case 402:
                    resultado.Mensaje  = "El permiso para realizar esta accion no se encuentra activo";
                    resultado.Icono  = "info";
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

        public async Task<IActionResult> Create()
        {
            ViewData["Id_ejemplar"] = new SelectList(await _ejemplaresServices.ObtenerEjemplares(), "Id", "Id");
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

            Console.WriteLine("id del ejemplar a registrar: ",peticiones.Id_ejemplar);
            int status = await _peticionesServices.Registrar(User,peticiones);

            if (status == 401 || status == 402){
                var resultado = new ResponseModel();
                MensajeRespuestaValidacionPermiso(_peticionesServices.MensajeRespuestaValidacionPermiso(status),status) ;          
            }else{
                MensajeRespuestaPeticion(status);        
            }
            return RedirectToAction("Catalog","Libros");
        }
        public async Task<IActionResult> Delete(int id)
        {
            Console.WriteLine("oelo");
            int status  = await _peticionesServices.EliminarPeticion(User,id);
            MensajeRespuestaValidacionPermiso(_peticionesServices.MensajeRespuestaValidacionPermiso(status),status);
            return RedirectToAction(nameof(Index));
        }

    }
}

