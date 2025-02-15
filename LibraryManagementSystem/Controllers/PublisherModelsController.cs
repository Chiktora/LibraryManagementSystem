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
    /// Controller responsible for managing publisher operations.
    /// Requires Admin role for access to all actions.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class PublisherModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the PublisherModelsController.
        /// </summary>
        /// <param name="context">The database context to be used.</param>
        public PublisherModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all publishers with optional name filtering.
        /// </summary>
        /// <param name="searchString">Optional text to filter publishers by name.</param>
        /// <returns>A view containing the filtered list of publishers.</returns>
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Publishers
                .Include(p => p.Books)
                .AsQueryable();

            // Apply name filter
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.Name.Contains(searchString));
            }

            return View(await query.ToListAsync());
        }

        /// <summary>
        /// Displays detailed information about a specific publisher.
        /// </summary>
        /// <param name="id">The ID of the publisher to display.</param>
        /// <returns>A view containing the publisher details, or NotFound if the publisher doesn't exist.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisherModel = await _context.Publishers
                .Include(p => p.Books)
                    .ThenInclude(b => b.BookAuthors)
                        .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (publisherModel == null)
            {
                return NotFound();
            }

            return View(publisherModel);
        }

        /// <summary>
        /// Displays the form for creating a new publisher.
        /// </summary>
        /// <returns>A view containing the publisher creation form.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processes the creation of a new publisher.
        /// </summary>
        /// <param name="publisherModel">The publisher model containing the form data.</param>
        /// <returns>Redirects to Index if successful, or returns to the form if validation fails.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] PublisherModel publisherModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(publisherModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(publisherModel);
        }

        /// <summary>
        /// Displays the form for editing an existing publisher.
        /// </summary>
        /// <param name="id">The ID of the publisher to edit.</param>
        /// <returns>A view containing the publisher edit form, or NotFound if the publisher doesn't exist.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisherModel = await _context.Publishers.FindAsync(id);
            if (publisherModel == null)
            {
                return NotFound();
            }
            return View(publisherModel);
        }

        /// <summary>
        /// Processes the update of an existing publisher.
        /// </summary>
        /// <param name="id">The ID of the publisher to update.</param>
        /// <param name="publisherModel">The publisher model containing the updated data.</param>
        /// <returns>Redirects to Index if successful, or returns to the form if validation fails.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] PublisherModel publisherModel)
        {
            if (id != publisherModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publisherModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublisherModelExists(publisherModel.Id))
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
            return View(publisherModel);
        }

        /// <summary>
        /// Displays the confirmation page for deleting a publisher.
        /// </summary>
        /// <param name="id">The ID of the publisher to delete.</param>
        /// <returns>A view asking for deletion confirmation, or NotFound if the publisher doesn't exist.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisherModel = await _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (publisherModel == null)
            {
                return NotFound();
            }

            return View(publisherModel);
        }

        /// <summary>
        /// Processes the deletion of a publisher.
        /// Cannot delete publishers that have associated books.
        /// </summary>
        /// <param name="id">The ID of the publisher to delete.</param>
        /// <returns>Redirects to Index after successful deletion, or shows error if deletion is not allowed.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publisherModel = await _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (publisherModel == null)
            {
                return NotFound();
            }

            if (publisherModel.Books.Any())
            {
                ModelState.AddModelError("", "Cannot delete publisher that has books assigned to it.");
                return View(publisherModel);
            }

            _context.Publishers.Remove(publisherModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a publisher exists in the database.
        /// </summary>
        /// <param name="id">The ID of the publisher to check.</param>
        /// <returns>True if the publisher exists, false otherwise.</returns>
        private bool PublisherModelExists(int id)
        {
            return _context.Publishers.Any(e => e.Id == id);
        }
    }
}
