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
    /// <summary>
    /// Controller responsible for managing genre operations.
    /// Requires Admin role for access to all actions.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class GenreModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the GenreModelsController.
        /// </summary>
        /// <param name="context">The database context to be used.</param>
        public GenreModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all genres with optional name filtering.
        /// </summary>
        /// <param name="searchString">Optional text to filter genres by name.</param>
        /// <returns>A view containing the filtered list of genres.</returns>
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

        /// <summary>
        /// Displays detailed information about a specific genre.
        /// </summary>
        /// <param name="id">The ID of the genre to display.</param>
        /// <returns>A view containing the genre details, or NotFound if the genre doesn't exist.</returns>
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

        /// <summary>
        /// Displays the form for creating a new genre.
        /// </summary>
        /// <returns>A view containing the genre creation form.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processes the creation of a new genre.
        /// </summary>
        /// <param name="genreModel">The genre model containing the form data.</param>
        /// <returns>Redirects to Index if successful, or returns to the form if validation fails.</returns>
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

        /// <summary>
        /// Displays the form for editing an existing genre.
        /// </summary>
        /// <param name="id">The ID of the genre to edit.</param>
        /// <returns>A view containing the genre edit form, or NotFound if the genre doesn't exist.</returns>
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

        /// <summary>
        /// Processes the update of an existing genre.
        /// </summary>
        /// <param name="id">The ID of the genre to update.</param>
        /// <param name="genreModel">The genre model containing the updated data.</param>
        /// <returns>Redirects to Index if successful, or returns to the form if validation fails.</returns>
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

        /// <summary>
        /// Displays the confirmation page for deleting a genre.
        /// </summary>
        /// <param name="id">The ID of the genre to delete.</param>
        /// <returns>A view asking for deletion confirmation, or NotFound if the genre doesn't exist.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genreModel = await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genreModel == null)
            {
                return NotFound();
            }

            return View(genreModel);
        }

        /// <summary>
        /// Processes the deletion of a genre.
        /// Cannot delete genres that have associated books.
        /// </summary>
        /// <param name="id">The ID of the genre to delete.</param>
        /// <returns>Redirects to Index after successful deletion, or shows error if deletion is not allowed.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genreModel = await _context.Genres
                .Include(g => g.Books)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (genreModel == null)
            {
                return NotFound();
            }

            if (genreModel.Books.Any())
            {
                ModelState.AddModelError("", "Cannot delete genre that has books assigned to it.");
                return View(genreModel);
            }

            _context.Genres.Remove(genreModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a genre exists in the database.
        /// </summary>
        /// <param name="id">The ID of the genre to check.</param>
        /// <returns>True if the genre exists, false otherwise.</returns>
        private bool GenreModelExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }
    }
}
