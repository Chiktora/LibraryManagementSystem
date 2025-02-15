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
    /// Controller responsible for managing book operations.
    /// Requires Admin role for access to all actions.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class BookModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the BookModelsController.
        /// </summary>
        /// <param name="context">The database context to be used.</param>
        public BookModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Populates ViewData with lists for dropdowns (genres, publishers, authors).
        /// </summary>
        /// <param name="bookModel">Optional book model to set selected values in dropdowns.</param>
        private void PopulateViewBags(BookModel bookModel = null)
        {
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", bookModel?.GenreId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", bookModel?.PublisherId);
            
            var authors = _context.Authors.Select(a => new
            {
                Id = a.Id,
                Name = $"{a.FirstName} {a.LastName}"
            }).ToList();

            if (bookModel != null && bookModel.BookAuthors != null)
            {
                ViewData["Authors"] = new MultiSelectList(authors, "Id", "Name", 
                    bookModel.BookAuthors.Select(ba => ba.AuthorId));
            }
            else
            {
                ViewData["Authors"] = new MultiSelectList(authors, "Id", "Name");
            }
        }

        /// <summary>
        /// Displays a list of all books with optional filtering.
        /// </summary>
        /// <param name="searchString">Optional text to filter books by title or ISBN.</param>
        /// <param name="genreId">Optional genre ID to filter books.</param>
        /// <param name="publisherId">Optional publisher ID to filter books.</param>
        /// <param name="authorId">Optional author ID to filter books.</param>
        /// <returns>A view containing the filtered list of books.</returns>
        public async Task<IActionResult> Index(string searchString, int? genreId, int? publisherId, int? authorId)
        {
            var query = _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.Title.Contains(searchString) || b.ISBN.Contains(searchString));
            }

            if (genreId.HasValue)
            {
                query = query.Where(b => b.GenreId == genreId.Value);
            }

            if (publisherId.HasValue)
            {
                query = query.Where(b => b.PublisherId == publisherId.Value);
            }

            if (authorId.HasValue)
            {
                query = query.Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId.Value));
            }

            // Get distinct genres and publishers for filter dropdowns
            ViewBag.Genres = new SelectList(await _context.Genres.OrderBy(g => g.Name).ToListAsync(), "Id", "Name");
            ViewBag.Publishers = new SelectList(await _context.Publishers.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            ViewBag.Authors = new SelectList(
                await _context.Authors
                    .OrderBy(a => a.LastName)
                    .ThenBy(a => a.FirstName)
                    .Select(a => new { Id = a.Id, Name = $"{a.FirstName} {a.LastName}" })
                    .ToListAsync(),
                "Id",
                "Name"
            );

            return View(await query.ToListAsync());
        }

        /// <summary>
        /// Displays detailed information about a specific book.
        /// </summary>
        /// <param name="id">The ID of the book to display.</param>
        /// <returns>A view containing the book details, or NotFound if the book doesn't exist.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookModel = await _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookModel == null)
            {
                return NotFound();
            }

            return View(bookModel);
        }

        /// <summary>
        /// Displays the form for creating a new book.
        /// </summary>
        /// <returns>A view containing the book creation form.</returns>
        public IActionResult Create()
        {
            PopulateViewBags();
            return View();
        }

        /// <summary>
        /// Processes the creation of a new book.
        /// </summary>
        /// <param name="bookModel">The book model containing the form data.</param>
        /// <returns>Redirects to Index if successful, or returns to the form if validation fails.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ISBN,GenreId,PublisherId,PublishedDate,Description,AuthorIds")] BookModel bookModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookModel);
                await _context.SaveChangesAsync();

                if (bookModel.AuthorIds != null && bookModel.AuthorIds.Any())
                {
                    foreach (var authorId in bookModel.AuthorIds)
                    {
                        var bookAuthor = new BookAuthorModel
                        {
                            BookId = bookModel.Id,
                            AuthorId = authorId
                        };
                        _context.BookAuthors.Add(bookAuthor);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateViewBags(bookModel);
            return View(bookModel);
        }

        /// <summary>
        /// Displays the form for editing an existing book.
        /// </summary>
        /// <param name="id">The ID of the book to edit.</param>
        /// <returns>A view containing the book edit form, or NotFound if the book doesn't exist.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookModel = await _context.Books
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (bookModel == null)
            {
                return NotFound();
            }

            PopulateViewBags(bookModel);
            return View(bookModel);
        }

        /// <summary>
        /// Processes the update of an existing book.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="bookModel">The book model containing the updated data.</param>
        /// <returns>Redirects to Index if successful, or returns to the form if validation fails.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ISBN,GenreId,PublisherId,PublishedDate,Description,AuthorIds")] BookModel bookModel)
        {
            if (id != bookModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Remove existing book-author relationships
                    var existingBookAuthors = await _context.BookAuthors
                        .Where(ba => ba.BookId == bookModel.Id)
                        .ToListAsync();
                    _context.BookAuthors.RemoveRange(existingBookAuthors);

                    // Add new book-author relationships
                    if (bookModel.AuthorIds != null && bookModel.AuthorIds.Any())
                    {
                        foreach (var authorId in bookModel.AuthorIds)
                        {
                            var bookAuthor = new BookAuthorModel
                            {
                                BookId = bookModel.Id,
                                AuthorId = authorId
                            };
                            _context.BookAuthors.Add(bookAuthor);
                        }
                    }

                    _context.Update(bookModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookModelExists(bookModel.Id))
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

            PopulateViewBags(bookModel);
            return View(bookModel);
        }

        /// <summary>
        /// Displays the confirmation page for deleting a book.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>A view asking for deletion confirmation, or NotFound if the book doesn't exist.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookModel = await _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookModel == null)
            {
                return NotFound();
            }

            return View(bookModel);
        }

        /// <summary>
        /// Processes the deletion of a book.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>Redirects to Index after successful deletion.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookModel = await _context.Books
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookModel != null)
            {
                _context.BookAuthors.RemoveRange(bookModel.BookAuthors);
                _context.Books.Remove(bookModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookModelExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
