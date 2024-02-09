using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace tallerbiblioteca.Controllers
{
    [Authorize]
    public class RolController : Controller
    {
        private readonly BibliotecaDbContext _context;
        private RolServices _rol_services;

        public RolController(BibliotecaDbContext context,RolServices rolServices)
        {
            _context = context;
            _rol_services = rolServices;
        }

        // GET: Rol
        public async Task<IActionResult> Index(string busqueda,int Numero_pagina = 1, int itemsPagina = 10)
        {

            
            var roles = await _rol_services.ObtenerRoles();

            if (busqueda!=null){
                roles = _rol_services.Busqueda(busqueda);
            }
            int total_roles = roles.Count;

            

            var RolesPaginados = roles.Skip((Numero_pagina-1)*itemsPagina).Take(itemsPagina).ToList();

            Paginacion<Rol> paginacion  = new Paginacion<Rol>(RolesPaginados,total_roles,Numero_pagina,itemsPagina);


            return View(paginacion);
        }

        // GET: Rol/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Rol == null)
            {
                return NotFound();
            }

            var rol = await _context.Rol
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rol == null)
            {
                return NotFound();
            }

            var configuraciones =  await _context.Configuracion.Include(c=>c.Permiso).Where(c=>c.Id_rol == id).ToListAsync();
            
           

            ConfiguracionViewModel detallesRol =  new ConfiguracionViewModel 
            { 
                Rol = rol,
                Configuraciones = configuraciones
            
            };

            return View(detallesRol);
        }

        // GET: Rol/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rol/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Estado")] Rol rol)
        {
            
            if (ModelState.IsValid)
            {

                _context.Add(rol);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rol);
        }

        // GET: Rol/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Rol == null)
            {
                return NotFound();
            }

            var rol = await _context.Rol.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }
            return View(rol);
        }

        // POST: Rol/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Estado")] Rol rol)
        {
            Console.WriteLine("estamos en editar");
            if (id != rol.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    MensajeRespuestaValidacionPermiso(await _rol_services.Editar(User,rol));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolExists(rol.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rol);
        }
        
         public void MensajeRespuestaValidacionPermiso(ResponseModel resultado){
            
            TempData["Mensaje"] = JsonConvert.SerializeObject(resultado);
            
        }  

        // GET: Rol/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Rol == null)
            {
                return NotFound();
            }

            var rol = await _context.Rol
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rol == null)
            {
                return NotFound();
            }

            return View(rol);
        }

        // POST: Rol/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Rol == null)
            {
                return Problem("Entity set 'BibliotecaDbContext.Rol'  is null.");
            }
            var rol = await _context.Rol.FindAsync(id);
            if (rol != null)
            {
                _context.Rol.Remove(rol);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RolExists(int id)
        {
            return (_context.Rol?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
