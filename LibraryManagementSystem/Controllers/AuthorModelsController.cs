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
    /// Controller responsible for managing author operations.
    /// Requires Admin role for access to all actions.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AuthorModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the AuthorModelsController.
        /// </summary>
        /// <param name="context">The database context to be used.</param>
        public AuthorModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays a list of all authors with optional name filtering.
        /// </summary>
        /// <param name="searchString">Optional text to filter authors by first or last name.</param>
        /// <returns>A view containing the filtered list of authors.</returns>
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.Authors
                .Include(a => a.BookAuthors)
                .AsQueryable();

            // Apply name filter
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(a => 
                    a.FirstName.Contains(searchString) || 
                    a.LastName.Contains(searchString));
            }

            return View(await query.ToListAsync());
        }

        /// <summary>
        /// Displays detailed information about a specific author.
        /// </summary>
        /// <param name="id">The ID of the author to display.</param>
        /// <returns>A view containing the author details, or NotFound if the author doesn't exist.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorModel = await _context.Authors
                .Include(a => a.BookAuthors)
                    .ThenInclude(ba => ba.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authorModel == null)
            {
                return NotFound();
            }

            return View(authorModel);
        }

        /// <summary>
        /// Displays the form for creating a new author.
        /// </summary>
        /// <returns>A view containing the author creation form.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Processes the creation of a new author.
        /// </summary>
        /// <param name="authorModel">The author model containing the form data.</param>
        /// <returns>Redirects to Index if successful, or returns to the form if validation fails.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName")] AuthorModel authorModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(authorModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(authorModel);
        }

        /// <summary>
        /// Displays the form for editing an existing author.
        /// </summary>
        /// <param name="id">The ID of the author to edit.</param>
        /// <returns>A view containing the author edit form, or NotFound if the author doesn't exist.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorModel = await _context.Authors.FindAsync(id);
            if (authorModel == null)
            {
                return NotFound();
            }
            return View(authorModel);
        }

        /// <summary>
        /// Processes the update of an existing author.
        /// </summary>
        /// <param name="id">The ID of the author to update.</param>
        /// <param name="authorModel">The author model containing the updated data.</param>
        /// <returns>Redirects to Index if successful, or returns to the form if validation fails.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName")] AuthorModel authorModel)
        {
            if (id != authorModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(authorModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorModelExists(authorModel.Id))
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
            return View(authorModel);
        }

        /// <summary>
        /// Displays the confirmation page for deleting an author.
        /// </summary>
        /// <param name="id">The ID of the author to delete.</param>
        /// <returns>A view asking for deletion confirmation, or NotFound if the author doesn't exist.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorModel = await _context.Authors
                .Include(a => a.BookAuthors)
                    .ThenInclude(ba => ba.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authorModel == null)
            {
                return NotFound();
            }

            return View(authorModel);
        }

        /// <summary>
        /// Processes the deletion of an author.
        /// </summary>
        /// <param name="id">The ID of the author to delete.</param>
        /// <returns>Redirects to Index after successful deletion.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var authorModel = await _context.Authors
                .Include(a => a.BookAuthors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (authorModel != null)
            {
                _context.BookAuthors.RemoveRange(authorModel.BookAuthors);
                _context.Authors.Remove(authorModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if an author exists in the database.
        /// </summary>
        /// <param name="id">The ID of the author to check.</param>
        /// <returns>True if the author exists, false otherwise.</returns>
        private bool AuthorModelExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
