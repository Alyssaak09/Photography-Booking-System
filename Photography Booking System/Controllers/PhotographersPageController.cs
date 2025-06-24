using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photography_Booking_System.Data;
using Photography_Booking_System.Models;
using Photography_Booking_System.Models.ViewModels;

namespace Photography_Booking_System.Controllers
{
    public class PhotographersPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhotographersPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PhotographersPage
        public async Task<IActionResult> Index()
        {
            var photographers = await _context.Photographers.ToListAsync();
            return View(photographers);
        }

        // GET: PhotographersPage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var photographer = await _context.Photographers
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.PhotographerId == id);

            if (photographer == null) return NotFound();

            return View(photographer);
        }

        // GET: PhotographersPage/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PhotographersPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Photographer photographer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(photographer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(photographer);
        }

        // GET: PhotographersPage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var photographer = await _context.Photographers.FindAsync(id);
            if (photographer == null) return NotFound();

            return View(photographer);
        }

        // POST: PhotographersPage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Photographer photographer)
        {
            if (id != photographer.PhotographerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(photographer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Photographers.Any(p => p.PhotographerId == id))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(photographer);
        }

        // GET: PhotographersPage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var photographer = await _context.Photographers
                .FirstOrDefaultAsync(p => p.PhotographerId == id);
            if (photographer == null) return NotFound();

            return View(photographer);
        }

        // POST: PhotographersPage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var photographer = await _context.Photographers.FindAsync(id);
            if (photographer != null)
            {
                _context.Photographers.Remove(photographer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

