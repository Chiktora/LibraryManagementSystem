using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GenreModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GenreModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GenreModels
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Genres
                .Include(g => g.Books)
                .AsQueryable();

            // Apply name filter
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(g => g.Name.Contains(searchString));
            }

            return View(await query.ToListAsync());
        }

        // GET: GenreModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genreModel = await _context.Genres
                .Include(g => g.Books)
                    .ThenInclude(b => b.BookAuthors)
                        .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genreModel == null)
            {
                return NotFound();
            }

            return View(genreModel);
        }

        // GET: GenreModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GenreModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] GenreModel genreModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genreModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genreModel);
        }

        // GET: GenreModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genreModel = await _context.Genres.FindAsync(id);
            if (genreModel == null)
            {
                return NotFound();
            }
            return View(genreModel);
        }

        // POST: GenreModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] GenreModel genreModel)
        {
            if (id != genreModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genreModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreModelExists(genreModel.Id))
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
            return View(genreModel);
        }

        // GET: GenreModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genreModel = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genreModel == null)
            {
                return NotFound();
            }

            return View(genreModel);
        }

        // POST: GenreModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genreModel = await _context.Genres.FindAsync(id);
            if (genreModel != null)
            {
                _context.Genres.Remove(genreModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GenreModelExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
    }
}
