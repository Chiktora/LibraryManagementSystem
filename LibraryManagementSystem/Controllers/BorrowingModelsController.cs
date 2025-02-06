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
    public class BorrowingModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowingModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BorrowingModels
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Borrowings.Include(b => b.Book).Include(b => b.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BorrowingModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowingModel = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrowingModel == null)
            {
                return NotFound();
            }

            return View(borrowingModel);
        }

        // GET: BorrowingModels/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: BorrowingModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,UserId,BorrowDate,ReturnDate,Status")] BorrowingModel borrowingModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(borrowingModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", borrowingModel.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", borrowingModel.UserId);
            return View(borrowingModel);
        }

        // GET: BorrowingModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowingModel = await _context.Borrowings.FindAsync(id);
            if (borrowingModel == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", borrowingModel.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", borrowingModel.UserId);
            return View(borrowingModel);
        }

        // POST: BorrowingModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,UserId,BorrowDate,ReturnDate,Status")] BorrowingModel borrowingModel)
        {
            if (id != borrowingModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowingModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowingModelExists(borrowingModel.Id))
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
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", borrowingModel.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", borrowingModel.UserId);
            return View(borrowingModel);
        }

        // GET: BorrowingModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var borrowingModel = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrowingModel == null)
            {
                return NotFound();
            }

            return View(borrowingModel);
        }

        // POST: BorrowingModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrowingModel = await _context.Borrowings.FindAsync(id);
            if (borrowingModel != null)
            {
                _context.Borrowings.Remove(borrowingModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowingModelExists(int id)
        {
            return _context.Borrowings.Any(e => e.Id == id);
        }
    }
}
