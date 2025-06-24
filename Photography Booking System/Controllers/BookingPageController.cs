using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photography_Booking_System.Data;
using Photography_Booking_System.Models;
using Photography_Booking_System.Models.ViewModels;

namespace Photography_Booking_System.Controllers
{
    public class BookingPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookingViewModels = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Photographer)
                .Select(b => new BookingSummaryViewModel
                {
                    BookingId = b.BookingId,
                    BookingDate = b.BookingDate,
                    Location = b.Location ?? string.Empty,
                    ClientName = b.Client != null ? b.Client.Name : "Unknown Client",
                    PhotographerName = b.Photographer != null ? b.Photographer.Name : "Unknown Photographer",
                    ServiceCount = _context.Booking_Services.Count(bs => bs.BookingId == b.BookingId)
                })
                .ToListAsync();

            return View(bookingViewModels);
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Photographer)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            var viewModel = new BookingDetailsViewModel
            {
                Booking = new DTOs.BookingDTO
                {
                    BookingId = booking.BookingId,
                    BookingDate = booking.BookingDate,
                    Location = booking.Location,
                    ClientId = booking.ClientId,
                    ClientName = booking.Client.Name,
                    PhotographerId = booking.PhotographerId,
                    PhotographerName = booking.Photographer.Name,
                    Services = booking.BookingServices.Select(bs => new DTOs.ServiceDTO
                    {
                        ServiceId = bs.ServiceId,
                        Name = bs.Service.Name,
                        Price = bs.Service.Price
                    }).ToList()
                }
            };

            return View(viewModel);
        }

        // GET: Bookings/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new BookingFormViewModel
            {
                Clients = await _context.Clients.ToListAsync(),
                Photographers = await _context.Photographers.ToListAsync(),
                Services = await _context.Services.ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Clients = await _context.Clients.ToListAsync();
                model.Photographers = await _context.Photographers.ToListAsync();
                model.Services = await _context.Services.ToListAsync();
                return View(model);
            }

            var booking = new Booking
            {
                BookingDate = model.BookingDate,
                Location = model.Location,
                ClientId = model.ClientId,
                PhotographerId = model.PhotographerId,
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            foreach (var serviceId in model.SelectedServiceIds)
            {
                _context.Booking_Services.Add(new Booking_Service
                {
                    BookingId = booking.BookingId,
                    ServiceId = serviceId
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingServices)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            var model = new BookingFormViewModel
            {
                BookingId = booking.BookingId,
                BookingDate = booking.BookingDate,
                Location = booking.Location,
                ClientId = booking.ClientId,
                PhotographerId = booking.PhotographerId,
                SelectedServiceIds = booking.BookingServices.Select(bs => bs.ServiceId).ToList(),
                Clients = await _context.Clients.ToListAsync(),
                Photographers = await _context.Photographers.ToListAsync(),
                Services = await _context.Services.ToListAsync()
            };

            return View(model);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookingFormViewModel model)
        {
            if (id != model.BookingId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                model.Clients = await _context.Clients.ToListAsync();
                model.Photographers = await _context.Photographers.ToListAsync();
                model.Services = await _context.Services.ToListAsync();
                return View(model);
            }

            var booking = await _context.Bookings
                .Include(b => b.BookingServices)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            booking.BookingDate = model.BookingDate;
            booking.Location = model.Location;
            booking.ClientId = model.ClientId;
            booking.PhotographerId = model.PhotographerId;

            _context.Booking_Services.RemoveRange(booking.BookingServices);

            booking.BookingServices = model.SelectedServiceIds.Select(sid => new Booking_Service
            {
                BookingId = booking.BookingId,
                ServiceId = sid
            }).ToList();

            _context.Update(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Client)
                .Include(b => b.Photographer)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingServices)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            _context.Booking_Services.RemoveRange(booking.BookingServices);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings by Service
        // URL: /BookingPage/ByService/5
        public async Task<IActionResult> ByService(int serviceId)
        {
            if (serviceId <= 0)
                return BadRequest("Invalid service ID.");

            var service = await _context.Services
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            if (service == null)
                return NotFound($"Service with ID {serviceId} not found.");

            var bookings = await _context.Bookings
                .Where(b => b.BookingServices.Any(bs => bs.ServiceId == serviceId))
                .Include(b => b.Client)
                .Include(b => b.Photographer)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .ToListAsync();

            if (!bookings.Any())
                return NotFound($"No bookings found for service '{service.Name}'.");

            var bookingViewModels = bookings.Select(b => new BookingSummaryViewModel
            {
                BookingId = b.BookingId,
                BookingDate = b.BookingDate,
                Location = b.Location ?? string.Empty,
                ClientName = b.Client?.Name ?? "Unknown Client",
                PhotographerName = b.Photographer?.Name ?? "Unknown Photographer",
                ServiceCount = b.BookingServices?.Count ?? 0
            }).ToList();

            ViewData["ServiceId"] = serviceId;
            ViewData["ServiceName"] = service.Name;

            return View(bookingViewModels);
        }

        // GET: Bookings by Photographer
        // URL: /BookingPage/ByPhotographer/5
        public async Task<IActionResult> ByPhotographer(int photographerId)
        {
            if (photographerId <= 0)
                return BadRequest("Invalid photographer ID.");

            var photographer = await _context.Photographers
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PhotographerId == photographerId);

            if (photographer == null)
                return NotFound($"Photographer with ID {photographerId} not found.");

            var bookings = await _context.Bookings
                .Where(b => b.PhotographerId == photographerId)
                .Include(b => b.Client)
                .Include(b => b.Photographer)
                .Include(b => b.BookingServices)
                    .ThenInclude(bs => bs.Service)
                .ToListAsync();

            if (!bookings.Any())
                return NotFound($"No bookings found for photographer '{photographer.Name}'.");

            var bookingViewModels = bookings.Select(b => new BookingSummaryViewModel
            {
                BookingId = b.BookingId,
                BookingDate = b.BookingDate,
                Location = b.Location ?? string.Empty,
                ClientName = b.Client?.Name ?? "Unknown Client",
                PhotographerName = b.Photographer?.Name ?? "Unknown Photographer",
                ServiceCount = b.BookingServices?.Count ?? 0
            }).ToList();

            ViewData["PhotographerName"] = photographer.Name;  

            return View(bookingViewModels);
        }

        // PUT: Bookings/Update/5
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, BookingFormViewModel model)
        {
            if (id != model.BookingId)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                model.Clients = await _context.Clients.ToListAsync();
                model.Photographers = await _context.Photographers.ToListAsync();
                model.Services = await _context.Services.ToListAsync();
                return View("Edit", model);
            }

            var booking = await _context.Bookings
                .Include(b => b.BookingServices)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            booking.BookingDate = model.BookingDate;
            booking.Location = model.Location;
            booking.ClientId = model.ClientId;
            booking.PhotographerId = model.PhotographerId;

            _context.Booking_Services.RemoveRange(booking.BookingServices);

            booking.BookingServices = model.SelectedServiceIds.Select(sid => new Booking_Service
            {
                BookingId = booking.BookingId,
                ServiceId = sid
            }).ToList();

            _context.Update(booking);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
