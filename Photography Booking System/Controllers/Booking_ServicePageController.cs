using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Photography_Booking_System.Data;
using Photography_Booking_System.Models;
using Photography_Booking_System.Models.ViewModels;

namespace Photography_Booking_System.Controllers
{
    public class Booking_ServicePageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Booking_ServicePageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Booking_Service
        public async Task<IActionResult> Index()
        {
            var bookingServices = _context.Booking_Services
                .Include(b => b.Booking)
                .Include(b => b.Service);
            return View(await bookingServices.ToListAsync());
        }

        // GET: Booking_Service/Details/5/10
        public async Task<IActionResult> Details(int? bookingId, int? serviceId)
        {
            if (bookingId == null || serviceId == null)
                return NotFound();

            var bookingService = await _context.Booking_Services
                .Include(b => b.Booking)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(m => m.BookingId == bookingId && m.ServiceId == serviceId);

            if (bookingService == null)
                return NotFound();

            return View(bookingService);
        }

        // GET: Booking_Service/Create
        public IActionResult Create()
        {
            var viewModel = new BookingServiceViewModel
            {
                Bookings = _context.Bookings.Select(b => new SelectListItem
                {
                    Value = b.BookingId.ToString(),
                    Text = b.BookingId.ToString()
                }),
                Services = _context.Services.Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = s.Name
                })
            };

            return View(viewModel);
        }

        // POST: Booking_Service/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingServiceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var bookingService = new Booking_Service
                {
                    BookingId = viewModel.BookingId,
                    ServiceId = viewModel.ServiceId
                };

                _context.Add(bookingService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdowns in case of validation failure
            viewModel.Bookings = _context.Bookings.Select(b => new SelectListItem
            {
                Value = b.BookingId.ToString(),
                Text = b.BookingId.ToString()
            });
            viewModel.Services = _context.Services.Select(s => new SelectListItem
            {
                Value = s.ServiceId.ToString(),
                Text = s.Name
            });

            return View(viewModel);
        }

        // GET: Booking_Service/Edit/5/10
        public async Task<IActionResult> Edit(int? bookingId, int? serviceId)
        {
            if (bookingId == null || serviceId == null)
                return NotFound();

            var bookingService = await _context.Booking_Services.FindAsync(bookingId, serviceId);
            if (bookingService == null)
                return NotFound();

            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId", bookingService.BookingId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", bookingService.ServiceId);
            return View(bookingService);
        }

        // POST: Booking_Service/Edit/5/10
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int bookingId, int serviceId, [Bind("BookingId,ServiceId")] Booking_Service bookingService)
        {
            if (bookingId != bookingService.BookingId || serviceId != bookingService.ServiceId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookingService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Booking_ServiceExists(bookingService.BookingId, bookingService.ServiceId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId", bookingService.BookingId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", bookingService.ServiceId);
            return View(bookingService);
        }

        // GET: Booking_Service/Delete/5/10
        public async Task<IActionResult> Delete(int? bookingId, int? serviceId)
        {
            if (bookingId == null || serviceId == null)
                return NotFound();

            var bookingService = await _context.Booking_Services
                .Include(b => b.Booking)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(m => m.BookingId == bookingId && m.ServiceId == serviceId);

            if (bookingService == null)
                return NotFound();

            return View(bookingService);
        }

        // POST: Booking_Service/Delete/5/10
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int bookingId, int serviceId)
        {
            var bookingService = await _context.Booking_Services.FindAsync(bookingId, serviceId);
            if (bookingService != null)
            {
                _context.Booking_Services.Remove(bookingService);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool Booking_ServiceExists(int bookingId, int serviceId)
        {
            return _context.Booking_Services.Any(e => e.BookingId == bookingId && e.ServiceId == serviceId);
        }
    }
}
