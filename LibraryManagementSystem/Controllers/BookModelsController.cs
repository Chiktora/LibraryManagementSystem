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
    public class BookModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

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

        // GET: BookModels
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BookModels/Details/5
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

        // GET: BookModels/Create
        public IActionResult Create()
        {
            PopulateViewBags();
            return View();
        }

        // POST: BookModels/Create
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

        // GET: BookModels/Edit/5
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

        // POST: BookModels/Edit/5
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

        // GET: BookModels/Delete/5
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

        // POST: BookModels/Delete/5
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
