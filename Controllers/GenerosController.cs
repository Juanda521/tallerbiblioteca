using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tallerbiblioteca.Context;
using tallerbiblioteca.Migrations;
using tallerbiblioteca.Models;
using tallerbiblioteca.Services;

namespace tallerbiblioteca.Controllers
{
    public class GenerosController : Controller
    {
        private readonly BibliotecaDbContext _context;
        // private readonly GeneroServices _generosServices; 

        public GenerosController(BibliotecaDbContext context)
        {
            _context = context;
            // _generosServices = new GeneroServices(_context);
        }

        // GET: Generoes
        public async Task<IActionResult> Index()
        {
              return _context.Genero != null ? 
                          View(await _context.Genero.ToListAsync()) :
                          Problem("Entity set 'BibliotecaDbContext.Generos'  is null.");
        }

        // GET: Generoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Genero == null)
            {
                return NotFound();
            }

            var genero = await _context.Genero
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // GET: Generoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Generoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombreGenero")] Genero genero)
        {
            if (ModelState.IsValid)
            {
                if (_context.Libros.FirstOrDefault(g => g.Id == genero.Id) == null)
                {
                      _context.Add(genero);
                    if(await _context.SaveChangesAsync()>0){
                          await _context.SaveChangesAsync();
                        TempData["Creacion"] = "Genero creado correctamente";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al momento de la creacion";
                    }
                    ViewData["Id"] = new SelectList(_context.Libros, "Id", "Nombre");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al momento de la creacion";
                    return View();
                }

            }
            else
            {
                TempData["ErrorMessage"] = "Error";
                return View();
            }
            //if (ModelState.IsValid)
            //{
            //    _context.Add(genero);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //return View(genero);
        }

        // GET: Generoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Genero == null)
            {
                return NotFound();
            }

            var genero = await _context.Genero.FindAsync(id);
            if (genero == null)
            {
                return NotFound();
            }
            return View(genero);
        }

        // POST: Generoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreGenero")] Genero genero)
        {
            if (id != genero.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genero);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneroExists(genero.Id))
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
            return View(genero);
        }

        // GET: Generoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Genero == null)
            {
                return NotFound();
            }

            var genero = await _context.Genero
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // POST: Generoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Genero == null)
            {
                return Problem("Entity set 'BibliotecaDbContext.Genero'  is null.");
            }
            var genero = await _context.Genero.FindAsync(id);
            if (genero != null)
            {
                _context.Genero.Remove(genero);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneroExists(int id)
        {
          return (_context.Genero?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
