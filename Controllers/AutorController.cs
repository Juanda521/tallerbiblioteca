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
    public class AutorController : Controller
    {
        private readonly BibliotecaDbContext _context;
        // private readonly AutoresServices _autorServices;

        public AutorController(BibliotecaDbContext context)
        {
            _context = context;
            // _autorServices = new AutoresServices(_context);

        }

        // GET: Autor
        public async Task<IActionResult> Index()
        {
              return _context.Autores != null ? 
                          View(await _context.Autores.ToListAsync()) :
                          Problem("Entity set 'BibliotecaDbContext.Autores'  is null.");
        }

        // GET: Autor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Autores == null)
            {
                return NotFound();
            }

            var autor = await _context.Autores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // GET: Autor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Autor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombreAutor")] Autor autor)
        {
            if (ModelState.IsValid)
            {
                if (_context.Autores.FirstOrDefault(a => a.Id == autor.Id) == null)
                {
                        _context.Add(autor);
                    if(await _context.SaveChangesAsync()>0){
                          await _context.SaveChangesAsync();
                        TempData["Creacion"] = "autor creado correctamente";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al momento de la creacion";
                    }
                    ViewData["Id"] = new SelectList(_context.Libros, "Id", "Nombre");
                    return View();
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
        }

        // GET: Autor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Autores == null)
            {
                return NotFound();
            }

            var autor = await _context.Autores.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        // POST: Autor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreAutor")] Autor autor)
        {
            if (id != autor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(autor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AutorExists(autor.Id))
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
            return View(autor);
        }

        // GET: Autor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Autores == null)
            {
                return NotFound();
            }

            var autor = await _context.Autores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // POST: Autor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Autores == null)
            {
                return Problem("Entity set 'BibliotecaDbContext.Autores'  is null.");
            }
            var autor = await _context.Autores.FindAsync(id);
            if (autor != null)
            {
                _context.Autores.Remove(autor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AutorExists(int id)
        {
          return (_context.Autores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
