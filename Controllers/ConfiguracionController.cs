using Microsoft.AspNetCore.Authorization;
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;

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

        [HttpPost]
        public ActionResult SubirArchivo(IFormFile archivoCSV)
        {
            Console.WriteLine("llegamos a la funcion ");
            if (archivoCSV != null && archivoCSV.Length > 0)
            {
                Console.WriteLine("tenemos el archivo csv");
                using (var memoryStream = new MemoryStream())
                {
                    archivoCSV.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin); // Reiniciar el puntero del flujo de memoria

                    using (var reader = new StreamReader(memoryStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(';'); // Puedes ajustar el separador según el formato del CSV

                            if (values.Length >= 3) // Asegúrate de tener al menos 3 valores por fila (Documento, Nombre, Apellido)
                            {
                                long documento;
                                if (long.TryParse(values[0], out documento))
                                {
                                    Console.WriteLine("vamos a crear el estudiante matriculado");
                                    var matriculado = new Matriculados
                                    {
                                        Documento = documento,
                                        Nombre = values[1],
                                        Apellido = values[2]
                                    };
                                    Console.WriteLine($"se supone que creamos un matriculado con el sguiente numero de documento: {matriculado.Documento} ");

                                    _configServices.RegistrarExcel(matriculado);
                                }
                                else
                                {
                                    Console.WriteLine("No se pudo convertir el documento a long");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Formato incorrecto de fila en el archivo CSV");
                            }
                        }
                    }
                }

                _configServices.guardarUsuariosFromExcel();
                Console.WriteLine("Datos insertados en la base de datos correctamente.");

                return RedirectToAction("Index");
            }
            else
            {
                  Console.WriteLine("no tenemos el csv");
                ViewBag.MensajeError = "Por favor, seleccione un archivo CSV.";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public ActionResult SubirArchivoExcel(IFormFile archivoExcel)
        {
            if (archivoExcel != null && archivoExcel.Length > 0)
            {
                // Realizar la lógica para leer y procesar el archivo Excel
                // Puedes utilizar el código que proporcioné anteriormente
                using (var memoryStream = new MemoryStream())
                {
                    archivoExcel.CopyTo(memoryStream);
                
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;
                        if (rowCount>1){
                            Console.WriteLine("se encontraron datos en el archivo excel.");
                            for (int row = 2; row <= rowCount; row++)
                            {
                                Console.WriteLine("A la de dios cuantos de estos nos salgan");
                                long documento;
                                if (long.TryParse(worksheet.Cells[row, 1].Value.ToString(), out documento))
                                {
                                    var matriculado = new Matriculados
                                    {
                                        Documento = documento,
                                        Nombre = worksheet.Cells[row, 2].Value.ToString(),
                                        Apellido = worksheet.Cells[row, 3].Value.ToString(),
                                    };

                                    _configServices.RegistrarExcel(matriculado);
                                }else{
                                    Console.WriteLine("no parsea el numero en long");
                                }
                            }
                             _configServices.guardarUsuariosFromExcel();
                            Console.WriteLine("Datos insertados en la base de datos correctamente.");
                        }else{
                            Console.WriteLine("no se encontraron registros en el archivo excel");
                        }
                    
                    }
                }

                return RedirectToAction("Index"); // Redirige a la página principal después de procesar el archivo
            }
            else
            {
                Console.WriteLine("no estamos subiendo archivo");
                // Manejar el caso en el que no se seleccionó un archivo
                ViewBag.MensajeError = "Por favor, seleccione un archivo Excel.";
                return RedirectToAction("Index"); // Redirige a la página principal después de procesar el archivo
            }
        }

        public async Task<IActionResult> UsuariosInactivos()
        {
            try
            {       

                var usuarios =  await _configServices.UsuariosInactivos()  ;
                 ViewBag.UsuariosInactivos  = usuarios.Count;
                return View(usuarios);
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
                Console.WriteLine($"este es el id que esta llegando del formulario {id}");
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
