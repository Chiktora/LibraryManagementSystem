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
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class BorrowingModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BorrowingModelsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BorrowingModels
        public async Task<IActionResult> Index(string searchString, string status, DateTime? fromDate, DateTime? toDate)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            IQueryable<BorrowingModel> borrowings = _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User);

            if (!isAdmin)
            {
                // Regular users can only see their own borrowings
                borrowings = borrowings.Where(b => b.UserId == user.Id);
            }

            // Apply filters
            if (!string.IsNullOrEmpty(searchString))
            {
                borrowings = borrowings.Where(b => b.Book.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(status))
            {
                borrowings = borrowings.Where(b => b.Status == status);
            }

            if (fromDate.HasValue)
            {
                borrowings = borrowings.Where(b => b.BorrowDate.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                borrowings = borrowings.Where(b => b.BorrowDate.Date <= toDate.Value.Date);
            }

            // Get distinct statuses for the filter dropdown
            var statuses = await _context.Borrowings.Select(b => b.Status).Distinct().ToListAsync();
            ViewBag.Statuses = new SelectList(statuses);

            return View(await borrowings.ToListAsync());
        }

        // GET: BorrowingModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var borrowingModel = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (borrowingModel == null)
            {
                return NotFound();
            }

            if (!isAdmin && borrowingModel.UserId != user.Id)
            {
                return Forbid();
            }

            return View(borrowingModel);
        }

        // GET: BorrowingModels/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title");
            return View();
        }

        // POST: BorrowingModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId")] BorrowingModel borrowingModel)
        {
            // Set required properties before ModelState validation
            var user = await _userManager.GetUserAsync(User);
            borrowingModel.UserId = user.Id;
            borrowingModel.Status = "Pending";
            borrowingModel.BorrowDate = DateTime.Now;

            // Clear any existing ModelState errors for these properties
            ModelState.Remove("UserId");
            ModelState.Remove("Status");
            ModelState.Remove("BorrowDate");

            if (ModelState.IsValid)
            {
                _context.Add(borrowingModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", borrowingModel.BookId);
            return View(borrowingModel);
        }

        // GET: BorrowingModels/Edit/5
        [Authorize(Roles = "Admin")]
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
            ViewData["UserId"] = new SelectList(_context.Users.OrderBy(u => u.Email), "Id", "Email", borrowingModel.UserId);
            return View(borrowingModel);
        }

        // POST: BorrowingModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
            ViewData["UserId"] = new SelectList(_context.Users.OrderBy(u => u.Email), "Id", "Email", borrowingModel.UserId);
            return View(borrowingModel);
        }

        // GET: BorrowingModels/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
