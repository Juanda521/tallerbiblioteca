using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;

namespace tallerbiblioteca.Controllers
{
    [Authorize(Roles = "1")]
    public class ConfiguracionController : Controller
    {
       
        private readonly ConfiguracionServices _configServices;
       
    
        public ConfiguracionController(ConfiguracionServices configuracionServices)
        {
            _configServices =  configuracionServices;    
        }

        public async Task<IActionResult> UsuariosInactivos()
        {
            try
            {            
                return View(await _configServices.UsuariosInactivos());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: ConfiguracionController
        [Authorize]
        public async Task<IActionResult> Index()
        {
            //creamos un objeto de ConfiguracionViewModel para poder enviar varios modelos a la vista
            var detalles = await _configServices.CrearViewModel();

            //iteramos sobre cada rol para mostrar las configurciones asociadas
            foreach (var rol in detalles.Roles)
            {
                //hacemos uso de la funcion mostrarConfiguracion la cual se encargara de añadir a la lista de las configuraciones del rol, las configuraciones asociadas a esta
                //debemos enviarle el id del rol por el cual va buscar y el modelo para que le añada las condfiguraciones a su respectiva lista
                detalles = _configServices.MostrarConfiguracion(rol.Id, detalles);
                switch (rol.Id)
                {
                    case 1:
                        ViewData["PermisosNoAsociadosAdmin"] = new SelectList(_configServices.PermisosNoAsociados(rol.Id), "Id", "Nombre");
                        break;
                    case 2:
                        ViewData["PermisosNoAsociadosUser"] = new SelectList(_configServices.PermisosNoAsociados(rol.Id), "Id", "Nombre");
                        break;
                    case 3:
                        ViewData["PermisosNoAsociadosAlfa"] = new SelectList(_configServices.PermisosNoAsociados(rol.Id), "Id", "Nombre");
                        break;
                }
            }
            ViewData["Id_permiso"] = new SelectList(_configServices.ObtenerPermisos(), "Id", "Nombre");
            return View(detalles);
        }

        // GET: ConfiguracionController/Create
        public ActionResult Create()
        {
            ViewData["Id_permiso"] = new SelectList(_configServices.ObtenerPermisos(), "Id", "Id");
            return View();
        }

        // POST: ConfiguracionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Configuracion configuracion)
        {
            try
            {
                await _configServices.Create(configuracion);
                MensajeRespuestaValidacionPermiso(_configServices.MensajeRespuestaValidacionPermiso(200));
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Json(new { success = false }); // Respuesta JSON
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                await _configServices.Edit(id);
                 MensajeRespuestaValidacionPermiso(_configServices.MensajeRespuestaValidacionPermiso(200));
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Json(new { success = false }); // Respuesta JSON
            }
        }

        private void MensajeRespuestaValidacionPermiso(ResponseModel resultado){
            
            TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            Console.WriteLine("ya pusimos el mensaje en la variable global");
             
         }       

        // POST: ConfiguracionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _configServices.Delete(id);
            MensajeRespuestaValidacionPermiso(_configServices.MensajeRespuestaValidacionPermiso(200));
            return RedirectToAction(nameof(Index));
            
        }

    }
}
//public IActionResult AccesoDenegado()
//{
//    if (User.Identity.IsAuthenticated)
//    {
//        if (User.IsInRole("1"))
//        {
//            // Usuario con rol "1" intentó acceder a una página protegida, puedes redirigirlo si lo deseas
//            return RedirectToAction(nameof(Index));
//        }
//    }

//    // Si el usuario no está autenticado o no tiene el rol "1", muestra la página de acceso denegado normalmente
//    return View();

//}
