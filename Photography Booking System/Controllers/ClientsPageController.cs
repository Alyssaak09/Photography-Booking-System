using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photography_Booking_System.Data;
using Photography_Booking_System.Models;
using Photography_Booking_System.Models.ViewModels;

namespace Photography_Booking_System.Controllers
{
    public class ClientsPageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsPageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ClientsPage
        // Returns a list of all clients.
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients.ToListAsync();
            return View(clients);
        }

        // GET: ClientsPage/Details/5
        // Shows detailed info for a single client.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == id);
            if (client == null) return NotFound();

            return View(client);
        }

        // GET: ClientsPage/Create
        // Displays the form to create a new client.
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClientsPage/Create
        // Handles form submission to create a new client.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,Name,Email,PhoneNumber")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: ClientsPage/Edit/5
        // Displays the form to edit an existing client.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            return View(client);
        }

        // POST: ClientsPage/Edit/5
        // Handles form submission to update an existing client.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,Name,Email,PhoneNumber")] Client client)
        {
            if (id != client.ClientId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: ClientsPage/Delete/5
        // Displays a confirmation view to delete a client.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.ClientId == id);
            if (client == null) return NotFound();

            return View(client);
        }

        // POST: ClientsPage/Delete/5
        // Handles the deletion after confirmation.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method to check if a client exists in the database.
        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }
    }
}

