using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photography_Booking_System.Data;
using Photography_Booking_System.Models;
using Photography_Booking_System.Models.ViewModels;
using Photography_Booking_System.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace Photography_Booking_System.Controllers
{
    public class ServicesPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServicesPage
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        // GET: ServicesPage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services.FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null) return NotFound();

            var dto = new ServiceDTO
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Price = service.Price
            };

            return View(dto);
        }

        // GET: ServicesPage/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServicesPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price")] ServiceDTO serviceDto)
        {
            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    Name = serviceDto.Name,
                    Price = serviceDto.Price
                };
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceDto);
        }

        // GET: ServicesPage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            var dto = new ServiceDTO
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Price = service.Price
            };

            return View(dto);
        }

        // POST: ServicesPage/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceId,Name,Price")] ServiceDTO serviceDto)
        {
            if (id != serviceDto.ServiceId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var service = await _context.Services.FindAsync(id);
                    if (service == null) return NotFound();

                    service.Name = serviceDto.Name;
                    service.Price = serviceDto.Price;

                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Services.Any(e => e.ServiceId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(serviceDto);
        }

        // GET: ServicesPage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.Services.FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null) return NotFound();

            var dto = new ServiceDTO
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Price = service.Price
            };

            return View(dto);
        }

        // POST: ServicesPage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
