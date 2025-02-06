using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class BookAuthorModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookAuthorModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BookAuthorModels
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BookAuthors.Include(b => b.Author).Include(b => b.Book);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BookAuthorModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookAuthorModel = await _context.BookAuthors
                .Include(b => b.Author)
                .Include(b => b.Book)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (bookAuthorModel == null)
            {
                return NotFound();
            }

            return View(bookAuthorModel);
        }

        // GET: BookAuthorModels/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FirstName");
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title");
            return View();
        }

        // POST: BookAuthorModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,AuthorId")] BookAuthorModel bookAuthorModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookAuthorModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FirstName", bookAuthorModel.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bookAuthorModel.BookId);
            return View(bookAuthorModel);
        }

        // GET: BookAuthorModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookAuthorModel = await _context.BookAuthors.FindAsync(id);
            if (bookAuthorModel == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FirstName", bookAuthorModel.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bookAuthorModel.BookId);
            return View(bookAuthorModel);
        }

        // POST: BookAuthorModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,AuthorId")] BookAuthorModel bookAuthorModel)
        {
            if (id != bookAuthorModel.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookAuthorModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookAuthorModelExists(bookAuthorModel.BookId))
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
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "FirstName", bookAuthorModel.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", bookAuthorModel.BookId);
            return View(bookAuthorModel);
        }

        // GET: BookAuthorModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookAuthorModel = await _context.BookAuthors
                .Include(b => b.Author)
                .Include(b => b.Book)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (bookAuthorModel == null)
            {
                return NotFound();
            }

            return View(bookAuthorModel);
        }

        // POST: BookAuthorModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookAuthorModel = await _context.BookAuthors.FindAsync(id);
            if (bookAuthorModel != null)
            {
                _context.BookAuthors.Remove(bookAuthorModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookAuthorModelExists(int id)
        {
            return _context.BookAuthors.Any(e => e.BookId == id);
        }
    }
}
